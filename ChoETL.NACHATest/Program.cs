using ChoETL.NACHA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHATest
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public void ReadACHFile()
        {
            foreach (var r in new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
                Console.WriteLine(r.ToStringEx());
        }

        public void WriteACHFile()
        {
            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = "123456789";
            config.OriginatingCompanyId = "123456789";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";
            using (var w = new ChoNACHAWriter("ACH.txt", config))
            {
                using (var b = w.CreateBatch(200))
                {
                    using (var e = b.CreateDebitEntryDetail(20, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
                    {
                        e.CreateAddendaRecord("Monthly bill");
                    }
                    using (b.CreateCreditEntryDetail(20, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
                    {

                    }
                }
                using (var b1 = w.CreateBatch(200))
                {
                }
            }
        }
    }
}
