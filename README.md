[![github](https://img.shields.io/github/stars/Cinchoo/ChoETL.NACHA.svg)]()

# Cinchoo ETL - NACHA Library
.NET Library of NACHA file structure

This simple, nifty library exposes the .NET classes to read and process the NACHA files. These classes can be used in conjuction with Cinchoo ETL to read and generate ACH files easily.

## Install

To install Cinchoo ETL - NACHA Library, run the following command in the Package Manager Console

##### .NET Framework [![NuGet](https://img.shields.io/nuget/v/ChoETL.NACHA.svg)](https://www.nuget.org/packages/ChoETL.NACHA/)

    PM> Install-Package ChoETL.NACHA

##### .NET Core [![NuGet](https://img.shields.io/nuget/v/ChoETL.NACHA.NETStandard.svg)](https://www.nuget.org/packages/ChoETL.NACHA.NETStandard/)

    PM> Install-Package ChoETL.NACHA.NETStandard

Add namespace to the program

``` csharp
using ChoETL.NACHA;
```
# How to use

To read NACHA file (using c# 7, otherwise use if-else statement)

``` csharp
foreach (var r in new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
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
}
```

To write NACHA file

``` csharp
ChoNACHAConfiguration config = new ChoNACHAConfiguration();
config.DestinationBankRoutingNumber = "123456789";
config.OriginatingCompanyId = "123456789";
config.DestinationBankName = "PNC Bank";
config.OriginatingCompanyName = "Microsoft Inc.";
config.ReferenceCode = "Internal Use Only.";
config.BlockCount = 10;
using (var nachaWriter = new ChoNACHAWriter("ACH.txt", config))
{
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
```
# ChoNACHAConfiguration 
Here are the NACHA configuration available to configure while reading / writing NACHA files using Cinchoo NACHA.

- FieldValueTrimOption - Option to trim the values while parsing. Possible values are None, TrimStart, TrimEnd, Trim.
- PriorityCode - The lower the number, the higher processing priority. Currently, only 01 is used
- DestinationBankRoutingNumber - Number that identifies the bank site where it process the files.
- TurnOffDestinationBankRoutingNumber
- OriginatingCompanyId - is a number that identifyies entities, called originators, collecting payments.
- TurnOffOriginatingCompanyIdValidation
- FileIDModifier - Unique file identifier. Code to distinguish among multiple input files.
- BlockingFactor - a non-zero value, the NACHAWriter will generate the file with FileControl FILLER records in the last incomplete block.
- FormatCode - Currently there is only one code. Enter 1.
- DestinationBankName - Destination bank name.
- OriginatingCompanyName - Originating bank name.
- Reserved - reserved character used to File Trailer Record's (Type 9) Reserved field.
- ReferenceCode - Optional field you may use to describe input file for internal accounting purposes.
- BatchNumber - Number batches sequentially.
- BatchNumberGenerator - Custom batch number generator.
- EntryDetailTraceSource - Source of TraceNumber in Entry Detail Record (DestinationBankRoutingNumber/OriginatingDFI) 


More about Cinchoo NACHA library, visit [CodeProject](https://www.codeproject.com/Articles/1170069/Cinchoo-NACHA) Article

If this project help you reduce time to develop, you can give me a cup of coffee :)

[$10](https://buy.stripe.com/7sIdSt3Cg1OM8KIeV1)/[$25](https://buy.stripe.com/4gw3dP5KoeBy6CAcMR)/[$50](https://buy.stripe.com/28o15Hgp2gJG6CA4go)/[$100](https://buy.stripe.com/bIYbKl8WA2SQe523cl)
