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
        private bool _isDisposed = false;

        private readonly ChoNACHABatchHeaderRecord _NACHABatchHeaderRecord = ChoActivator.CreateInstance<ChoNACHABatchHeaderRecord>();
        private readonly ChoNACHABatchControlRecord _NACHABatchControlRecord = ChoActivator.CreateInstance<ChoNACHABatchControlRecord>();
        private ChoNACHAConfiguration _configuration = null;
        private ChoNACHAEntryDetailWriter _activeEntry = null;
        private readonly Lazy<bool> _batchHeaderWriter = null;

        public string CompanyName { get; set; }
        public string CompanyID { get; set; }
        public string OriginatingDFIID { get; set; }

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
                WriteBatchHeader();
                return true;
            });
        }

        public ChoNACHAEntryDetailWriter CreateDebitEntryDetail(int transactionCode, string RDFIRoutingNumber, string DFIAccountNumber, decimal amount, string individualIDNumber, string individualName, string discretionaryData = null)
        {
            return CreateEntryDetail(transactionCode, RDFIRoutingNumber, DFIAccountNumber, amount, individualIDNumber, individualName, discretionaryData, true);

        }

        public ChoNACHAEntryDetailWriter CreateCreditEntryDetail(int transactionCode, string RDFIRoutingNumber, string DFIAccountNumber, decimal amount, string individualIDNumber, string individualName, string discretionaryData = null)
        {
            return CreateEntryDetail(transactionCode, RDFIRoutingNumber, DFIAccountNumber, amount, individualIDNumber, individualName, discretionaryData, false);
        }

        private ChoNACHAEntryDetailWriter CreateEntryDetail(int transactionCode, string RDFIRoutingNumber, string DFIAccountNumber, decimal amount, string individualIDNumber, string individualName, string discretionaryData = null, bool isDebit = false)
        {
            CheckDisposed();

            if (_activeEntry != null && !_activeEntry.IsClosed())
                throw new ChoNACHAException("There is already open entry detail associated with this writer which must be closed first.");

            var x = _batchHeaderWriter.Value;

            //Increment batch count
            _activeEntry = new ChoNACHAEntryDetailWriter(_writer, _batchRunningStatObject, _configuration);
            _activeEntry.TransactionCode = transactionCode;
            _activeEntry.ReceivingDFIID = ulong.Parse(RDFIRoutingNumber.NTrim().First(8));
            _activeEntry.CheckDigit = RDFIRoutingNumber.Last();
            _activeEntry.DFIAccountNumber = DFIAccountNumber;
            _activeEntry.Amount = amount;
            _activeEntry.IndividualIDNumber = individualIDNumber;
            _activeEntry.IndividualName = individualName;
            _activeEntry.DiscretionaryData = discretionaryData;
            uint tn = ++_fileRunningStatObject.TraceNumber;
            _activeEntry.TraceNumber = _configuration.DestinationBankRoutingNumber.NTrim().First(8) + tn.ToString().PadLeft(7, '0');
            _activeEntry.IsDebit = isDebit;

            return _activeEntry;

        }

        public bool IsClosed()
        {
            return _isDisposed;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void CloseCurrentEntry()
        {
            if (_activeEntry != null && !_activeEntry.IsClosed())
                _activeEntry.Close();
        }

        private void WriteBatchHeader()
        {
			_fileRunningStatObject.NewBatch();

			uint batchNumber = _configuration.BatchNumber;
			if (_configuration.BatchNumberGenerator != null)
				batchNumber = _configuration.BatchNumberGenerator();
			if (batchNumber == 0)
				batchNumber = new Random().Next(1, Int32.MaxValue);

			_NACHABatchHeaderRecord.BatchNumber = batchNumber == 0 ? 1 : batchNumber;
			_NACHABatchHeaderRecord.ServiceClassCode = ServiceClassCode;
            _NACHABatchHeaderRecord.CompanyName = CompanyName;
            _NACHABatchHeaderRecord.CompanyDiscretionaryData = CompanyDiscretionaryData;
            _NACHABatchHeaderRecord.CompanyID = CompanyID;
            _NACHABatchHeaderRecord.StandardEntryClassCode = StandardEntryClassCode;
            _NACHABatchHeaderRecord.CompanyEntryDescription = CompanyEntryDescription;
            _NACHABatchHeaderRecord.CompanyDescriptiveDate = CompanyDescriptiveDate;
            _NACHABatchHeaderRecord.EffectiveEntryDate = EffectiveEntryDate;
            _NACHABatchHeaderRecord.JulianSettlementDate = JulianSettlementDate;
            _NACHABatchHeaderRecord.OriginatorStatusCode = OriginatorStatusCode;
            _NACHABatchHeaderRecord.OriginatingDFIID = OriginatingDFIID;

            _writer.Write(_NACHABatchHeaderRecord);
        }

        private void WriteBatchControl()
        {
            _NACHABatchControlRecord.ServiceClassCode = _NACHABatchHeaderRecord.ServiceClassCode;
            _NACHABatchControlRecord.EntryAddendaCount = _batchRunningStatObject.AddendaEntryCount;
            _NACHABatchControlRecord.EntryHash = _batchRunningStatObject.EntryHash;
            _NACHABatchControlRecord.TotalDebitEntryDollarAmount = _batchRunningStatObject.TotalDebitEntryDollarAmount;
            _NACHABatchControlRecord.TotalCreditEntryDollarAmount = _batchRunningStatObject.TotalCreditEntryDollarAmount;
            _NACHABatchControlRecord.CompanyID = CompanyID;
            _NACHABatchControlRecord.MessageAuthenticationCode = MessageAuthenticationCode;
            _NACHABatchControlRecord.OriginatingDFIID = OriginatingDFIID;
            _NACHABatchControlRecord.BatchNumber = _NACHABatchHeaderRecord.BatchNumber;

            _writer.Write(_NACHABatchControlRecord);
        }

        public void Close()
        {
            if (_isDisposed)
                return;

            var x = _batchHeaderWriter.Value;

            CloseCurrentEntry();

            WriteBatchControl();

            _fileRunningStatObject.UpdateStat(_batchRunningStatObject);

            _isDisposed = true;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
