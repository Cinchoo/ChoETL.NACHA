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
        private  ChoNACHABatchWriter _activeBatch = null;
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

            WriteFileHeader();
            _fileControlRecord = ChoActivator.CreateInstance<ChoNACHAFileControlRecord>();
        }

        private void IsDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public ChoNACHABatchWriter StartBatch(int serviceClassCode, string standardEntryClassCode = "PPD", string companyEntryDescription = null, 
            DateTime? companyDescriptiveDate = null, DateTime? effectiveEntryDate = null, string julianSettlementDate = null,
            string companyDiscretionaryData = null, char originatorStatusCode = '1')
        {
            if (_activeBatch != null)
                _activeBatch.Close();

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

            return _activeBatch;
        }

        public void Close()
        {
            Dispose();
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
            header.ImmediateOriginName = Configuration.DestinationBankName;
            header.ReferenceCode = Configuration.ReferenceCode;

            _writer.Write(header);
        }

        private void WriteFileControl()
        {
            _fileControlRecord.BatchCount = _runningStatObject.BatchCount;
            _fileControlRecord.BlockCount = _runningStatObject.TotalNoOfRecord % Configuration.BlockingFactor;

            _writer.Write(_fileControlRecord);
        }

        //public static string ToText(IEnumerable<ChoEntryDetailRecord> records, ChoNACHAConfiguration configuration = null)
        //{
        //    using (var stream = new MemoryStream())
        //    using (var reader = new StreamReader(stream))
        //    using (var writer = new StreamWriter(stream))
        //    using (var parser = new ChoNACHAWriter(writer, configuration))
        //    {
        //        parser.Write(records);

        //        writer.Flush();
        //        stream.Position = 0;

        //        return reader.ReadToEnd();
        //    }
        //}

        //public static string ToText(ChoEntryDetailRecord record, ChoNACHAConfiguration configuration = null)
        //{
        //    return ToText(ChoEnumerable.AsEnumerable(record), configuration);
        //}

        public void Dispose()
        {
            if (_activeBatch != null)
                _activeBatch.Close();

            WriteFileControl();

            if (_closeStreamOnDispose)
                _streamWriter.Dispose();

            _isDisposed = true;
        }
    }
}
