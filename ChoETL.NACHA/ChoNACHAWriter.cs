using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAWriter : IDisposable
    {
        private TextWriter _textWriter;
        private bool _closeStreamOnDispose = false;
        private ChoManifoldWriter _writer;
        private bool _isDisposed = false;
        private ChoNACHABatchWriter _activeBatch = null;
        private ChoNACHAFileControlRecord _fileControlRecord = null;
        private ChoNACHARunningStat _runningStatObject = new ChoNACHARunningStat();
        private Lazy<object> _headerInitializer;


        public ChoManifoldWriter WriterHandle
        {
            get { return _writer; }
        }

        public ChoNACHAConfiguration Configuration
        {
            get;
            private set;
        }

        public ChoNACHAWriter(StringBuilder sb, ChoNACHAConfiguration configuration = null) : this(new StringWriter(sb), configuration)
        {

        }

        public ChoNACHAWriter(string filePath, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();

            _textWriter = new StreamWriter(ChoPath.GetFullPath(filePath), false, Configuration.Encoding, Configuration.BufferSize);
            _closeStreamOnDispose = true;

            Init();
        }

        public ChoNACHAWriter(TextWriter textWriter, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(textWriter, "TextWriter");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();
            _textWriter = textWriter;

            Init();
        }

        public ChoNACHAWriter(Stream inStream, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(inStream, "Stream");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();

            if (inStream is MemoryStream)
                _textWriter = new StreamWriter(inStream);
            else
                _textWriter = new StreamWriter(inStream, Configuration.Encoding, Configuration.BufferSize);

            Init();
        }

        private void Init()
        {
            _writer = new ChoManifoldWriter(_textWriter, Configuration as ChoManifoldRecordConfiguration).WithRecordSelector(0, 1, typeof(ChoNACHABatchHeaderRecord), typeof(ChoNACHABatchControlRecord),
               typeof(ChoNACHAFileHeaderRecord), typeof(ChoNACHAFileControlRecord), typeof(ChoNACHAEntryDetailRecord), typeof(ChoNACHAAddendaRecord));
            _writer.Configuration.ObjectValidationMode = ChoObjectValidationMode.ObjectLevel;

            _headerInitializer = new Lazy<object>(() =>
            {
                WriteFileHeader();
                _fileControlRecord = ChoActivator.CreateInstanceAndInit<ChoNACHAFileControlRecord>();
                return null;
            });
        }

        public ChoNACHABatchWriter CreateBatch(int serviceClassCode, string standardEntryClassCode = "PPD", string companyEntryDescription = null,
            DateTime? companyDescriptiveDate = null, DateTime? effectiveEntryDate = null, string julianSettlementDate = null,
            string companyDiscretionaryData = null, char originatorStatusCode = '1', string companyName = null, string companyID = null, string originatingDFIID = null,
            string isoOriginatingCurrencyCode = "USD", string isoDestinationCurrencyCode = "USD")
        {
            CheckDisposed();

            var x = _headerInitializer.Value;

            if (_activeBatch != null && !_activeBatch.IsClosed())
                throw new ChoNACHAException("There is already open batch associated with this writer which must be closed first.");

            //Increment batch count
            var batch = new ChoNACHABatchWriter(_writer, _runningStatObject, Configuration);
            batch.ServiceClassCode = serviceClassCode;
            batch.StandardEntryClassCode = standardEntryClassCode;
            batch.CompanyEntryDescription = companyEntryDescription;
            if (standardEntryClassCode != null && standardEntryClassCode.ToUpper().Trim() == "IAT")
                batch.CompanyDescriptiveDate = String.Format("{0}{1}", isoOriginatingCurrencyCode, isoDestinationCurrencyCode);
            else
                batch.CompanyDescriptiveDate = companyDescriptiveDate == null ? DateTime.Now.Date.ToString("yyMMdd") : companyDescriptiveDate.Value.Date.ToString("yyMMdd");

            batch.EffectiveEntryDate = effectiveEntryDate;
            batch.JulianSettlementDate = julianSettlementDate;
            batch.CompanyDiscretionaryData = companyDiscretionaryData;
            batch.OriginatorStatusCode = originatorStatusCode;

            batch.CompanyName = companyName.IsNullOrEmpty() ? Configuration.OriginatingCompanyName : companyName;
            batch.CompanyID = companyID.IsNullOrEmpty() ? Configuration.OriginatingCompanyId : companyID;
            batch.OriginatingDFIID = originatingDFIID.IsNullOrEmpty() ? Configuration.DestinationBankRoutingNumber.NTrim().First(8) : originatingDFIID.NTrim().First(8);

            _activeBatch = batch;

            return _activeBatch;
        }

        public void Close()
        {
            if (_isDisposed)
                return;

            var x = _headerInitializer.Value;

            CloseCurrentBatch();

            WriteFileControl();
            WriteFileControlFillerRecords();

            if (_closeStreamOnDispose)
                _textWriter.Dispose();

            _isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void CloseCurrentBatch()
        {
            if (_activeBatch != null && !_activeBatch.IsClosed())
                _activeBatch.Close();
        }

        private void WriteFileHeader()
        {
            ChoNACHAFileHeaderRecord header = ChoActivator.CreateInstanceAndInit<ChoNACHAFileHeaderRecord>();
            header.PriorityCode = Configuration.PriorityCode;
            header.ImmediateDestination = Configuration.DestinationBankRoutingNumber;
            header.ImmediateOrigin = Configuration.OriginatingCompanyId;
            header.FileIDModifier = Configuration.FileIDModifier;
            header.BlockingFactor = Configuration.BlockingFactor;
            header.FormatCode = Configuration.FormatCode;
            header.ImmediateDestinationName = Configuration.DestinationBankName;
            header.ImmediateOriginName = Configuration.OriginatingCompanyName;
            header.ReferenceCode = Configuration.ReferenceCode;

            header.Validate(Configuration);

            _writer.Write(header);
        }

        private void WriteFileControl()
        {
            if (_fileControlRecord != null)
            {
                _fileControlRecord.BatchCount = _runningStatObject.BatchCount;
                if (Configuration.BlockingFactor > 0)
                    _fileControlRecord.BlockCount = (ulong)(Math.Ceiling(_runningStatObject.TotalNoOfRecord / (Configuration.BlockingFactor * 1.0)));
                _fileControlRecord.EntryAddendaCount = _runningStatObject.AddendaEntryCount;
                _fileControlRecord.EntryHash = _runningStatObject.EntryHash;
                _fileControlRecord.TotalDebitEntryDollarAmount = _runningStatObject.TotalDebitEntryDollarAmount;
                _fileControlRecord.TotalCreditEntryDollarAmount = _runningStatObject.TotalCreditEntryDollarAmount;

                _writer.Write(_fileControlRecord);
            }
        }

        private void WriteFileControlFillerRecords()
        {
            if (Configuration.BlockingFactor <= 0)
                return;

            uint remain = _runningStatObject.TotalNoOfRecord % Configuration.BlockingFactor;
            if (remain <= 0)
                return;

            ChoNACHAFileControlRecord NACHAFileControlFillerRecord = ChoActivator.CreateInstanceAndInit<ChoNACHAFileControlRecord>();
            NACHAFileControlFillerRecord.BatchCount = 999999;
            NACHAFileControlFillerRecord.BlockCount = 999999;
            NACHAFileControlFillerRecord.EntryAddendaCount = 99999999;
            NACHAFileControlFillerRecord.EntryHash = 9999999999;
            NACHAFileControlFillerRecord.TotalDebitEntryDollarAmount = 9999999999.99M;
            NACHAFileControlFillerRecord.TotalCreditEntryDollarAmount = 9999999999.99M;
            NACHAFileControlFillerRecord.Reserved = ChoString.Repeat("9", 39);

            for (int i = 0; i < Configuration.BlockingFactor - remain; i++)
                _writer.Write(NACHAFileControlFillerRecord);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
