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
            ChoFileControlRecord r = new ChoFileControlRecord();
            r.Validate();
            Console.WriteLine(ChoFixedLengthWriter.ToText<ChoFileControlRecord>(r).Length);
        }
    }
}
