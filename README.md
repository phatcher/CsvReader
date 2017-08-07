Repo: https://github.com/molopony/CsvReader
This is a forked version of LumenWorksCsvReader.

Changes since fork:
- Fixed: Exception when reading data and hasHeaders is false
- TargetFrameworkVersion: v2.0 => v4.6.1

See below for the original Readme text...

CSV Reader
==========

The [CsvReader](https://www.nuget.org/packages/LumenWorksCsvReader/) library is an extended version of Sébastien Lorion's [fast CSV Reader](http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader) project 
and provides fast parsing and reading of CSV files

[![NuGet](https://img.shields.io/nuget/v/LumenWorksCsvReader.svg)](https://img.shields.io/nuget/v/LumenWorksCsvReader.svg)
[![Build status](https://ci.appveyor.com/api/projects/status/ouvglmaox83bpyti/branch/master?svg=true)](https://ci.appveyor.com/project/PaulHatcher/csvreader/branch/master)

To this end it is a straight drop-in replacement for the existing NuGet package [LumenWork.Framework.IO](https://www.nuget.org/packages/LumenWorks.Framework.IO/), but with additional
capabilities; the other rationale for the project is that the code is not available elsewhere in a public source repository, making it difficult to extend/contribute to.

Welcome to contributions from anyone.

You can see the version history [here](RELEASE_NOTES.md).

## Build the project
* Windows: Run *build.cmd*

I have my tools in C:\Tools so I use *build.cmd Default tools=C:\Tools encoding=UTF-8*

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
	
## Performance
One of the main reasons for using this library is its excellent performance on reading/parsing raw data, here's a recent run of the benchmark (which is in the source)
```csharp
Test pass #1 - All fields

CsvReader - No cache      : 3495429 ticks, 1.7940 sec., 24.5265 MB/sec.
CachedCsvReader - Run 1   : 6641089 ticks, 3.4084 sec., 12.9091 MB/sec.
CachedCsvReader - Run 2   : 4393 ticks, 0.0023 sec., 19515.3071 MB/sec.
TextFieldParser           : 36877894 ticks, 18.9270 sec., 2.3247 MB/sec.
Regex                     : 15011358 ticks, 7.7044 sec., 5.7111 MB/sec.

Test pass #1 - Field #72 (middle)

CsvReader - No cache      : 2085871 ticks, 1.0705 sec., 41.1007 MB/sec.
CachedCsvReader - Run 1   : 6205399 ticks, 3.1848 sec., 13.8155 MB/sec.
CachedCsvReader - Run 2   : 214 ticks, 0.0001 sec., 400610.9533 MB/sec.
TextFieldParser           : 36458115 ticks, 18.7116 sec., 2.3515 MB/sec.
Regex                     : 6976827 ticks, 3.5808 sec., 12.2879 MB/sec.


Test pass #2 - All fields

CsvReader - No cache      : 3431492 ticks, 1.7612 sec., 24.9835 MB/sec.
CachedCsvReader - Run 1   : 6110812 ticks, 3.1363 sec., 14.0294 MB/sec.
CachedCsvReader - Run 2   : 173 ticks, 0.0001 sec., 495553.4335 MB/sec.
TextFieldParser           : 36671647 ticks, 18.8212 sec., 2.3378 MB/sec.
Regex                     : 15064341 ticks, 7.7315 sec., 5.6910 MB/sec.

Test pass #2 - Field #72 (middle)

CsvReader - No cache      : 2162568 ticks, 1.1099 sec., 39.6430 MB/sec.
CachedCsvReader - Run 1   : 5135074 ticks, 2.6355 sec., 16.6951 MB/sec.
CachedCsvReader - Run 2   : 220 ticks, 0.0001 sec., 389685.2000 MB/sec.
TextFieldParser           : 36913575 ticks, 18.9453 sec., 2.3225 MB/sec.
Regex                     : 7107108 ticks, 3.6476 sec., 12.0627 MB/sec.


Test pass #3 - All fields

CsvReader - No cache      : 3552781 ticks, 1.8234 sec., 24.1306 MB/sec.
CachedCsvReader - Run 1   : 5668694 ticks, 2.9094 sec., 15.1235 MB/sec.
CachedCsvReader - Run 2   : 186 ticks, 0.0001 sec., 460917.9785 MB/sec.
TextFieldParser           : 36650220 ticks, 18.8102 sec., 2.3392 MB/sec.
Regex                     : 15108079 ticks, 7.7540 sec., 5.6745 MB/sec.

Test pass #3 - Field #72 (middle)

CsvReader - No cache      : 2212999 ticks, 1.1358 sec., 38.7396 MB/sec.
CachedCsvReader - Run 1   : 5246701 ticks, 2.6928 sec., 16.3399 MB/sec.
CachedCsvReader - Run 2   : 214 ticks, 0.0001 sec., 400610.9533 MB/sec.
TextFieldParser           : 36718316 ticks, 18.8451 sec., 2.3348 MB/sec.
Regex                     : 7049832 ticks, 3.6182 sec., 12.1607 MB/sec.


Done
```
This was run on a high-spec machine (Xeon E5-2620, 16Gb RAM and 512Gb SSD; you have to have some toys!) so the overall thoughput would be good, but CsvReader performs at 10x the speed the TextFieldParser and 5x faster than Regex
