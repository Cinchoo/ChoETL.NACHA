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
        private StreamWriter _streamWriter;
        private bool _closeStreamOnDispose = false;
        private ChoManifoldWriter _writer;
        private bool _isDisposed = false;
        private ChoNACHABatchWriter _activeBatch = null;
        private ChoNACHAFileControlRecord _fileControlRecord = null;
        private ChoNACHARunningStat _runningStatObject = new ChoNACHARunningStat();

        public ChoNACHAConfiguration Configuration
        {
            get;
            private set;
        }

        public ChoNACHAWriter(string filePath, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();

            _streamWriter = new StreamWriter(ChoPath.GetFullPath(filePath), false, Configuration.Encoding, Configuration.BufferSize);
            _closeStreamOnDispose = true;

            Init();
        }

        public ChoNACHAWriter(StreamWriter streamWriter, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(streamWriter, "StreamWriter");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();
            _streamWriter = streamWriter;

            Init();
        }

        public ChoNACHAWriter(Stream inStream, ChoNACHAConfiguration configuration = null)
        {
            ChoGuard.ArgumentNotNull(inStream, "Stream");
            Configuration = configuration;
            if (Configuration == null)
                Configuration = new ChoNACHAConfiguration();

            _streamWriter = new StreamWriter(inStream, Configuration.Encoding, Configuration.BufferSize);
            _closeStreamOnDispose = true;

            Init();
        }

        private void Init()
        {
            _writer = new ChoManifoldWriter(_streamWriter, Configuration as ChoManifoldRecordConfiguration).WithRecordSelector(0, 1, typeof(ChoNACHABatchHeaderRecord), typeof(ChoNACHABatchControlRecord),
               typeof(ChoNACHAFileHeaderRecord), typeof(ChoNACHAFileControlRecord), typeof(ChoNACHAEntryDetailRecord), typeof(ChoNACHAAddendaRecord));
            _writer.Configuration.ObjectValidationMode = ChoObjectValidationMode.ObjectLevel;

            WriteFileHeader();
            _fileControlRecord = ChoActivator.CreateInstance<ChoNACHAFileControlRecord>();
        }

        public ChoNACHABatchWriter CreateBatch(int serviceClassCode, string standardEntryClassCode = "PPD", string companyEntryDescription = null,
            DateTime? companyDescriptiveDate = null, DateTime? effectiveEntryDate = null, string julianSettlementDate = null,
            string companyDiscretionaryData = null, char originatorStatusCode = '1', string companyName = null, string companyID = null, string originatingDFIID = null)
        {
            CheckDisposed();

            if (_activeBatch != null && !_activeBatch.IsClosed())
                throw new ChoNACHAException("There is already open batch associated with this writer which must be closed first.");

            //Increment batch count
            _activeBatch = new ChoNACHABatchWriter(_writer, _runningStatObject, Configuration);
            _activeBatch.ServiceClassCode = serviceClassCode;
            _activeBatch.StandardEntryClassCode = standardEntryClassCode;
            _activeBatch.CompanyEntryDescription = companyEntryDescription;
            _activeBatch.CompanyDescriptiveDate = companyDescriptiveDate;
            _activeBatch.EffectiveEntryDate = effectiveEntryDate;
            _activeBatch.JulianSettlementDate = julianSettlementDate;
            _activeBatch.CompanyDiscretionaryData = companyDiscretionaryData;
            _activeBatch.OriginatorStatusCode = originatorStatusCode;

            _activeBatch.CompanyName = companyName.IsNullOrEmpty() ? Configuration.OriginatingCompanyName : companyName;
            _activeBatch.CompanyID = companyID.IsNullOrEmpty() ? Configuration.OriginatingCompanyId : companyID;
            _activeBatch.OriginatingDFIID = originatingDFIID.IsNullOrEmpty() ? Configuration.DestinationBankRoutingNumber.First(8) : originatingDFIID.First(8);

            return _activeBatch;
        }

        public void Close()
        {
            if (_isDisposed)
                return;

            CloseCurrentBatch();

            WriteFileControl();
            WriteFileControlFillerRecords();

            if (_closeStreamOnDispose)
                _streamWriter.Dispose();

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
            ChoNACHAFileHeaderRecord header = ChoActivator.CreateInstance<ChoNACHAFileHeaderRecord>();
            header.PriorityCode = Configuration.PriorityCode;
            header.ImmediateDestination = Configuration.DestinationBankRoutingNumber;
            header.ImmediateOrigin = Configuration.OriginatingCompanyId;
            header.FileIDModifier = Configuration.FileIDModifier;
            header.BlockingFactor = Configuration.BlockingFactor;
            header.FormatCode = Configuration.FormatCode;
            header.ImmediateDestinationName = Configuration.DestinationBankName;
            header.ImmediateOriginName = Configuration.OriginatingCompanyName;
            header.ReferenceCode = Configuration.ReferenceCode;

            _writer.Write(header);
        }

        private void WriteFileControl()
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

        private void WriteFileControlFillerRecords()
        {
            if (Configuration.BlockingFactor <= 0)
                return;

            uint remain = _runningStatObject.TotalNoOfRecord % Configuration.BlockingFactor;
            if (remain <= 0)
                return;

            ChoNACHAFileControlRecord NACHAFileControlFillerRecord = ChoActivator.CreateInstance<ChoNACHAFileControlRecord>();
            NACHAFileControlFillerRecord.BatchCount = 999999;
            NACHAFileControlFillerRecord.BlockCount = 999999;
            NACHAFileControlFillerRecord.EntryAddendaCount = 99999999;
            NACHAFileControlFillerRecord.EntryHash = 9999999999;
            NACHAFileControlFillerRecord.TotalDebitEntryDollarAmount = 999999999999;
            NACHAFileControlFillerRecord.TotalCreditEntryDollarAmount = 999999999999;
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
