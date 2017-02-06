using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public class ChoNACHABatchWriter
    {
        private readonly ChoManifoldWriter _writer;
        private readonly ChoNACHARunningStat _fileRunningStatObject;
        private readonly ChoNACHARunningStat _batchRunningStatObject;
        private bool _isClosed = false;

        internal ChoNACHABatchWriter(ChoManifoldWriter writer, ChoNACHARunningStat fileRunningStatObject)
        {
            _writer = writer;
            _batchRunningStatObject = new ChoNACHARunningStat();

            _fileRunningStatObject = fileRunningStatObject;
            _batchRunningStatObject.BatchNumber = _fileRunningStatObject.NewBatch();
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
            if (_isClosed)
                throw new ChoNACHAException("Batch is in closed state.");
        }

        public void Close()
        {
            _fileRunningStatObject.IncRecordCountBy(_batchRunningStatObject.TotalNoOfRecord);
            _fileRunningStatObject.IncAddendaRecordCountBy(_batchRunningStatObject.AddendaEntryCount);
            _fileRunningStatObject.EntryHash += _batchRunningStatObject.EntryHash;
            _fileRunningStatObject.TotalDebitEntryDollarAmount += _batchRunningStatObject.TotalDebitEntryDollarAmount;
            _fileRunningStatObject.TotalCreditEntryDollarAmount += _batchRunningStatObject.TotalCreditEntryDollarAmount;
        }
    }

    internal class ChoNACHARunningStat
    {
        public uint BatchNumber { get; set; }
        public uint BatchCount { get; set; }
        public uint TotalNoOfRecord { get; set; }
        public uint AddendaEntryCount { get; set; }
        public ulong EntryHash { get; set; }
        public decimal TotalDebitEntryDollarAmount { get; set; }
        public decimal TotalCreditEntryDollarAmount { get; set; }

        public ChoNACHARunningStat()
        {
            TotalNoOfRecord = 2;
        }

        public uint NewBatch()
        {
            BatchCount++;
            return BatchNumber;
        }

        public void IncRecordCountBy(uint recCount = 1)
        {
            TotalNoOfRecord += recCount;
        }

        public void IncAddendaRecordCountBy(uint recCount = 1)
        {
            AddendaEntryCount += recCount;
        }

        public void UpdateStat(IEnumerable<ChoNACHAEntryDetailRecord> records, bool isDebit = false)
        {
            uint rc = (uint)records.Count();
            IncRecordCountBy(rc);
            EntryHash += (ulong)records.Sum(r => long.Parse(r.ReceivingDFIID));
            if (isDebit)
                TotalDebitEntryDollarAmount += records.Sum(r => r.Amount);
            else
                TotalCreditEntryDollarAmount += records.Sum(r => r.Amount);
        }

        public void UpdateStat(IEnumerable<ChoNACHAAddendaRecord> records)
        {
            uint rc = (uint)records.Count();
            IncRecordCountBy(rc);
            IncAddendaRecordCountBy(rc);
        }
    }
}
