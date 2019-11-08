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
            //ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Verbose;
            WriteACHFile();
        }

        static void ReadACHFile()
        {
            foreach (var r in new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
            {
                Console.WriteLine(r.ToString());
            }
        }

        static void WriteACHFile()
        {
            //ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Verbose;

            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = " 123456789";
            config.OriginatingCompanyId = "1234567890";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";

            //config.TurnOffOriginatingCompanyIdValidation = true;
            //config.ErrorMode = ChoErrorMode.ThrowAndStop;
            //config.BlockingFactor = 20;
            try
            {
                using (var nachaWriter = new ChoNACHAWriter(@"C:\NACHAFiles\TestACH.txt", config))
                {
                    using (var bw1 = nachaWriter.CreateBatch(200, "IAT", "test", DateTime.Now.Date, DateTime.Now.Date, null, "testdata", '1', "APMEX", "95123542", "12351231"))

                    {
                        //using (var entry1 = bw1.CreateDebitEntryDetail(22, "123456789", "1313131313", 22.505M, null, "ID Name", "Desc Data"))
                        //{
                        //    entry1.CreateAddendaRecord("Monthly bill");
                        //}
                        //using (var entry2 = bw1.CreateCreditEntryDetail(22, "123456789", "1313131313", 22.505M, null, "ID Name", "Desc Data"))
                        //{

                        //}
                        //using (var entry1 = bw1.CreateDebitEntryDetail(22, "123456789", "1313131313", 22.505M, null, "ID Name", "Desc Data"))
                        //{
                        //    entry1.CreateAddendaRecord("Monthly bill");
                        //}
                        using (var entry2 = bw1.CreateCreditEntryDetail(22, "123456789", "1313131313", 10M, null, "ID Name", "Desc Data"))
                        {

                        }
                        //using (var entry1 = bw1.CreateDebitEntryDetail(22, "123456789", "1313131313", 22.505M, null, "ID Name", "Desc Data"))
                        //{
                        //    entry1.CreateAddendaRecord("Monthly bill");
                        //}
                        using (var entry2 = bw1.CreateCreditEntryDetail(22, "123456789", "1313131313", 20M, null, "ID Name", "Desc Data"))
                        {

                        }
                        //Create offset record
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        try
                        {
                            using (var entry2 = bw1.CreateDebitEntryDetail(27, "bad entry", "1313131313", 30M, null, "OFFSET", ""))
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        try
                        {
                            using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                            {

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                        {

                        }
                    }
                    //using (var bw2 = nachaWriter.CreateBatch(200))
                    //{
                    //}
                }
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
                Console.ReadLine();
            }
            Console.ReadLine();
        }
    }
}
