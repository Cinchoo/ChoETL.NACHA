[![github](https://img.shields.io/github/stars/Cinchoo/ChoETL.NACHA.svg)]()

# Cinchoo ETL - NACHA Library
.NET Library of NACHA file structure

This simple, nifty library exposes the .NET classes to read and process the NACHA files. These classes can be used in conjuction with Cinchoo ETL to read and generate ACH files easily.

## Install

To install Cinchoo PGP, run the following command in the Package Manager Console

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

If this project help you reduce time to develop, you can give me a cup of coffee :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=6S2UVXDPR63X8&source=url)
