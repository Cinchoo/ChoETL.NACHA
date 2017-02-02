using ChoETL.NACHA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoETL.NACHATest
{
    class Program
    {
        static void Main(string[] args)
        {
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

            foreach (var r in new ChoNACHAReader("ACH.txt"))
                Console.WriteLine(r.ToStringEx());
        }
    }
}
