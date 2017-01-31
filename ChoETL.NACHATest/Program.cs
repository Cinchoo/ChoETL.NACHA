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
            Console.WriteLine(ChoFixedLengthWriter.ToText<ChoBatchHeaderRecord>(ChoActivator.CreateInstance<ChoBatchHeaderRecord>()));
        }
    }
}
