# Cinchoo ETL - NACHA Library
.NET Library of NACHA file structure

This simple, nifty library exposes the .NET classes to read and process the NACHA files. These classes can be used in conjuction with Cinchoo ETL to read and generate ACH files easily.

## Install

To install Cinchoo PGP, run the following command in the Package Manager Console

    PM> Install-Package ChoETL.NACHA

Add namespace to the program

``` csharp
    using ChoETL.NACHA;
```
# How to use

To read NACHA file

``` csharp
foreach (var r in new ChoNACHAReader("20151027B0000327P018CHK.ACH"))
	Console.WriteLine(r.ToStringEx());
```

To write NACHA file

``` csharp
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
		b.CreateEntryDetail(11, 20, 123456789, 22.505M, "ID Number", "ID Name", "Desc Data");
	}
	using (var b1 = w.CreateBatch(200))
	{
	}
}
```
