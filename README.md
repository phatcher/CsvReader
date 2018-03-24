CSV Reader
==========

The [CsvReader](https://www.nuget.org/packages/LumenWorksCsvReader/) library is an extended version of Sébastien Lorion's [fast CSV Reader](http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader) project 
and provides fast parsing and reading of CSV files

[![NuGet](https://img.shields.io/nuget/v/LumenWorksCsvReader.svg)](https://www.nuget.org/packages/LumenWorksCsvReader/)
[![Build status](https://ci.appveyor.com/api/projects/status/ouvglmaox83bpyti/branch/master?svg=true)](https://ci.appveyor.com/project/PaulHatcher/csvreader/branch/master)

To this end it is a straight drop-in replacement for the existing NuGet package [LumenWorks.Framework.IO](https://www.nuget.org/packages/LumenWorks.Framework.IO/), but with additional
capabilities; the other rationale for the project is that the code is not available elsewhere in a public source repository, making it difficult to extend/contribute to.

Welcome to contributions from anyone.

You can see the version history [here](RELEASE_NOTES.md).

## Build the project
* Windows: Run *build.cmd*

The tooling should be automatically installed by paket/Fake. The default build will compile and test the project, and also produce a nuget package.

The library supports for .NET 2.0, 3.5, 4.5 and 4.6.1 and .netstandard 1.3 and 2.0, the netstandard1.3 version does not contain CachedCsvReader since the necessary interfaces are not available.

## Library License

The library is available under the [MIT License](http://en.wikipedia.org/wiki/MIT_License), for more information see the [License file][1] in the GitHub repository.

 [1]: https://github.com/phatcher/CsvReader/blob/master/License.md

## Getting Started
A good starting point is to look at Sébastien's [article](http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader) on Code Project.

A basic use of the reader something like this...
```csharp
using System.IO;
using LumenWorks.Framework.IO.Csv;

void ReadCsv()
{
    // open the file "data.csv" which is a CSV file with headers
    using (var csv = new CachedCsvReader(new StreamReader("data.csv"), true))
    {
        // Field headers will automatically be used as column names
        myDataGrid.DataSource = csv;
    }
}
```
Having said that, there are some extensions built into this version of the library that it is worth mentioning.

## Additional Features

### Columns
One addition is the addition of a Column list which holds the names and types of the data in the CSV file. If there are no headers present, we default the column names to Column1, Column2 etc; this can be overridden by setting the DefaultColumnHeader property e.g.
```csharp
using System.IO;
using LumenWorks.Framework.IO.Csv;

void ReadCsv()
{
    // open the file "data.csv" which is a CSV file with headers
    using (var csv = new CachedCsvReader(new StreamReader("data.csv"), false))
    {
        csv.DefaultColumnHeader = "Fred"

        // Field headers will now be Fred1, Fred2, etc
        myDataGrid.DataSource = csv;
    }
}
```

You can specify the columns yourself if there are none, and also specify the expected type; this is especially important when using against SqlBulkCopy which we will come back to later.
```csharp
using System.IO;
using LumenWorks.Framework.IO.Csv;

void ReadCsv()
{
    // open the file "data.csv" which is a CSV file with headers
    using (var csv = new CachedCsvReader(new StreamReader("data.csv"), false))
    {
        csv.Columns.Add(new Column { Name = "PriceDate", Type = typeof(DateTime) });
        csv.Columns.Add(new Column { Name = "OpenPrice", Type = typeof(decimal) });
        csv.Columns.Add(new Column { Name = "HighPrice", Type = typeof(decimal) });
        csv.Columns.Add(new Column { Name = "LowPrice", Type = typeof(decimal) });
        csv.Columns.Add(new Column { Name = "ClosePrice", Type = typeof(decimal) });
        csv.Columns.Add(new Column { Name = "Volume", Type = typeof(int) });

        // Field headers will now be picked from the Columns collection
        myDataGrid.DataSource = csv;
    }
}
```

### SQL Bulk Copy
One use of CSV Reader is to have a nice .NET way of using SQL Bulk Copy (SBC) rather than bcp for bulk loading of data into SQL Server.

A couple of issues arise when using SBC
    1. SBC wants the data presented as the correct type rather than as string
    2. You need to map between the table destination columns and the CSV if the order does not match *exactly*

Below is a example using the Columns collection to set up the correct metadata for SBC
```csharp
public void Import(string fileName, string connectionString)
{
    using (var reader = new CsvReader(new StreamReader(fileName), false))
    {
        reader.Columns = new List<LumenWorks.Framework.IO.Csv.Column>
        {
            new LumenWorks.Framework.IO.Csv.Column { Name = "PriceDate", Type = typeof(DateTime) },
            new LumenWorks.Framework.IO.Csv.Column { Name = "OpenPrice", Type = typeof(decimal) },
            new LumenWorks.Framework.IO.Csv.Column { Name = "HighPrice", Type = typeof(decimal) },
            new LumenWorks.Framework.IO.Csv.Column { Name = "LowPrice", Type = typeof(decimal) },
            new LumenWorks.Framework.IO.Csv.Column { Name = "ClosePrice", Type = typeof(decimal) },
            new LumenWorks.Framework.IO.Csv.Column { Name = "Volume", Type = typeof(int) },
        };

        // Now use SQL Bulk Copy to move the data
        using (var sbc = new SqlBulkCopy(connectionString))
        {
            sbc.DestinationTableName = "dbo.DailyPrice";
            sbc.BatchSize = 1000;

            sbc.AddColumnMapping("PriceDate", "PriceDate");
            sbc.AddColumnMapping("OpenPrice", "OpenPrice");
            sbc.AddColumnMapping("HighPrice", "HighPrice");
            sbc.AddColumnMapping("LowPrice", "LowPrice");
            sbc.AddColumnMapping("ClosePrice", "ClosePrice");
            sbc.AddColumnMapping("Volume", "Volume");

            sbc.WriteToServer(reader);
        }
    }
}
```
The method AddColumnMapping is an extension I wrote to simplify adding mappings to SBC
```csharp
public static class SqlBulkCopyExtensions
{
    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, int sourceColumnOrdinal, int targetColumnOrdinal)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumnOrdinal, targetColumnOrdinal);
        sbc.ColumnMappings.Add(map);

        return map;
    }

    public static SqlBulkCopyColumnMapping AddColumnMapping(this SqlBulkCopy sbc, string sourceColumn, string targetColumn)
    {
        var map = new SqlBulkCopyColumnMapping(sourceColumn, targetColumn);
        sbc.ColumnMappings.Add(map);

        return map;
    }
}
```

One other issue recently arose where we wanted to use SBC but some of the data was not in the file itself, but metadata that needed to be included on every row. The solution was to amend the CSV reader and Columns collection to allow default values to be provided that are not in the data.

The additional columns should be added at the end of the Columns collection to avoid interfering with the parsing, see the amended example below...
```csharp
public void Import(string fileName, string connectionString)
{
    using (var reader = new CsvReader(new StreamReader(fileName), false))
    {
        reader.Columns = new List<LumenWorks.Framework.IO.Csv.Column>
        {
            ...
            new LumenWorks.Framework.IO.Csv.Column { Name = "Volume", Type = typeof(int) },
            // NB Fake column so bulk import works
            new LumenWorks.Framework.IO.Csv.Column { Name = "Ticker", Type = typeof(string) },
        };

        // Fix up the column defaults with the values we need
        reader.UseColumnDefaults = true;
        reader.Columns[reader.GetFieldIndex("Ticker")] = Path.GetFileNameWithoutExtension(fileName);

        // Now use SQL Bulk Copy to move the data
        using (var sbc = new SqlBulkCopy(connectionString))
        {
            ...
            sbc.AddColumnMapping("Ticker", "Ticker");

            sbc.WriteToServer(reader);
        }
    }
}
```
To give an idea of performance, this took a naive sample app using an ORM from 2m 27s to 1.37s using SBC and the full import took just over 11m to import 9.8m records.

### Null Byte Removal StreamReader
Use NullRemovalStreamReader when CSV files contain large number of null bytes and you do not control how to generate CSV files.

If you ever experienced "System.OutOfMemoryException" or long processing time, you will most likely get a huge performance gain with NullRemovalStreamReader.
```csharp
public void Process(string path, bool addMark)
{
    using (StreamReader stream = new StreamReader(path))
    using (CsvReader csv = new CsvReader(stream.BaseStream, false, stream.CurrentEncoding, addMark))
    // or using (CsvReader csv = new CsvReader(File.OpenRead(path), false, Encoding.UTF8, addMark))
    {
        while (csv.ReadNextRecord())
        {
            string data = csv[i];
            // do stuff
        }
    }
}
```

When addMark is true, consecutive null bytes will be replaced by [removed x null bytes] to indicate the removal, you can see this from the benchmark output below.

Performance differences shown when tested with 20 million null bytes (20MB in storage) :
```csharp
CsvReader -     without using NullRemovalStreamReader : 536968 ticks, 0.2120 sec., 94.3518 MB/sec.

CsvReader - with NullRemovalStreamReader without mark : 191137 ticks, 0.0755 sec., 265.0660 MB/sec.
AddMark =(False) LastCell =(cell63 followed by 20971520 null bytes)

CsvReader - with NullRemovalStreamReader with    mark : 168819 ticks, 0.0666 sec., 300.1079 MB/sec.
AddMark =(True) LastCell =(cell63 followed by 20971520 null bytes[removed 20971520 null bytes])
```

Adjust number of null bytes in benchmark to see how much memory/time you will be able to save:
```csharp
X:\Path\CsvReader\build\Debug\CsvReaderBenchmarks\net461>CsvReaderBenchmarks.exe NullRemoval
```


## Performance
One of the main reasons for using this library is its excellent performance on reading/parsing raw data, here's a recent run of the benchmark (which is in the source)
```csharp
Test pass #1 - All fields

CsvReader - No cache      : 3134597 ticks, 1.2374 sec., 35.5582 MB/sec.
CachedCsvReader - Run 1   : 7452030 ticks, 2.9418 sec., 14.9571 MB/sec.
CachedCsvReader - Run 2   : 4525 ticks, 0.0018 sec., 24632.1821 MB/sec.
TextFieldParser           : 31568009 ticks, 12.4617 sec., 3.5308 MB/sec.
Regex                     : 11273590 ticks, 4.4503 sec., 9.8869 MB/sec.

Test pass #1 - Field #72 (middle)

CsvReader - No cache      : 2358656 ticks, 0.9311 sec., 47.2560 MB/sec.
CachedCsvReader - Run 1   : 7119186 ticks, 2.8104 sec., 15.6564 MB/sec.
CachedCsvReader - Run 2   : 325 ticks, 0.0001 sec., 342955.7662 MB/sec.
TextFieldParser           : 31171440 ticks, 12.3052 sec., 3.5757 MB/sec.
Regex                     : 5793093 ticks, 2.2869 sec., 19.2403 MB/sec.


Test pass #2 - All fields

CsvReader - No cache      : 2941954 ticks, 1.1614 sec., 37.8866 MB/sec.
CachedCsvReader - Run 1   : 7204077 ticks, 2.8439 sec., 15.4719 MB/sec.
CachedCsvReader - Run 2   : 314 ticks, 0.0001 sec., 354970.1401 MB/sec.
TextFieldParser           : 31213609 ticks, 12.3218 sec., 3.5709 MB/sec.
Regex                     : 11095897 ticks, 4.3802 sec., 10.0452 MB/sec.

Test pass #2 - Field #72 (middle)

CsvReader - No cache      : 2186909 ticks, 0.8633 sec., 50.9672 MB/sec.
CachedCsvReader - Run 1   : 7131654 ticks, 2.8153 sec., 15.6290 MB/sec.
CachedCsvReader - Run 2   : 296 ticks, 0.0001 sec., 376556.1622 MB/sec.
TextFieldParser           : 31381026 ticks, 12.3879 sec., 3.5518 MB/sec.
Regex                     : 5151353 ticks, 2.0335 sec., 21.6372 MB/sec.


Test pass #3 - All fields

CsvReader - No cache      : 2693834 ticks, 1.0634 sec., 41.3762 MB/sec.
CachedCsvReader - Run 1   : 7105358 ticks, 2.8049 sec., 15.6868 MB/sec.
CachedCsvReader - Run 2   : 326 ticks, 0.0001 sec., 341903.7546 MB/sec.
TextFieldParser           : 31323784 ticks, 12.3653 sec., 3.5583 MB/sec.
Regex                     : 11303752 ticks, 4.4622 sec., 9.8605 MB/sec.

Test pass #3 - Field #72 (middle)

CsvReader - No cache      : 2177773 ticks, 0.8597 sec., 51.1810 MB/sec.
CachedCsvReader - Run 1   : 7326816 ticks, 2.8923 sec., 15.2127 MB/sec.
CachedCsvReader - Run 2   : 328 ticks, 0.0001 sec., 339818.9756 MB/sec.
TextFieldParser           : 31168390 ticks, 12.3040 sec., 3.5761 MB/sec.
Regex                     : 5134853 ticks, 2.0270 sec., 21.7067 MB/sec.


Done
```
This was run on a high-spec machine (Xeon E5-2640, 32Gb RAM and M.2 1Tb SSD; you have to have some toys!) so the overall thoughput would be good, but CsvReader performs at 10x the speed the TextFieldParser and 5x faster than Regex
