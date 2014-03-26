//	LumenWorks.Framework.Tests.Unit.IO.CSV.CsvReaderMalformedTest
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


// A special thanks goes to "shriop" at CodeProject for providing many of the standard and Unicode parsing tests.


using System;
using System.Globalization;
using System.IO;
using System.Text;

using NUnit.Framework;

using LumenWorks.Framework.IO.Csv;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
	[TestFixture()]
	public class CsvReaderMalformedTest
	{
		#region Utilities

		private void CheckMissingFieldUnquoted(long recordCount, int fieldCount, long badRecordIndex, int badFieldIndex, int bufferSize)
		{
			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, true, MissingFieldAction.ParseError);
			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, true, MissingFieldAction.ReplaceByEmpty);
			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, true, MissingFieldAction.ReplaceByNull);

			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, false, MissingFieldAction.ParseError);
			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, false, MissingFieldAction.ReplaceByEmpty);
			CheckMissingFieldUnquoted(recordCount, fieldCount, badRecordIndex, badFieldIndex, bufferSize, false, MissingFieldAction.ReplaceByNull);
		}

		private void CheckMissingFieldUnquoted(long recordCount, int fieldCount, long badRecordIndex, int badFieldIndex, int bufferSize, bool sequentialAccess, MissingFieldAction action)
		{
			// construct the csv data with template "00,01,02\n10,11,12\n...." and calculate expected error position

			long capacity = recordCount * (fieldCount * 2 + fieldCount - 1) + recordCount;
			Assert.IsTrue(capacity <= int.MaxValue);

			StringBuilder sb = new StringBuilder((int) capacity);
			int expectedErrorPosition = 0;

			for (long i = 0; i < recordCount; i++)
			{
				int realFieldCount;

				if (i == badRecordIndex)
					realFieldCount = badFieldIndex;
				else
					realFieldCount = fieldCount;

				for (int j = 0; j < realFieldCount; j++)
				{
					sb.Append(i);
					sb.Append(j);
					sb.Append(CsvReader.DefaultDelimiter);
				}

				sb.Length--;
				sb.Append('\n');

				if (i == badRecordIndex)
				{
					expectedErrorPosition = sb.Length % bufferSize;

					// when eof is true, buffer is cleared and position is reset to 0, so exception will have CurrentPosition = 0
					if (i == recordCount - 1)
						expectedErrorPosition = 0;
				}
			}

			// test csv

			using (CsvReader csv = new CsvReader(new StringReader(sb.ToString()), false, bufferSize))
			{
				csv.MissingFieldAction = action;
				Assert.AreEqual(fieldCount, csv.FieldCount);

				while (csv.ReadNextRecord())
				{
					Assert.AreEqual(fieldCount, csv.FieldCount);

					// if not sequential, directly test the missing field
					if (!sequentialAccess)
						CheckMissingFieldValueUnquoted(csv, badFieldIndex, badRecordIndex, badFieldIndex, expectedErrorPosition, sequentialAccess, action);

					for (int i = 0; i < csv.FieldCount; i++)
						CheckMissingFieldValueUnquoted(csv, i, badRecordIndex, badFieldIndex, expectedErrorPosition, sequentialAccess, action);
				}
			}
		}

		private void CheckMissingFieldValueUnquoted(CsvReader csv, int fieldIndex, long badRecordIndex, int badFieldIndex, int expectedErrorPosition, bool sequentialAccess, MissingFieldAction action)
		{
			const string Message = "RecordIndex={0}; FieldIndex={1}; Position={2}; Sequential={3}; Action={4}";

			// make sure s contains garbage as to not have false successes
			string s = "asdfasdfasdf";

			try
			{
				s = csv[fieldIndex];
			}
			catch (MissingFieldCsvException ex)
			{
				Assert.AreEqual(badRecordIndex, ex.CurrentRecordIndex, Message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);
				Assert.IsTrue(fieldIndex >= badFieldIndex, Message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);
				Assert.AreEqual(expectedErrorPosition, ex.CurrentPosition, Message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);

				return;
			}

			if (csv.CurrentRecordIndex != badRecordIndex || fieldIndex < badFieldIndex)
				Assert.AreEqual(csv.CurrentRecordIndex.ToString() + fieldIndex.ToString(), s, Message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
			else
			{
				switch (action)
				{
					case MissingFieldAction.ReplaceByEmpty:
						Assert.AreEqual(string.Empty, s, Message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
						break;

					case MissingFieldAction.ReplaceByNull:
						Assert.IsNull(s, Message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
						break;

					case MissingFieldAction.ParseError:
						Assert.Fail("Failed to throw ParseError. - " + Message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
						break;

					default:
						Assert.Fail("'{0}' is not handled by this test.", action);
						break;
				}
			}
		}

		#endregion

		[Test()]
		public void MissingFieldUnquotedTest1()
		{
			CheckMissingFieldUnquoted(4, 4, 2, 2, CsvReader.DefaultBufferSize);
			CheckMissingFieldUnquoted(4, 4, 2, 2, CsvReader.DefaultBufferSize);
		}

		[Test()]
		public void MissingFieldUnquotedTest2()
		{
			// With bufferSize = 16, faulty new line char is at the start of next buffer read
			CheckMissingFieldUnquoted(4, 4, 2, 3, 16);
		}

		[Test()]
		public void MissingFieldUnquotedTest3()
		{
			// test missing field when end of buffer has been reached
			CheckMissingFieldUnquoted(3, 4, 2, 3, 16);
		}

		[Test()]
		[ExpectedException(typeof(MissingFieldCsvException))]
		public void MissingFieldQuotedTest1()
		{
			const string Data = "a,b,c,d\n1,1,1,1\n2,\"2\"\n3,3,3,3";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MissingFieldCsvException ex)
			{
				if (ex.CurrentRecordIndex == 2 && ex.CurrentFieldIndex == 2 && ex.CurrentPosition == 22)
					throw ex;
			}
		}

		[Test()]
		[ExpectedException(typeof(MissingFieldCsvException))]
		public void MissingFieldQuotedTest2()
		{
			const string Data = "a,b,c,d\n1,1,1,1\n2,\"2\",\n3,3,3,3";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false, 11))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MissingFieldCsvException ex)
			{
				if (ex.CurrentRecordIndex == 2 && ex.CurrentFieldIndex == 2 && ex.CurrentPosition == 1)
					throw ex;
			}
		}

		[Test()]
		[ExpectedException(typeof(MissingFieldCsvException))]
		public void MissingFieldQuotedTest3()
		{
			const string Data = "a,b,c,d\n1,1,1,1\n2,\"2\"\n\"3\",3,3,3";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MissingFieldCsvException ex)
			{
				if (ex.CurrentRecordIndex == 2 && ex.CurrentFieldIndex == 2 && ex.CurrentPosition == 22)
					throw ex;
			}
		}

		[Test()]
		[ExpectedException(typeof(MissingFieldCsvException))]
		public void MissingFieldQuotedTest4()
		{
			const string Data = "a,b,c,d\n1,1,1,1\n2,\"2\",\n\"3\",3,3,3";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false, 11))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MissingFieldCsvException ex)
			{
				if (ex.CurrentRecordIndex == 2 && ex.CurrentFieldIndex == 2 && ex.CurrentPosition == 1)
					throw ex;
			}
		}

		[Test()]
		[ExpectedException(typeof(MalformedCsvException))]
		public void MissingDelimiterAfterQuotedFieldTest1()
		{
			const string Data = "\"111\",\"222\"\"333\"";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false, ',', '"', '\\', '#', ValueTrimmingOptions.UnquotedOnly))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MalformedCsvException ex)
			{
				if (ex.CurrentRecordIndex == 0 && ex.CurrentFieldIndex ==1 && ex.CurrentPosition == 11)
					throw ex;
			}
		}

		[Test()]
		[ExpectedException(typeof(MalformedCsvException))]
		public void MissingDelimiterAfterQuotedFieldTest2()
		{
			const string Data = "\"111\",\"222\",\"333\"\n\"111\",\"222\"\"333\"";

			try
			{
				using (CsvReader csv = new CsvReader(new StringReader(Data), false, ',', '"', '\\', '#', ValueTrimmingOptions.UnquotedOnly))
				{
					while (csv.ReadNextRecord())
						for (int i = 0; i < csv.FieldCount; i++)
						{
							string s = csv[i];
						}
				}
			}
			catch (MalformedCsvException ex)
			{
				if (ex.CurrentRecordIndex == 1 && ex.CurrentFieldIndex == 1 && ex.CurrentPosition == 29)
					throw ex;
			}
		}

		[Test()]
		public void MoreFieldsTest()
		{
			const string Data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

			using (CsvReader csv = new CsvReader(new System.IO.StringReader(Data), false))
			{
				csv.SupportsMultiline = false;

				while (csv.ReadNextRecord())
				{
					for (int i = 0; i < csv.FieldCount; i++)
					{
						string s = csv[i];
					}
				}
			}
		}

		[Test()]
		public void MoreFieldsMultilineTest()
		{
			const string Data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

			using (CsvReader csv = new CsvReader(new System.IO.StringReader(Data), false))
			{
				while (csv.ReadNextRecord())
				{
					for (int i = 0; i < csv.FieldCount; i++)
					{
						string s = csv[i];
					}
				}
			}
		}

		[Test]
		public void ParseErrorBeforeInitializeTest()
		{
			const string Data = "\"0022 - SKABELON\";\"\"Tandremstrammer\";\"\";\"0,00\";\"\"\n\"15907\";\"\"BOLT TIL 2-05-405\";\"\";\"42,50\";\"4027816159070\"\n\"19324\";\"FJEDER TIL 2-05-405\";\"\";\"14,50\";\"4027816193241\"";

			using (var csv = new CsvReader(new System.IO.StringReader(Data), false, ';'))
			{
				csv.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;

				Assert.IsTrue(csv.ReadNextRecord());

				Assert.AreEqual("19324", csv[0]);
				Assert.AreEqual("FJEDER TIL 2-05-405", csv[1]);
				Assert.AreEqual("", csv[2]);
				Assert.AreEqual("14,50", csv[3]);
				Assert.AreEqual("4027816193241", csv[4]);

				Assert.IsFalse(csv.ReadNextRecord());
			}
		}

		[Test]
		public void LastFieldEmptyFollowedByMissingFieldsOnNextRecord()
		{
			const string Data = "a,b,c,d,e"
				+ "\na,b,c,d,"
				+ "\na,b,";

			using (var csv = new CsvReader(new StringReader(Data), false))
			{
				csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

				var record = new string[5];

				Assert.IsTrue(csv.ReadNextRecord());
				csv.CopyCurrentRecordTo(record);
				CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "e" }, record);

				Assert.IsTrue(csv.ReadNextRecord());
				csv.CopyCurrentRecordTo(record);
				CollectionAssert.AreEqual(new string[] { "a", "b", "c", "d", "" }, record);

				Assert.IsTrue(csv.ReadNextRecord());
				csv.CopyCurrentRecordTo(record);
				CollectionAssert.AreEqual(new string[] { "a", "b", "", null, null }, record);

				Assert.IsFalse(csv.ReadNextRecord());
			}
		}
	}
}