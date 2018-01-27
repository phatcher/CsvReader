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
using System.IO;
using System.Text;

using NUnit.Framework;

using LumenWorks.Framework.IO.Csv;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    [TestFixture]
    public class CsvReaderMalformedTest
    {
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

            var capacity = recordCount * (fieldCount * 2 + fieldCount - 1) + recordCount;
            Assert.IsTrue(capacity <= int.MaxValue);

            var sb = new StringBuilder((int) capacity);
            var expectedErrorPosition = 0;

            for (long i = 0; i < recordCount; i++)
            {
                int realFieldCount;

                if (i == badRecordIndex)
                    realFieldCount = badFieldIndex;
                else
                    realFieldCount = fieldCount;

                for (var j = 0; j < realFieldCount; j++)
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

            using (var csv = new CsvReader(new StringReader(sb.ToString()), false, bufferSize))
            {
                csv.MissingFieldAction = action;
                Assert.AreEqual(fieldCount, csv.FieldCount);

                while (csv.ReadNextRecord())
                {
                    Assert.AreEqual(fieldCount, csv.FieldCount);

                    // if not sequential, directly test the missing field
                    if (!sequentialAccess)
                        CheckMissingFieldValueUnquoted(csv, badFieldIndex, badRecordIndex, badFieldIndex, expectedErrorPosition, sequentialAccess, action);

                    for (var i = 0; i < csv.FieldCount; i++)
                        CheckMissingFieldValueUnquoted(csv, i, badRecordIndex, badFieldIndex, expectedErrorPosition, sequentialAccess, action);
                }
            }
        }

        private void CheckMissingFieldValueUnquoted(CsvReader csv, int fieldIndex, long badRecordIndex, int badFieldIndex, int expectedErrorPosition, bool sequentialAccess, MissingFieldAction action)
        {
            const string message = "RecordIndex={0}; FieldIndex={1}; Position={2}; Sequential={3}; Action={4}";

            // make sure s contains garbage as to not have false successes
            var s = "asdfasdfasdf";

            try
            {
                s = csv[fieldIndex];
            }
            catch (MissingFieldCsvException ex)
            {
                Assert.AreEqual(badRecordIndex, ex.CurrentRecordIndex, message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);
                Assert.IsTrue(fieldIndex >= badFieldIndex, message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);
                Assert.AreEqual(expectedErrorPosition, ex.CurrentPosition, message, ex.CurrentRecordIndex, ex.CurrentFieldIndex, ex.CurrentPosition, sequentialAccess, action);

                return;
            }

            if (csv.CurrentRecordIndex != badRecordIndex || fieldIndex < badFieldIndex)
            {
                Assert.AreEqual(csv.CurrentRecordIndex.ToString() + fieldIndex.ToString(), s, message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
            }
            else
            {
                switch (action)
                {
                    case MissingFieldAction.ReplaceByEmpty:
                        Assert.AreEqual(string.Empty, s, message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
                        break;

                    case MissingFieldAction.ReplaceByNull:
                        Assert.IsNull(s, message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
                        break;

                    case MissingFieldAction.ParseError:
                        Assert.Fail("Failed to throw ParseError. - " + message, csv.CurrentRecordIndex, fieldIndex, -1, sequentialAccess, action);
                        break;

                    default:
                        Assert.Fail("'{0}' is not handled by this test.", action);
                        break;
                }
            }
        }

        [Test]
        public void MissingFieldUnquotedTest1()
        {
            CheckMissingFieldUnquoted(4, 4, 2, 2, CsvReader.DefaultBufferSize);
            CheckMissingFieldUnquoted(4, 4, 2, 2, CsvReader.DefaultBufferSize);
        }

        [Test]
        public void MissingFieldUnquotedTest2()
        {
            // With bufferSize = 16, faulty new line char is at the start of next buffer read
            CheckMissingFieldUnquoted(4, 4, 2, 3, 16);
        }

        [Test]
        public void MissingFieldUnquotedTest3()
        {
            // test missing field when end of buffer has been reached
            CheckMissingFieldUnquoted(3, 4, 2, 3, 16);
        }

        [Test]
        public void MissingFieldAllQuotedFields_Issue_12()
        {
            var sample =
                "\"A\",\"B\"\n" +
                "\"1\",\"2\"\n" +
                "\"3\"\n" +
                "\"5\",\"6\"";

            var buffer = new string[2];

            Assert.Throws<MissingFieldCsvException>(() =>
            {
                using (var csv = new CsvReader(new StringReader(sample), false))
                {
                    while (csv.ReadNextRecord())
                    {
                        csv.CopyCurrentRecordTo(buffer);
                    }
                }
            });
        }

        [Test]
        public void MissingFieldQuotedTest1()
        {
            const string data = "a,b,c,d\n1,1,1,1\n2,\"2\"\n3,3,3,3";

            var ep = ParseException<MissingFieldCsvException>(data);
            if (!(ep.CurrentRecordIndex == 2 && ep.CurrentFieldIndex == 2 && ep.CurrentPosition == 22))
            {
                throw ep;
            }
        }

        [Test]
        public void MissingFieldQuotedTest2()
        {
            const string data = "a,b,c,d\n1,1,1,1\n2,\"2\",\n3,3,3,3";

            // NOTE: Buffer size affects reported error position - surely should be based on source doc position?
            var ep = ParseException<MissingFieldCsvException>(data, 11);
            if (!(ep.CurrentRecordIndex == 2 && ep.CurrentFieldIndex == 2 && ep.CurrentPosition == 1))
                throw ep;
        }

        [Test]
        public void MissingFieldQuotedTest3()
        {
            const string data = "a,b,c,d\n1,1,1,1\n2,\"2\"\n\"3\",3,3,3";

            var ep = ParseException<MissingFieldCsvException>(data);
            if (!(ep.CurrentRecordIndex == 2 && ep.CurrentFieldIndex == 2 && ep.CurrentPosition == 22))
                throw ep;
        }

        [Test]
        public void MissingFieldQuotedTest4()
        {
            const string data = "a,b,c,d\n1,1,1,1\n2,\"2\",\n\"3\",3,3,3";

            // NOTE: Buffer size affects reported error position - surely should be based on source doc position?
            var ep = ParseException<MissingFieldCsvException>(data, 11);
            if (!(ep.CurrentRecordIndex == 2 && ep.CurrentFieldIndex == 2 && ep.CurrentPosition == 1))
            {
                throw ep;
            }
        }

        [Test]
        public void MissingDelimiterAfterQuotedFieldTest1()
        {
            const string data = "\"111\",\"222\"\"333\"";

            var ep = ParseExceptionTrimmingOptions<MalformedCsvException>(data);
            if (!(ep.CurrentRecordIndex == 0 && ep.CurrentFieldIndex == 1 && ep.CurrentPosition == 11))
            {
                throw ep;
            }
        }

        [Test]
        public void MissingDelimiterAfterQuotedFieldTest2()
        {
            const string data = "\"111\",\"222\",\"333\"\n\"111\",\"222\"\"333\"";

            var ep = ParseExceptionTrimmingOptions<MalformedCsvException>(data);
            if (!(ep.CurrentRecordIndex == 1 && ep.CurrentFieldIndex == 1 && ep.CurrentPosition == 29))
            {
                throw ep;
            }
        }

        [Test]
        public void MoreFieldsTest_AdvanceToNextLine()
        {
            const string data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                csv.SupportsMultiline = false;
                csv.DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine;

                while (csv.ReadNextRecord())
                {
                    for (var i = 0; i < csv.FieldCount; i++)
                    {
                        var s = csv[i];
                    }
                }
            }
        }

        [Test]
        public void MoreFieldsTest_RaiseEvent()
        {
            const string data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

            using (var csv = new CsvReader(new StringReader(data), false))
            {
                var sawError = false;
                csv.SupportsMultiline = false;
                csv.DefaultParseErrorAction = ParseErrorAction.RaiseEvent;
                csv.ParseError += (obj, args) => sawError = true;
                while (csv.ReadNextRecord())
                {
                    for (var i = 0; i < csv.FieldCount; i++)
                    {
                        var s = csv[i];
                    }
                }

                Assert.That(sawError, Is.True);
            }
        }

        [Test]
        public void MoreFieldsTest_ThrowsException()
        {
            const string data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

            Assert.Throws<MalformedCsvException>(() =>
            {
                using (var csv = new CsvReader(new System.IO.StringReader(data), false))
                {
                    csv.SupportsMultiline = false;
                    csv.DefaultParseErrorAction = ParseErrorAction.ThrowException;
                    while (csv.ReadNextRecord())
                    {
                        for (var i = 0; i < csv.FieldCount; i++)
                        {
                            var s = csv[i];
                        }
                    }
                }
            });
        }

        [Test]
        public void MoreFieldsMultilineTest()
        {
            const string data = "ORIGIN,DESTINATION\nPHL,FLL,kjhkj kjhkjh,eg,fhgf\nNYC,LAX";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false))
            {
                while (csv.ReadNextRecord())
                {
                    for (var i = 0; i < csv.FieldCount; i++)
                    {
                        var s = csv[i];
                    }
                }
            }
        }

        [Test]
        public void ParseErrorBeforeInitializeTest()
        {
            const string data = "\"0022 - SKABELON\";\"\"Tandremstrammer\";\"\";\"0,00\";\"\"\n\"15907\";\"\"BOLT TIL 2-05-405\";\"\";\"42,50\";\"4027816159070\"\n\"19324\";\"FJEDER TIL 2-05-405\";\"\";\"14,50\";\"4027816193241\"";

            using (var csv = new CsvReader(new System.IO.StringReader(data), false, ';'))
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
            const string data = "a,b,c,d,e"
                + "\na,b,c,d,"
                + "\na,b,";

            using (var csv = new CsvReader(new StringReader(data), false))
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

        private T ParseException<T>(string data)
            where T : Exception
        {
            var ep = Assert.Throws<T>(() =>
            {
                using (var csv = new CsvReader(new StringReader(data), false))
                {
                    while (csv.ReadNextRecord())
                    {
                        for (var i = 0; i < csv.FieldCount; i++)
                        {
                            var s = csv[i];
                        }
                    }
                }
            });

            return ep;
        }

        private T ParseException<T>(string data, int bufferSize)
            where T : Exception
        {
            var ep = Assert.Throws<T>(() =>
            {
                using (var csv = new CsvReader(new StringReader(data), false, bufferSize))
                {
                    while (csv.ReadNextRecord())
                    {
                        for (var i = 0; i < csv.FieldCount; i++)
                        {
                            var s = csv[i];
                        }
                    }
                }
            });

            return ep;
        }

        private T ParseExceptionTrimmingOptions<T>(string data, ValueTrimmingOptions options = ValueTrimmingOptions.UnquotedOnly)
            where T : Exception
        {
            var ep = Assert.Throws<T>(() =>
            {
                using (var csv = new CsvReader(new StringReader(data), false, ',', '"', '\\', '#', options))
                {
                    while (csv.ReadNextRecord())
                    {
                        for (var i = 0; i < csv.FieldCount; i++)
                        {
                            var s = csv[i];
                        }
                    }
                }
            });

            return ep;
        }
    }
}