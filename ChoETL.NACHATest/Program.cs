using ChoETL.NACHA;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            WriteACHFile1();
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");

            //var x = new ChoDateTimeConverter();
            //var c = x.Convert("121015", typeof(DateTime), "yyMMdd", new CultureInfo("hi-IN"));
            //Console.WriteLine(c);
            //return;

            ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Off;
            ReadACHFile();
            Console.ReadLine();
        }

        private static void ReadACHFile()
        {
            using (ChoNACHAReader fr = new ChoNACHAReader("20151027B0000327P018CHK.ACH", new ChoNACHAConfiguration() { FieldValueTrimOption = ChoFieldValueTrimOption.None }))
            {
                foreach (var r in fr)
                {
                    switch (r)
                    {
                        case ChoNACHAFileHeaderRecord fileHeaderRecord:
                            Console.WriteLine(fileHeaderRecord.ImmediateOrigin);
                            break;
                        case ChoNACHAFileControlRecord fileControlRecord:
                            Console.WriteLine(fileControlRecord.BatchCount);
                            break;
                        case ChoNACHABatchHeaderRecord batchHeaderRecord:
                            Console.WriteLine(batchHeaderRecord.BatchNumber);
                            break;
                        case ChoNACHABatchControlRecord batchControlRecord:
                            Console.WriteLine(batchControlRecord.BatchNumber);
                            break;
                        case ChoNACHAEntryDetailRecord entryDetailRecord:
                            Console.WriteLine(entryDetailRecord.DFIAccountNumber);
                            break;


                    }

                    //switch (r.GetType())
                    //{
                    //    case var type when type == typeof(ChoNACHAFileHeaderRecord):
                    //        var fileHeaderRecord = r as ChoNACHAFileHeaderRecord;
                    //        Console.WriteLine(fileHeaderRecord.ImmediateOrigin);
                    //        break;
                    //    case var type when type == typeof(ChoNACHAFileControlRecord):
                    //        var fileControlRecord = r as ChoNACHAFileControlRecord;
                    //        Console.WriteLine(fileControlRecord.BatchCount);
                    //        break;
                    //    case var type when type == typeof(ChoNACHABatchHeaderRecord):
                    //        var batchHeaderRecord = r as ChoNACHABatchHeaderRecord;
                    //        Console.WriteLine(batchHeaderRecord.BatchNumber);
                    //        break;
                    //    case var type when type == typeof(ChoNACHABatchControlRecord):
                    //        var batchControlRecord = r as ChoNACHABatchControlRecord;
                    //        Console.WriteLine(batchControlRecord.BatchNumber);
                    //        break;
                    //    case var type when type == typeof(ChoNACHAEntryDetailRecord):
                    //        var entryDetailRecord = r as ChoNACHAEntryDetailRecord;
                    //        Console.WriteLine(entryDetailRecord.DFIAccountNumber);
                    //        break;
                    //    default:
                    //        break;
                    //}
                }
            }

            //Console.WriteLine(r.ToStringEx());

            //if (r.ToString() == "ChoETL.NACHA.ChoNACHAEntryDetailRecord")
            //{
            //    ChoNACHAEntryDetailRecord detail = (ChoNACHAEntryDetailRecord)r;
            //    Console.WriteLine(detail.IndividualName.Length);
            //}

            //Console.WriteLine(r.ToStringEx());
        }

        private static void ReadNWriteACHFile()
        {
            //using (ChoNACHAReader r = new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
            //    Console.WriteLine(r.ToStringEx());
            //return;

            for (int i = 0; i < 3; i++)
            {
                var t = Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        using (ChoNACHAReader r = new ChoNACHAReader("20151027B0000327P018CHK_" + i + ".ACH"))
                        {
                            foreach (var rec in r)
                            {
                                //Console.WriteLine(rec.ToStringEx());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                });
            }
            return;


            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = "123456789";
            config.OriginatingCompanyId = "123456789";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";
            config.BlockingFactor = 10;
            using (var nachaWriter = new ChoNACHAWriter(@"C:\Projects\GitHub\ChoETL.NACHA\ChoETL.NACHATest\bin\Debug\NACHA_TEST.txt", config))
            {
                nachaWriter.Configuration.ErrorMode = ChoETL.ChoErrorMode.ThrowAndStop;

                using (var bw1 = nachaWriter.CreateBatch(200))
                {
                    using (var entry1 = bw1.CreateDebitEntryDetail(20, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
                    {
                        entry1.CreateAddendaRecord("Monthly bill");
                    }
                    using (var entry2 = bw1.CreateCreditEntryDetail(20, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
                    {

                    }
                }
                using (var bw2 = nachaWriter.CreateBatch(200))
                {
                }
            }
        }

        static void WriteACHFile1()
        {
            using (ChoNACHAReader fr = new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
            {
                ChoNACHAConfiguration config = new ChoNACHAConfiguration(); config.DestinationBankRoutingNumber = "123456789"; config.OriginatingCompanyId = "123456789"; config.DestinationBankName = "PNC Bank"; config.OriginatingCompanyName = "Microsoft Inc."; config.ReferenceCode = "Internal Use Only."; config.BlockingFactor = 10;
                using (var nachaWriter = new ChoNACHAWriter(@"NACHA_TEST.txt", config))
                {
                    nachaWriter.Configuration.ErrorMode = ChoETL.ChoErrorMode.IgnoreAndContinue;

                    using (var bw1 = nachaWriter.CreateBatch(200))
                    {
                        using (var entry1 = bw1.CreateDebitEntryDetail(20, "123456789", "1313131313", 22.505M, "", "ID Name", "Desc Data"))
                        {
                            entry1.CreateAddendaRecord("Monthly bill");
                        }
                        using (var entry2 = bw1.CreateCreditEntryDetail(20, "123456789", "1313131313", 22.505M, "", "ID Name", "Desc Data"))
                        {

                        }
                    }
                    using (var bw2 = nachaWriter.CreateBatch(200))
                    {
                    }
                }
            }

        }

        static void WriteACHFile()
        {
            ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Verbose;

            ChoNACHAConfiguration config = new ChoNACHAConfiguration();
            config.DestinationBankRoutingNumber = " 123456789";
            config.OriginatingCompanyId = "1234567890";
            config.DestinationBankName = "PNC Bank";
            config.OriginatingCompanyName = "Microsoft Inc.";
            config.ReferenceCode = "Internal Use Only.";

            //config.TurnOffOriginatingCompanyIdValidation = true;
            config.ErrorMode = ChoErrorMode.ThrowAndStop;
            //config.BlockingFactor = 20;

            //var nachaWriter1 = new ChoNACHAWriter(@"ACH1.txt", config);
            //var nachaWriter2 = new ChoNACHAWriter(@"ACH2.txt", config);

            ChoActivator.Factory = (t, args) =>
            {
                if (t == typeof(ChoNACHAFileHeaderRecord))
                {
                    var header = new ChoNACHAFileHeaderRecord();
                    header.Initialize();

                    //Overrride any values here...
                    header.FileCreationDate = DateTime.Today.AddDays(100);
                    return header;
                }
                return null;
            };

            using (var nachaWriter = new ChoNACHAWriter(@"ACH.txt", config)
                )
            {
                nachaWriter.Configuration.ErrorMode = ChoErrorMode.IgnoreAndContinue;
                //nachaWriter.WriterHandle.BeforeRecordWrite += (o, e) =>
                //{
                //    var rec = e.Record as ChoNACHAFileHeaderRecord;
                //    if (rec != null)
                //        rec.FileCreationDate = DateTime.Today.AddDays(100);
                //};
                using (var bw1 = nachaWriter.CreateBatch(200, companyName: "JR PROCUREMENT"))
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
                    try
                    {
                        using (var entry2 = bw1.CreateCreditEntryDetail(22, "5670x0", "1313131313", 10M, null, "ID Name", "Desc Data"))
                        {

                        }
                    }
                    catch { }
                    using (var entry1 = bw1.CreateDebitEntryDetail(22, "123456789", "1313131313", 22.505M, null, "ID Name", "Desc Data"))
                    {
                        entry1.CreateAddendaRecord("Monthly bill");
                    }
                    //using (var entry2 = bw1.CreateCreditEntryDetail(22, "123456789", "1313131313", 20M, null, "ID Name", "Desc Data"))
                    //{

                    //}
                    ////Create offset record
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "1234567890x0", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                    //using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 30M, null, "OFFSET", ""))
                    //{

                    //}
                }
                //using (var bw2 = nachaWriter.CreateBatch(200))
                //{
                //}
            }
        }
    }
}
