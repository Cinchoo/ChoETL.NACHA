using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHABatchWriter : IDisposable
    {
        private readonly ChoManifoldWriter _writer;
        private readonly ChoNACHARunningStat _fileRunningStatObject;
        private readonly ChoNACHARunningStat _batchRunningStatObject;
        private bool _isClosed = false;

        private readonly ChoNACHABatchHeaderRecord _NACHABatchHeaderRecord = ChoActivator.CreateInstance<ChoNACHABatchHeaderRecord>();
        private readonly ChoNACHABatchControlRecord _NACHABatchControlRecord = ChoActivator.CreateInstance<ChoNACHABatchControlRecord>();
        private readonly Lazy<bool> _batchHeaderWriter = null;
        private ChoNACHAConfiguration _configuration = null;

        public int ServiceClassCode { get; set; }
        public string CompanyDiscretionaryData { get; set; }
        public string StandardEntryClassCode { get; set; }
        public string CompanyEntryDescription { get; set; }
        public DateTime? CompanyDescriptiveDate { get; set; }
        public DateTime? EffectiveEntryDate { get; set; }
        public string JulianSettlementDate { get; set; }
        public char OriginatorStatusCode { get; set; }

        public string MessageAuthenticationCode { get; set; }

        internal ChoNACHABatchWriter(ChoManifoldWriter writer, ChoNACHARunningStat fileRunningStatObject, ChoNACHAConfiguration configuration)
        {
            _configuration = configuration;
            _writer = writer;
            _batchRunningStatObject = new ChoNACHARunningStat();

            _fileRunningStatObject = fileRunningStatObject;

            _batchHeaderWriter = new Lazy<bool>(() =>
            {
                _NACHABatchHeaderRecord.BatchNumber = _fileRunningStatObject.NewBatch();
                _NACHABatchHeaderRecord.ServiceClassCode = ServiceClassCode;
                _NACHABatchHeaderRecord.CompanyName = configuration.OriginatingCompanyName;
                _NACHABatchHeaderRecord.CompanyDiscretionaryData = CompanyDiscretionaryData;
                _NACHABatchHeaderRecord.CompanyID = configuration.OriginatingCompanyId;
                _NACHABatchHeaderRecord.StandardEntryClassCode = StandardEntryClassCode;
                _NACHABatchHeaderRecord.CompanyEntryDescription = CompanyEntryDescription;
                _NACHABatchHeaderRecord.CompanyDescriptiveDate = CompanyDescriptiveDate;
                _NACHABatchHeaderRecord.EffectiveEntryDate = EffectiveEntryDate;
                _NACHABatchHeaderRecord.JulianSettlementDate = JulianSettlementDate;
                _NACHABatchHeaderRecord.OriginatorStatusCode = OriginatorStatusCode;
                _NACHABatchHeaderRecord.OriginatingDFIID = _configuration.DestinationBankRoutingNumber.Substring(0, _configuration.DestinationBankRoutingNumber.Length - 1);

                _writer.Write(_NACHABatchHeaderRecord);
                return true;
            });
        }

        public void Write(IEnumerable<ChoNACHAEntryDetailRecord> records, bool isDebit = false)
        {
            CheckState();

            _writer.Write(records);
            _batchRunningStatObject.UpdateStat(records, isDebit);
        }

        public void Write(ChoNACHAEntryDetailRecord record, bool isDebit = false)
        {
            Write(ChoEnumerable.AsEnumerable(record));
        }

        public void Write(IEnumerable<ChoNACHAAddendaRecord> records)
        {
            CheckState();

            _writer.Write(records);
            _batchRunningStatObject.UpdateStat(records);
        }

        public void Write(ChoNACHAAddendaRecord record)
        {
            Write(ChoEnumerable.AsEnumerable(record));
        }

        private void CheckState()
        {
            var r = _batchHeaderWriter.Value;
            if (_isClosed)
                throw new ChoNACHAException("Batch is in closed state.");
        }

        public void Close()
        {
            CheckState();

            var r = _batchHeaderWriter.Value;
            _NACHABatchControlRecord.ServiceClassCode = _NACHABatchHeaderRecord.ServiceClassCode;
            _NACHABatchControlRecord.EntryAddendaCount = _batchRunningStatObject.AddendaEntryCount;
            _NACHABatchControlRecord.EntryHash = _batchRunningStatObject.EntryHash;
            _NACHABatchControlRecord.TotalDebitEntryDollarAmount = _batchRunningStatObject.TotalDebitEntryDollarAmount;
            _NACHABatchControlRecord.TotalCreditEntryDollarAmount = _batchRunningStatObject.TotalCreditEntryDollarAmount;
            _NACHABatchControlRecord.CompanyID = _configuration.OriginatingCompanyId;
            _NACHABatchControlRecord.MessageAuthenticationCode = MessageAuthenticationCode;
            _NACHABatchControlRecord.OriginatingDFIID = _configuration.DestinationBankRoutingNumber.Substring(0, _configuration.DestinationBankRoutingNumber.Length - 1);
            _NACHABatchControlRecord.BatchNumber = _NACHABatchHeaderRecord.BatchNumber;

            _writer.Write(_NACHABatchControlRecord);
            _fileRunningStatObject.UpdateStat(_batchRunningStatObject);
            _isClosed = true;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
