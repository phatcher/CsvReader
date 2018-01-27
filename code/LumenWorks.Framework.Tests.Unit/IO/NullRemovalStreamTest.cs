namespace LumenWorks.Framework.Tests.Unit.IO
{
    using System;
    using System.IO;
    using System.Text;

    using Framework.IO;

    using NUnit.Framework;

    public class NullRemovalStreamTest
    {
        [Test]
        [TestCase(true, "[removed {0} null bytes]", 50000)]
        [TestCase(false, "", 50000)]
        public void TestInputContainsOnlyNull(bool addMark, string template, int size)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = string.Format(template, size);

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 20000, 100)]
        [TestCase(false, "", 20000, 100)]
        public void TestInputWithNullPrefix(bool addMark, string template, int size, int numberOfNull)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = string.Format(template, numberOfNull) + new string('a', size - numberOfNull);

            for (int i = 0; i < size; i++)
            {
                if (i >= numberOfNull)
                {
                    input[i] = Convert.ToByte('a');
                }
                else
                {
                    input[i] = 0;
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 10000, 400)]
        [TestCase(false, "", 10000, 400)]
        public void TestInputWithNullSuffix(bool addMark, string template, int size, int numberOfNonNull)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = new string('a', numberOfNonNull) + string.Format(template, size - numberOfNonNull);

            for (int i = 0; i < size; i++)
            {
                if (i < numberOfNonNull)
                {
                    input[i] = Convert.ToByte('a');
                }
                else
                {
                    input[i] = 0;
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 30000, 300)]
        [TestCase(false, "", 30000, 300)]
        public void TestInputWithNullInTheMiddle(bool addMark, string template, int size, int numberOfNull)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = new string('a', numberOfNull) + string.Format(template, numberOfNull) + new string('a', size - numberOfNull * 2);

            for (int i = 0; i < size; i++)
            {
                if (i < numberOfNull || i >= numberOfNull + numberOfNull)
                {
                    input[i] = Convert.ToByte('a');
                }
                else
                {
                    input[i] = 0;
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 40000, 10000)]
        [TestCase(false, "", 40000, 10000)]
        public void TestInputWithNullAtBothEnds(bool addMark, string template, int size, int numberOfNull)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = string.Format(template, numberOfNull) + new string('a', numberOfNull * 2) + string.Format(template, numberOfNull);

            for (int i = 0; i < size; i++)
            {
                if (i < numberOfNull || i >= numberOfNull * 3)
                {
                    input[i] = 0;
                }
                else
                {
                    input[i] = Convert.ToByte('a');
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 200, 59)]
        [TestCase(false, 200, 59)]
        public void TestInputWithFewerThanThresholdNullBytes(bool addMark, int size, int numberOfNull)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = new string(new char[numberOfNull]) + new string('a', size - numberOfNull);

            for (int i = 0; i < size; i++)
            {
                if (i >= numberOfNull)
                {
                    input[i] = Convert.ToByte('a');
                }
                else
                {
                    input[i] = 0;
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 200, 60, "[removed {0} null bytes]")]
        [TestCase(false, 200, 60, "")]
        public void TestInputWithExactThresholdNullBytes(bool addMark, int size, int numberOfNull, string template)
        {
            byte[] input = new byte[size];
            byte[] buffer = new byte[4096];
            string expected = (addMark ? string.Format(template, numberOfNull) : "") + new string('a', size - numberOfNull);

            for (int i = 0; i < size; i++)
            {
                if (i >= numberOfNull)
                {
                    input[i] = Convert.ToByte('a');
                }
                else
                {
                    input[i] = 0;
                }
            }

            StringBuilder result = new StringBuilder();
            int total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        private int ReadFromNullRemovalStream(byte[] input, byte[] buffer, StringBuilder sb, bool addMark)
        {
            using (MemoryStream memoryStream = new MemoryStream(input))
            using (NullRemovalStream nullRemovalStream = new NullRemovalStream(memoryStream, addMark))
            {
                int readCount = nullRemovalStream.Read(buffer, 0, 4096);
                int total = readCount;
                while (readCount != 0)
                {
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, readCount));
                    readCount = nullRemovalStream.Read(buffer, 0, 4096);
                    total += readCount;
                }
                return total;
            }
        }
    }
}
