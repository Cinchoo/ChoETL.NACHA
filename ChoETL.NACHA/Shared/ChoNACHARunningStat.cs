using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    internal class ChoNACHARunningStat
    {
        public uint TraceNumber { get; set; }
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

        public void UpdateStat(ChoNACHAEntryDetailRecord record, bool isDebit = false)
        {
            IncRecordCountBy(1);
            EntryHash += record.ReceivingDFIID;
            if (isDebit)
                TotalDebitEntryDollarAmount += record.Amount;
            else
                TotalCreditEntryDollarAmount += record.Amount;
        }

        public void UpdateStat(ChoNACHAAddendaRecord record)
        {
            IncRecordCountBy(1);
            IncAddendaRecordCountBy(1);
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
