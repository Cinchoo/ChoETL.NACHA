using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    internal class ChoNACHARunningStat
    {
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
            return ++BatchCount;
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
            EntryHash += (ulong)records.Sum(r => (long)r.ReceivingDFIID);
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

        public void UpdateStat(ChoNACHARunningStat src)
        {
            if (this == src) return;

            IncRecordCountBy(src.TotalNoOfRecord);
            IncAddendaRecordCountBy(src.AddendaEntryCount);
            EntryHash += src.EntryHash;
            TotalDebitEntryDollarAmount += src.TotalDebitEntryDollarAmount;
            TotalCreditEntryDollarAmount += src.TotalCreditEntryDollarAmount;
        }
    }
}
