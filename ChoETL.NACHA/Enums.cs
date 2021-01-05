using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHA
{
    public enum ChoRecordTypeCode
    {
        FileHeader = 1,
        BatchHeader = 5,
        EntryDetail = 6,
        Addenda = 7,
        BatchControl = 8,
        FileControl = 9
    }

    public enum ChoEntryDetailTraceSource
    {
        DestinationBankRoutingNumber = 0,
        OriginatingDFI = 1
    }
}
