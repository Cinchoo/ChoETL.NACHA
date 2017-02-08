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
            //string v = " 123456789";
            //Console.WriteLine(
            //    ((v.Length == 9 && !v.Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && v[0] == ' ' && !v.Skip(1).Where(c => !Char.IsDigit(c)).Any()) || (v.Length == 10 && !v.Where(c => !Char.IsDigit(c)).Any()))
            //    );
            //return;
            //double c = 232.1034;
            //Console.WriteLine(c.ToString("#.00").Replace(".", ""));
            //return;
            //ChoFileControlRecord r = new ChoFileControlRecord();
            //r.Validate();
            //Console.WriteLine(ChoFixedLengthWriter.ToText<ChoFileControlRecord>(r).Length);

            //foreach (var r in new ChoManifoldReader("ACH.txt").WithRecordSelector(0, 1, typeof(ChoBatchHeaderRecord), typeof(ChoBatchControlRecord),
            //    typeof(ChoFileHeaderRecord), typeof(ChoFileControlRecord), typeof(ChoEntryDetailRecord), typeof(ChoAddendaRecord)))
            //{
            //    Console.WriteLine(r.ToStringEx());
            //}

            //foreach (var r in new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
            //    Console.WriteLine(r.ToStringEx());
            //return;

            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = "123456789";
            config.OriginatingCompanyId = "123456789";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";
            //using (var stream = new MemoryStream())
            //using (var reader = new StreamReader(stream))
            //using (var writer = new StreamWriter(stream))
            //using (var w = new ChoNACHAWriter(writer, config))
            //{
            //    using (var b = w.StartBatch(200))
            //    {
            //    }
            //    using (var b1 = w.StartBatch(200))
            //    {
            //    }

            //    writer.Flush();
            //    stream.Position = 0;

            //    Console.WriteLine(reader.ReadToEnd());
            //}
            using (var w = new ChoNACHAWriter("ACH.txt", config))
            {
                using (var b = w.CreateBatch(200))
                {
                    b.CreateEntryDetail(11, 20, 123456789, 22.505M, "ID Number", "ID Name", "Desc Data");
                }
                using (var b1 = w.CreateBatch(200))
                {
                }
            }
        }

    }
}
