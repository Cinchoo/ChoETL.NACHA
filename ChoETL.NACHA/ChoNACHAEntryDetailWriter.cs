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

        private readonly ChoNACHAEntryDetailRecord _NACHAEntryDetailRecord = ChoActivator.CreateInstanceAndInit<ChoNACHAEntryDetailRecord>();
        private ChoNACHAConfiguration _configuration = null;
        private readonly Lazy<bool> _entryDetailWriter = null;

        public int TransactionCode { get; set; }
        public ulong ReceivingDFIID { get; set; }
        public char CheckDigit { get; set; }
        public string DFIAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string IndividualIDNumber { get; set; }
        public string IndividualName { get; set; }
        public string DiscretionaryData { get; set; }
        public string TraceNumber { get; set; }
        public bool IsDebit { get; set; }

        private uint _addendaSeqNo;

        internal ChoNACHAEntryDetailWriter(ChoManifoldWriter writer, ChoNACHARunningStat batchRunningStatObject, ChoNACHAConfiguration configuration)
        {
            _configuration = configuration;
            _writer = writer;
            _batchRunningStatObject = batchRunningStatObject;

            _entryDetailWriter = new Lazy<bool>(() =>
            {
                WriteEntryDetail();
                return true;
            });
        }

        public void CreateAddendaRecord(string paymentRelatedInformation, uint addendaTypeCode = 5)
        {
            CheckDisposed();

            _NACHAEntryDetailRecord.AddendaRecordIndicator = true;

            var x = _entryDetailWriter.Value;

            ChoNACHAAddendaRecord addendaRecord = ChoActivator.CreateInstanceAndInit<ChoNACHAAddendaRecord>();
            addendaRecord.AddendaTypeCode = addendaTypeCode;
            addendaRecord.PaymentRelatedInformation = paymentRelatedInformation;
            addendaRecord.AddendaSequenceNumber = ++_addendaSeqNo;
            addendaRecord.EntryDetailSequenceNumber = ulong.Parse(TraceNumber.ToString().Last(7));

            _batchRunningStatObject.UpdateStat(addendaRecord);

            _writer.Write(addendaRecord);
        }

        public void CreateReturnAddendaRecord(string returnReasonCode, string originalEntryTraceNumber, string OriginalReceivingDFIIdentification, string AddendaInformation = null, string DateOfDeath = null, uint addendaTypeCode = 5)
        {
            CheckDisposed();

            _NACHAEntryDetailRecord.AddendaRecordIndicator = true;

            var x = _entryDetailWriter.Value;

            ChoNACHAReturnAddendaRecord addendaRecord = ChoActivator.CreateInstanceAndInit<ChoNACHAReturnAddendaRecord>();
            addendaRecord.AddendaTypeCode = addendaTypeCode;
            addendaRecord.ReturnReasonCode = returnReasonCode;
            addendaRecord.OriginalEntryTraceNumber = originalEntryTraceNumber;
            addendaRecord.DateOfDeath = DateOfDeath;
            addendaRecord.OriginalReceivingDFIIdentification = OriginalReceivingDFIIdentification;
            addendaRecord.AddendaInformation = AddendaInformation;
            addendaRecord.EntryDetailSequenceNumber = ulong.Parse(TraceNumber.ToString());
            _batchRunningStatObject.UpdateStat(addendaRecord);

            _writer.Write(addendaRecord);

        }

        public bool IsClosed()
        {
            return _isDisposed;
        }

        private void WriteEntryDetail()
        {
            _NACHAEntryDetailRecord.TransactionCode = TransactionCode;
            _NACHAEntryDetailRecord.ReceivingDFIID = ReceivingDFIID;
            _NACHAEntryDetailRecord.CheckDigit = CheckDigit;
            _NACHAEntryDetailRecord.DFIAccountNumber = DFIAccountNumber;
            _NACHAEntryDetailRecord.Amount = Amount;
            _NACHAEntryDetailRecord.IndividualIDNumber = IndividualIDNumber;
            _NACHAEntryDetailRecord.IndividualName = IndividualName;
            _NACHAEntryDetailRecord.DiscretionaryData = DiscretionaryData;
            _NACHAEntryDetailRecord.TraceNumber = TraceNumber;
            _batchRunningStatObject.UpdateStat(_NACHAEntryDetailRecord, IsDebit);

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

            var x = _entryDetailWriter.Value;

            _batchRunningStatObject.UpdateStat(_batchRunningStatObject);

            _isDisposed = true;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
