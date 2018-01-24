// LumenWorks.Framework.Tests.Unit.IO.CSV.CsvReaderWithNullRmovalTest
// Copyright (c) 2005 Sébastien Lorion
//
// MIT license (http://en.wikipedia.org/wiki/MIT_License)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


// A special thanks goes to "shriop" at CodeProject for providing many of the standard and Unicode parsing tests.

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using NUnit.Framework;

    using Framework.IO.Csv;

    [TestFixture()]
    public class CsvReaderWithNullRemovalTest
    {
        #region Argument validation tests

        #region Constructors

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestCtor1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(null, false, Encoding.ASCII))
            {
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestCtor2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, 0))
            {
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestCtor3WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, -1))
            {
            }
        }

        [Test]
        public void ArgumentTestCtor4WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("hello world!")), false, Encoding.ASCII, 123))
            {
                Assert.AreEqual("hello world!".Length, csv.BufferSize);
            }
        }

        #endregion

        #region Indexers

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestIndexer1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[-1];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestIndexer2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[CsvReaderSampleData.SampleData1RecordCount];
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ArgumentTestIndexer3WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv["asdf"];
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ArgumentTestIndexer4WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[CsvReaderSampleData.SampleData1Header0];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestIndexer5WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[null];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestIndexer6WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[string.Empty];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestIndexer7WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string s = csv[null];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestIndexer8WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string s = csv[string.Empty];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentTestIndexer9WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string s = csv["asdf"];
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestIndexer10WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string s = csv[-1, 0];
            }
        }

        #endregion

        #region CopyCurrentRecordTo

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentTestCopyCurrentRecordTo1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.CopyCurrentRecordTo(null);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestCopyCurrentRecordTo2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.CopyCurrentRecordTo(new string[1], -1);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ArgumentTestCopyCurrentRecordTo3WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.CopyCurrentRecordTo(new string[1], 1);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentTestCopyCurrentRecordTo4WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.ReadNextRecord();
                csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount - 1], 0);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentTestCopyCurrentRecordTo5WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.ReadNextRecord();
                csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount], 1);
            }
        }

        #endregion

        #endregion

        #region Parsing tests

        [Test]
        public void ParsingTest1WithNullRemovalStreamReader()
        {
            const string data = "1\r\n\r\n1";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest2WithNullRemovalStreamReader()
        {
            // ["Bob said, ""Hey!""",2, 3 ]
            const string data = "\"Bob said, \"\"Hey!\"\"\",2, 3 ";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '"', '"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(@"Bob said, ""Hey!""", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(" 3 ", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest3WithNullRemovalStreamReader()
        {
            const string data = "1\r2\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest4WithNullRemovalStreamReader()
        {
            const string data = "\"\n\r\n\n\r\r\",,\t,\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual(4, csv.FieldCount);

                Assert.AreEqual("\n\r\n\n\r\r", csv[0]);
                Assert.AreEqual("", csv[1]);
                Assert.AreEqual("", csv[2]);
                Assert.AreEqual("", csv[3]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest5WithNullRemovalStreamReader()
        {
            Checkdata5(1024);

            // some tricky ones ...

            Checkdata5(1);
            Checkdata5(9);
            Checkdata5(14);
            Checkdata5(39);
            Checkdata5(166);
            Checkdata5(194);
        }

        [Test]
        public void ParsingTest5_RandomBufferSizesWithNullRemovalStreamReader()
        {
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
                Checkdata5(random.Next(1, 512));
        }

        private void Checkdata5(int bufferSize)
        {
            const string data = CsvReaderSampleData.SampleData1;

            try
            {
                using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), true, Encoding.ASCII, bufferSize))
                {
                    CsvReaderSampleData.CheckSampleData1(csv, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("BufferSize={0}", bufferSize), ex);
            }
        }

        [Test]
        public void ParsingTest6WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("1,2")), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest7WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\r\n1\r\n")), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.AreEqual("1", csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest8WithNullRemovalStreamReader()
        {
            const string data = "\"bob said, \"\"Hey!\"\"\",2, 3 ";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("bob said, \"Hey!\"", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(3, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest9WithNullRemovalStreamReader()
        {
            const string data = ",";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest10WithNullRemovalStreamReader()
        {
            const string data = "1\r2";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest11WithNullRemovalStreamReader()
        {
            const string data = "1\n2";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest12WithNullRemovalStreamReader()
        {
            const string data = "1\r\n2";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest13WithNullRemovalStreamReader()
        {
            const string data = "1\r";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest14WithNullRemovalStreamReader()
        {
            const string data = "1\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest15WithNullRemovalStreamReader()
        {
            const string data = "1\r\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest16WithNullRemovalStreamReader()
        {
            const string data = "1\r2\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, '\r', '"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest17WithNullRemovalStreamReader()
        {
            const string data = "\"July 4th, 2005\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("July 4th, 2005", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest18WithNullRemovalStreamReader()
        {
            const string data = " 1";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(" 1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest19WithNullRemovalStreamReader()
        {
            string data = string.Empty;

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest20WithNullRemovalStreamReader()
        {
            const string data = "user_id,name\r\n1,Bruce";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), true, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("Bruce", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.AreEqual("1", csv["user_id"]);
                Assert.AreEqual("Bruce", csv["name"]);
                Assert.IsFalse(csv.ReadNextRecord());
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest21WithNullRemovalStreamReader()
        {
            const string data = "\"data \r\n here\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("data \r\n here", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest22WithNullRemovalStreamReader()
        {
            const string data = "\r\r\n1\r";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, '\r', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(3, csv.FieldCount);

                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(string.Empty, csv[2]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest23WithNullRemovalStreamReader()
        {
            const string data = "\"double\"\"\"\"double quotes\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\"\"double quotes", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest24WithNullRemovalStreamReader()
        {
            const string data = "1\r";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest25WithNullRemovalStreamReader()
        {
            const string data = "1\r\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest26WithNullRemovalStreamReader()
        {
            const string data = "1\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest27WithNullRemovalStreamReader()
        {
            const string data = "'bob said, ''Hey!''',2, 3 ";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\'', '\'', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("bob said, 'Hey!'", csv[0]);
                Assert.AreEqual("2", csv[1]);
                Assert.AreEqual("3", csv[2]);
                Assert.AreEqual(',', csv.Delimiter);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(3, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest28WithNullRemovalStreamReader()
        {
            const string data = "\"data \"\" here\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\0', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("\"data \"\" here\"", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest29WithNullRemovalStreamReader()
        {
            string data = new string('a', 75) + "," + new string('b', 75);

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(new string('a', 75), csv[0]);
                Assert.AreEqual(new string('b', 75), csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest30WithNullRemovalStreamReader()
        {
            const string data = "1\r\n\r\n1";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest31WithNullRemovalStreamReader()
        {
            const string data = "1\r\n# bunch of crazy stuff here\r\n1";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest32WithNullRemovalStreamReader()
        {
            const string data = "\"1\",Bruce\r\n\"2\n\",Toni\r\n\"3\",Brian\r\n";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("1", csv[0]);
                Assert.AreEqual("Bruce", csv[1]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("2\n", csv[0]);
                Assert.AreEqual("Toni", csv[1]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("3", csv[0]);
                Assert.AreEqual("Brian", csv[1]);
                Assert.AreEqual(2, csv.CurrentRecordIndex);
                Assert.AreEqual(2, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest33WithNullRemovalStreamReader()
        {
            const string data = "\"double\\\\\\\\double backslash\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\\', '#', ValueTrimmingOptions.None))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("double\\\\double backslash", csv[0]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(1, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest34WithNullRemovalStreamReader()
        {
            const string data = "\"Chicane\", \"Love on the Run\", \"Knight Rider\", \"This field contains a comma, but it doesn't matter as the field is quoted\"\r\n" +
                      "\"Samuel Barber\", \"Adagio for Strings\", \"Classical\", \"This field contains a double quote character, \"\", but it doesn't matter as it is escaped\"";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, ',', '\"', '\"', '#', ValueTrimmingOptions.UnquotedOnly))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("Chicane", csv[0]);
                Assert.AreEqual("Love on the Run", csv[1]);
                Assert.AreEqual("Knight Rider", csv[2]);
                Assert.AreEqual("This field contains a comma, but it doesn't matter as the field is quoted", csv[3]);
                Assert.AreEqual(0, csv.CurrentRecordIndex);
                Assert.AreEqual(4, csv.FieldCount);
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("Samuel Barber", csv[0]);
                Assert.AreEqual("Adagio for Strings", csv[1]);
                Assert.AreEqual("Classical", csv[2]);
                Assert.AreEqual("This field contains a double quote character, \", but it doesn't matter as it is escaped", csv[3]);
                Assert.AreEqual(1, csv.CurrentRecordIndex);
                Assert.AreEqual(4, csv.FieldCount);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest35WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\t")), false, Encoding.ASCII, '\t'))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());

                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest36WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest37WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        [Test]
        public void ParsingTest38WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("abc,def,ghi\n")), false, Encoding.ASCII))
            {
                int fieldCount = csv.FieldCount;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("abc", csv[0]);
                Assert.AreEqual("def", csv[1]);
                Assert.AreEqual("ghi", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest39WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00,01,   \n10,11,   ")), false, Encoding.ASCII, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, ValueTrimmingOptions.UnquotedOnly, 1))
            {
                int fieldCount = csv.FieldCount;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);
                Assert.AreEqual("01", csv[1]);
                Assert.AreEqual("", csv[2]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);
                Assert.AreEqual("11", csv[1]);
                Assert.AreEqual("", csv[2]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest40WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("\"00\",\n\"10\",")), false, Encoding.ASCII))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);
                Assert.AreEqual(string.Empty, csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest41WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("First record          ,Second record")), false, Encoding.ASCII, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, ValueTrimmingOptions.UnquotedOnly, 16))
            {
                Assert.AreEqual(2, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("First record", csv[0]);
                Assert.AreEqual("Second record", csv[1]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest42WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(" ")), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(1, csv.FieldCount);
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void ParsingTest43WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("a,b\n   ")), false, Encoding.ASCII))
            {
                csv.SkipEmptyLines = true;
                csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(2, csv.FieldCount);
                Assert.AreEqual("a", csv[0]);
                Assert.AreEqual("b", csv[1]);

                csv.ReadNextRecord();
                Assert.AreEqual(string.Empty, csv[0]);
                Assert.AreEqual(null, csv[1]);
            }
        }

        [ExpectedException(typeof(MalformedCsvException))]
        [Test]
        public void ParsingTest44WithNullRemovalStreamReader()
        {
            const string data = "\"01234567891\"\r\ntest";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                csv.MaxQuotedFieldLength = 10;
                csv.ReadNextRecord();
            }
        }

        [Test]
        public void ParsingTest45WithNullRemovalStreamReader()
        {
            const string data = "\"01234567891\"\r\ntest";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII))
            {
                csv.MaxQuotedFieldLength = 11;
                csv.ReadNextRecord();
                Assert.AreEqual("01234567891", csv[0]);
            }
        }

        #endregion

        #region UnicodeParsing tests

        [Test]
        public void UnicodeParsingTest1WithNullRemovalStreamReader()
        {
            // control characters and comma are skipped

            char[] raw = new char[65536 - 13];

            for (int i = 0; i < raw.Length; i++)
                raw[i] = (char)(i + 14);

            raw[44 - 14] = ' '; // skip comma

            string data = new string(raw);

            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            string dataBack = Encoding.Unicode.GetString(dataBytes);

            using (CsvReader csv = new CsvReader(new MemoryStream(dataBytes), false, Encoding.Unicode))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(dataBack, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest2WithNullRemovalStreamReader()
        {
            byte[] buffer;

            string test = "München";

            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    writer.WriteLine(test);
                }

                buffer = stream.ToArray();
            }

            using (CsvReader csv = new CsvReader(new MemoryStream(buffer), false, Encoding.UTF8))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void UnicodeParsingTest3WithNullRemovalStreamReader()
        {
            byte[] buffer;

            string test = "München";

            using (MemoryStream stream = new MemoryStream())
            {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF32))
                {
                    writer.Write(test);
                }

                buffer = stream.ToArray();
            }

            using (CsvReader csv = new CsvReader(new MemoryStream(buffer), false, Encoding.UTF32))
            {
                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(test, csv[0]);
                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        #endregion

        #region FieldCount

        [Test]
        public void FieldCountTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                CsvReaderSampleData.CheckSampleData1(csv, true);
            }
        }

        #endregion

        #region GetFieldHeaders

        [Test]
        public void GetFieldHeadersTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                string[] headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [Test]
        public void GetFieldHeadersTest2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string[] headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(CsvReaderSampleData.SampleData1RecordCount, headers.Length);

                Assert.AreEqual(CsvReaderSampleData.SampleData1Header0, headers[0]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header1, headers[1]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header2, headers[2]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header3, headers[3]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header4, headers[4]);
                Assert.AreEqual(CsvReaderSampleData.SampleData1Header5, headers[5]);

                Assert.AreEqual(0, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header0));
                Assert.AreEqual(1, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header1));
                Assert.AreEqual(2, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header2));
                Assert.AreEqual(3, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header3));
                Assert.AreEqual(4, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header4));
                Assert.AreEqual(5, csv.GetFieldIndex(CsvReaderSampleData.SampleData1Header5));
            }
        }

        [Test]
        public void GetFieldHeadersTest_EmptyCsvWithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("#asdf\n\n#asdf,asdf")), true, Encoding.ASCII))
            {
                string[] headers = csv.GetFieldHeaders();

                Assert.IsNotNull(headers);
                Assert.AreEqual(0, headers.Length);
            }
        }

        [TestCase((string)null)]
        [TestCase("")]
        [TestCase("AnotherName")]
        public void GetFieldHeaders_WithEmptyHeaderNamesWithNullRemovalStreamReader(string defaultHeaderName)
        {
            if (defaultHeaderName == null)
                defaultHeaderName = "Column";

            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(",  ,,aaa,\"   \",,,")), true, Encoding.ASCII))
            {
                csv.DefaultHeaderName = defaultHeaderName;

                Assert.IsFalse(csv.ReadNextRecord());
                Assert.AreEqual(8, csv.FieldCount);

                string[] headers = csv.GetFieldHeaders();
                Assert.AreEqual(csv.FieldCount, headers.Length);

                Assert.AreEqual("aaa", headers[3]);
                foreach (int index in new[] { 0, 1, 2, 4, 5, 6, 7 })
                    Assert.AreEqual(defaultHeaderName + index, headers[index]);
            }
        }

        #endregion

        [Test]
        public void CachedNoHeaderWithNullRemovalStreamReader()
        {
            CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("12345678;Hello\r\n78945612;World")), false, Encoding.ASCII, ';');
            DataGridView dgv = new DataGridView { DataSource = csv };
            dgv.Refresh();
        }

        #region HasHeader

        [Test]
        public void HasHeader_NullHeaderWithNullRemovalStreamReader()
        {
            using(CsvReader csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("Header1,Header2\r\nValue1,Value2")), true, Encoding.ASCII))
            {
                Assert.Throws<ArgumentNullException>(delegate
                {
                    csvReader.HasHeader(null);
                });
            }
        }

        [Test]
        public void HasHeader_HeaderExistsWithNullRemovalStreamReader()
        {
            string header = "First Name";

            using (CsvReader csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsTrue(csvReader.HasHeader(header)); 
            }
        }

        [Test]
        public void HasHeader_HeaderDoesNotExistWithNullRemovalStreamReader()
        {
            string header = "Phone Number";

            using (CsvReader csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        [Test]
        public void HasHeader_NullFieldHeadersWithNullRemovalStreamReader()
        {
            string header = "NonExistingHeader";

            using (CsvReader csvReader = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("Value1,Value2")), false, Encoding.ASCII))
            {
                Assert.IsFalse(csvReader.HasHeader(header));
            }
        }

        #endregion

        #region CopyCurrentRecordTo

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CopyCurrentRecordToTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                csv.CopyCurrentRecordTo(new string[CsvReaderSampleData.SampleData1RecordCount]);
            }
        }

        #endregion

        #region MoveTo tests

        [Test]
        public void MoveToTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                for (int i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    Assert.IsTrue(csv.MoveTo(i));
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        public void MoveToTest2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsTrue(csv.MoveTo(1));
                Assert.IsFalse(csv.MoveTo(0));
            }
        }

        [Test]
        public void MoveToTest3WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                Assert.IsFalse(csv.MoveTo(CsvReaderSampleData.SampleData1RecordCount));
            }
        }

        [Test]
        public void MoveToTest4WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                csv.SupportsMultiline = false;

                string[] headers = csv.GetFieldHeaders();

                Assert.IsTrue(csv.MoveTo(2));
                Assert.AreEqual(2, csv.CurrentRecordIndex);
                CsvReaderSampleData.CheckSampleData1(csv, false);
            }
        }

        [Test]
        public void MoveToTest5WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), false, Encoding.ASCII))
            {
                Assert.IsTrue(csv.MoveTo(-1));
                csv.MoveTo(0);
                Assert.IsFalse(csv.MoveTo(-1));
            }
        }

        #endregion

        #region Iteration tests

        [Test]
        public void IterationTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                int index = 0;

                foreach (string[] record in csv)
                {
                    CsvReaderSampleData.CheckSampleData1(csv.HasHeaders, index, record);
                    index++;
                }
            }
        }

        [Test]
        public void IterationTest2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string[] previous = null;

                foreach (string[] record in csv)
                {
                    Assert.IsFalse(ReferenceEquals(previous, record));

                    previous = record;
                }
            }
        }

        #endregion

        #region Indexer tests

        [Test]
        public void IndexerTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                for (int i = 0; i < CsvReaderSampleData.SampleData1RecordCount; i++)
                {
                    string s = csv[i, 0];
                    CsvReaderSampleData.CheckSampleData1(i, csv);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IndexerTest2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string s = csv[1, 0];
                s = csv[0, 0];
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IndexerTest3WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(CsvReaderSampleData.SampleData1)), true, Encoding.ASCII))
            {
                string s = csv[CsvReaderSampleData.SampleData1RecordCount, 0];
            }
        }

        #endregion

        #region SkipEmptyLines

        [Test]
        public void SkipEmptyLinesTest1WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00\n\n10")), false, Encoding.ASCII))
            {
                csv.SkipEmptyLines = false;

                Assert.AreEqual(1, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual(string.Empty, csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        [Test]
        public void SkipEmptyLinesTest2WithNullRemovalStreamReader()
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes("00\n\n10")), false, Encoding.ASCII))
            {
                csv.SkipEmptyLines = true;

                Assert.AreEqual(1, csv.FieldCount);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("00", csv[0]);

                Assert.IsTrue(csv.ReadNextRecord());
                Assert.AreEqual("10", csv[0]);

                Assert.IsFalse(csv.ReadNextRecord());
            }
        }

        #endregion

        #region Trimming tests

        [TestCase("", ValueTrimmingOptions.None, new string[] { })]
        [TestCase("", ValueTrimmingOptions.QuotedOnly, new string[] { })]
        [TestCase("", ValueTrimmingOptions.UnquotedOnly, new string[] { })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.QuotedOnly, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb , ccc ", ValueTrimmingOptions.UnquotedOnly, new[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.QuotedOnly, new[] { "aaa", "bbb", "ccc" })]
        [TestCase("\" aaa \",\" bbb \",\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.None, new[] { " aaa ", " bbb ", " ccc " })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.QuotedOnly, new[] { " aaa ", " bbb ", "ccc" })]
        [TestCase(" aaa , bbb ,\" ccc \"", ValueTrimmingOptions.UnquotedOnly, new[] { "aaa", "bbb", " ccc " })]
        public void TrimFieldValuesTestWithNullRemovalStreamReader(string data, ValueTrimmingOptions trimmingOptions, params string[] expected)
        {
            using (CsvReader csv = new CsvReader(new MemoryStream(Encoding.ASCII.GetBytes(data)), false, Encoding.ASCII, CsvReader.DefaultDelimiter, CsvReader.DefaultQuote, CsvReader.DefaultEscape, CsvReader.DefaultComment, trimmingOptions))
            {
                while (csv.ReadNextRecord())
                {
                    string[] actual = new string[csv.FieldCount];
                    csv.CopyCurrentRecordTo(actual);

                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        #endregion
    }
}