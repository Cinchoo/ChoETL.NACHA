using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHAEntryDetailWriter : IDisposable
    {
        private readonly ChoManifoldWriter _writer;
        private readonly ChoNACHARunningStat _batchRunningStatObject;
        private bool _isDisposed = false;

        private readonly ChoNACHAEntryDetailRecord _NACHAEntryDetailRecord = ChoActivator.CreateInstance<ChoNACHAEntryDetailRecord>();
        private ChoNACHAConfiguration _configuration = null;
        private readonly Lazy<bool> _batchHeaderWriter = null;

        public int TransactionCode { get; set; }
        public ulong ReceivingDFIID { get; set; }
        public char CheckDigit { get; set; }
        public decimal Amount { get; set; }
        public string IndividualIDNumber { get; set; }
        public string IndividualName { get; set; }
        public string DiscretionaryData { get; set; }
        public ulong TraceNumber { get; set; }

        internal ChoNACHAEntryDetailWriter(ChoManifoldWriter writer, ChoNACHARunningStat batchRunningStatObject, ChoNACHAConfiguration configuration)
        {
            _configuration = configuration;
            _writer = writer;
            _batchRunningStatObject = batchRunningStatObject;

            _batchHeaderWriter = new Lazy<bool>(() =>
            {
                WriteEntryDetail();
                return true;
            });
        }

        //public ChoNACHAEntryDetailWriter CreateEntryDetail()
        //{
        //    CheckDisposed();

        //    if (_activeEntry != null && !_activeEntry.IsClosed())
        //        throw new ChoNACHAException("There is already open entry detail associated with this writer which must be closed first.");

        //    var x = _batchHeaderWriter.Value;

        //    //Increment batch count
        //    _activeEntry = new ChoNACHAEntryDetailWriter(_writer, _batchRunningStatObject, _configuration);
        //    //_activeEntry.ServiceClassCode = serviceClassCode;
        //    //_activeEntry.StandardEntryClassCode = standardEntryClassCode;
        //    //_activeEntry.CompanyEntryDescription = companyEntryDescription;
        //    //_activeEntry.CompanyDescriptiveDate = companyDescriptiveDate;
        //    //_activeEntry.EffectiveEntryDate = effectiveEntryDate;
        //    //_activeEntry.JulianSettlementDate = julianSettlementDate;
        //    //_activeEntry.CompanyDiscretionaryData = companyDiscretionaryData;
        //    //_activeEntry.OriginatorStatusCode = originatorStatusCode;

        //    return _activeEntry;

        //}

        public bool IsClosed()
        {
            return _isDisposed;
        }

        private void WriteEntryDetail()
        {
            _NACHAEntryDetailRecord.TransactionCode = TransactionCode;
            _NACHAEntryDetailRecord.ReceivingDFIID = ReceivingDFIID;
            _NACHAEntryDetailRecord.CheckDigit = CheckDigit;
            _NACHAEntryDetailRecord.Amount = Amount;
            _NACHAEntryDetailRecord.IndividualIDNumber = IndividualIDNumber;
            _NACHAEntryDetailRecord.IndividualName = IndividualName;
            _NACHAEntryDetailRecord.DiscretionaryData = DiscretionaryData;
            _NACHAEntryDetailRecord.TraceNumber = TraceNumber;

            _writer.Write(_NACHAEntryDetailRecord);
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Close()
        {
            if (_isDisposed)
                return;

            var x = _batchHeaderWriter.Value;

            _batchRunningStatObject.UpdateStat(_batchRunningStatObject);

            _isDisposed = true;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
