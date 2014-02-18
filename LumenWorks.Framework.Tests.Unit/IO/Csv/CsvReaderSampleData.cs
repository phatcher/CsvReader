//	LumenWorks.Framework.Tests.Unit.IO.CSV.CsvReaderSampleData
//	Copyright (c) 2005 Sébastien Lorion
//
//	MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

using NUnit.Framework;

using LumenWorks.Framework.IO.Csv;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
	public class CsvReaderSampleData
	{
		#region Sample data

		public const int SampleData1RecordCount = 6;
		public const int SampleData1FieldCount = 6;

		public const string SampleData1Header0 = "First Name";
		public const string SampleData1Header1 = "Last Name";
		public const string SampleData1Header2 = "Address";
		public const string SampleData1Header3 = "City";
		public const string SampleData1Header4 = "State";
		public const string SampleData1Header5 = "Zip Code";

		// <blank>
		// # This is a comment
		// "First Name", "Last Name", Address, City, State, "Zip Code"<tab>
		// John,Doe,120 jefferson st.,Riverside, NJ, 08075
		// Jack,McGinnis,220 hobo Av.,Phila<tab>, PA,09119
		// "John ""Da Man""",Repici,120 Jefferson St.,Riverside, NJ,08075
		// <blank>
		// # This is a comment
		// Stephen,Tyler,"7452 Terrace ""At the Plaza"" road",SomeTown,SD, 91234
		// ,Blankman,,SomeTown, SD, 00298
		// "Joan ""the bone"", Anne",Jet,"9th, at Terrace plc",Desert City,CO,00123

		public const string SampleData1 = @"
# This is a comment
""First Name"", ""Last Name"", Address, City, State, ""Zip Code""	
John,Doe,120 jefferson st.,Riverside, NJ, 08075
Jack,McGinnis,220 hobo Av.,Phila	, PA,09119
""John """"Da Man"""""",Repici,120 Jefferson St.,Riverside, NJ,08075

# This is a comment
Stephen,Tyler,""7452 Terrace """"At the Plaza"""" road"",SomeTown,SD, 91234
,Blankman,,SomeTown, SD, 00298
""Joan """"the bone"""", Anne"",Jet,""9th, at Terrace plc"",Desert City,CO,00123";

		public const string SampleTypedData1 = @"
System.Boolean,System.DateTime,System.Single,System.Double,System.Decimal,System.SByte,System.Int16,System.Int32,System.Int64,System.Byte,System.UInt16,System.UInt32,System.UInt64,System.Char,System.String,System.Guid,System.DBNull
1,2001-01-01,1,1,1,1,1,1,1,1,1,1,1,a,abc,{11111111-1111-1111-1111-111111111111},
""true"",""2001-01-01"",""1"",""1"",""1"",""1"",""1"",""1"",""1"",""1"",""1"",""1"",""1"",""a"",""abc"",""{11111111-1111-1111-1111-111111111111}"",""""";

		#endregion

		#region Sample data utility methods

		public static void CheckSampleData1(CsvReader csv, bool readToEnd)
		{
			if (readToEnd)
			{
				Assert.AreEqual(CsvReaderSampleData.SampleData1FieldCount, csv.FieldCount);

				if (csv.HasHeaders)
				{
					Assert.AreEqual(0, csv.GetFieldIndex(SampleData1Header0));
					Assert.AreEqual(1, csv.GetFieldIndex(SampleData1Header1));
					Assert.AreEqual(2, csv.GetFieldIndex(SampleData1Header2));
					Assert.AreEqual(3, csv.GetFieldIndex(SampleData1Header3));
					Assert.AreEqual(4, csv.GetFieldIndex(SampleData1Header4));
					Assert.AreEqual(5, csv.GetFieldIndex(SampleData1Header5));
				}
				
				Assert.AreEqual(-1, csv.CurrentRecordIndex);

				int recordCount = 0;

				while (csv.ReadNextRecord())
				{
					CheckSampleData1(csv.CurrentRecordIndex, csv);
					recordCount++;
				}

				if (csv.HasHeaders)
					Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, recordCount);
				else
					Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount + 1, recordCount);
			}
			else
				CheckSampleData1(csv.CurrentRecordIndex, csv);
		}

		public static void CheckSampleData1(long recordIndex, CsvReader csv)
		{
			string[] fields = new string[6];
			csv.CopyCurrentRecordTo(fields);

			CheckSampleData1(csv.HasHeaders, recordIndex, fields, 0);
		}

		public static void CheckSampleData1(bool hasHeaders, long recordIndex, string[] fields)
		{
			CheckSampleData1(hasHeaders, recordIndex, fields, 0);
		}

		public static void CheckSampleData1(bool hasHeaders, long recordIndex, string[] fields, int startIndex)
		{
			Assert.IsTrue(fields.Length - startIndex >= 6);

			long index = recordIndex;

			if (hasHeaders)
				index++;

			switch (index)
			{
				case 0:
					Assert.AreEqual(SampleData1Header0, fields[startIndex]);
					Assert.AreEqual(SampleData1Header1, fields[startIndex + 1]);
					Assert.AreEqual(SampleData1Header2, fields[startIndex + 2]);
					Assert.AreEqual(SampleData1Header3, fields[startIndex + 3]);
					Assert.AreEqual(SampleData1Header4, fields[startIndex + 4]);
					Assert.AreEqual(SampleData1Header5, fields[startIndex + 5]);
					break;

				case 1:
					Assert.AreEqual("John", fields[startIndex]);
					Assert.AreEqual("Doe", fields[startIndex + 1]);
					Assert.AreEqual("120 jefferson st.", fields[startIndex + 2]);
					Assert.AreEqual("Riverside", fields[startIndex + 3]);
					Assert.AreEqual("NJ", fields[startIndex + 4]);
					Assert.AreEqual("08075", fields[startIndex + 5]);
					break;

				case 2:
					Assert.AreEqual("Jack", fields[startIndex]);
					Assert.AreEqual("McGinnis", fields[startIndex + 1]);
					Assert.AreEqual("220 hobo Av.", fields[startIndex + 2]);
					Assert.AreEqual("Phila", fields[startIndex + 3]);
					Assert.AreEqual("PA", fields[startIndex + 4]);
					Assert.AreEqual("09119", fields[startIndex + 5]);
					break;

				case 3:
					Assert.AreEqual(@"John ""Da Man""", fields[startIndex]);
					Assert.AreEqual("Repici", fields[startIndex + 1]);
					Assert.AreEqual("120 Jefferson St.", fields[startIndex + 2]);
					Assert.AreEqual("Riverside", fields[startIndex + 3]);
					Assert.AreEqual("NJ", fields[startIndex + 4]);
					Assert.AreEqual("08075", fields[startIndex + 5]);
					break;

				case 4:
					Assert.AreEqual("Stephen", fields[startIndex]);
					Assert.AreEqual("Tyler", fields[startIndex + 1]);
					Assert.AreEqual(@"7452 Terrace ""At the Plaza"" road", fields[startIndex + 2]);
					Assert.AreEqual("SomeTown", fields[startIndex + 3]);
					Assert.AreEqual("SD", fields[startIndex + 4]);
					Assert.AreEqual("91234", fields[startIndex + 5]);
					break;

				case 5:
					Assert.AreEqual("", fields[startIndex]);
					Assert.AreEqual("Blankman", fields[startIndex + 1]);
					Assert.AreEqual("", fields[startIndex + 2]);
					Assert.AreEqual("SomeTown", fields[startIndex + 3]);
					Assert.AreEqual("SD", fields[startIndex + 4]);
					Assert.AreEqual("00298", fields[startIndex + 5]);
					break;

				case 6:
					Assert.AreEqual(@"Joan ""the bone"", Anne", fields[startIndex]);
					Assert.AreEqual("Jet", fields[startIndex + 1]);
					Assert.AreEqual("9th, at Terrace plc", fields[startIndex + 2]);
					Assert.AreEqual("Desert City", fields[startIndex + 3]);
					Assert.AreEqual("CO", fields[startIndex + 4]);
					Assert.AreEqual("00123", fields[startIndex + 5]);
					break;

				default:
					throw new IndexOutOfRangeException(string.Format("Specified recordIndex is '{0}'. Possible range is [0, 5].", recordIndex));
			}
		}

		#endregion
	}
}
