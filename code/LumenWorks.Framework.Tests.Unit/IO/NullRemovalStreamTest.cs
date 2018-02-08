using System;
using System.IO;
using System.Text;

using LumenWorks.Framework.IO;

using NUnit.Framework;

namespace LumenWorks.Framework.Tests.Unit.IO
{
    using System.Linq;

    public class NullRemovalStreamTest
    {
        [Test]
        [TestCase(true, "[removed {0} null bytes]", 50000)]
        [TestCase(false, "", 50000)]
        public void TestInputContainsOnlyNull(bool addMark, string template, int size)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = string.Format(template, size);

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 20000, 100)]
        [TestCase(false, "", 20000, 100)]
        public void TestInputWithNullPrefix(bool addMark, string template, int size, int numberOfNull)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = string.Format(template, numberOfNull) + new string('a', size - numberOfNull);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 10000, 400)]
        [TestCase(false, "", 10000, 400)]
        public void TestInputWithNullSuffix(bool addMark, string template, int size, int numberOfNonNull)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = new string('a', numberOfNonNull) + string.Format(template, size - numberOfNonNull);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 30000, 300)]
        [TestCase(false, "", 30000, 300)]
        public void TestInputWithNullInTheMiddle(bool addMark, string template, int size, int numberOfNull)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = new string('a', numberOfNull) + string.Format(template, numberOfNull) + new string('a', size - numberOfNull * 2);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, "[removed {0} null bytes]", 40000, 10000)]
        [TestCase(false, "", 40000, 10000)]
        public void TestInputWithNullAtBothEnds(bool addMark, string template, int size, int numberOfNull)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = string.Format(template, numberOfNull) + new string('a', numberOfNull * 2) + string.Format(template, numberOfNull);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 200, 59)]
        [TestCase(false, 200, 59)]
        public void TestInputWithFewerThanThresholdNullBytes(bool addMark, int size, int numberOfNull)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = new string(new char[numberOfNull]) + new string('a', size - numberOfNull);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 200, 60, "[removed {0} null bytes]")]
        [TestCase(false, 200, 60, "")]
        public void TestInputWithExactThresholdNullBytes(bool addMark, int size, int numberOfNull, string template)
        {
            var input = new byte[size];
            var buffer = new byte[4096];
            var expected = (addMark ? string.Format(template, numberOfNull) : "") + new string('a', size - numberOfNull);

            for (var i = 0; i < size; i++)
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

            var result = new StringBuilder();
            var total = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 8199, 60, "[removed {0} null bytes]")]
        [TestCase(false, 8199, 60, "")]
        public void TestInputWithNullBytesAboveThresholdFromLastInBuffer(bool addMark, int size, int numberOfNull, string template)
        {
            var input    = new byte[size];
            var buffer   = new byte[4096];
            var expected = string.Empty;
            if (addMark)
            {
                expected = new string('a', 4095) + string.Format(template, numberOfNull) + new string('a', size - 4095 - numberOfNull);
            }
            else
            {
                expected = new string('a', size - numberOfNull);
            }

            for (var i = 0; i < size; i++)
            {
                if (i >= 4095 && i < 4095 + numberOfNull)
                {
                    input[i] = 0;
                }
                else
                {
                    input[i] = Convert.ToByte('a');
                }
            }

            var result = new StringBuilder();
            var total  = ReadFromNullRemovalStream(input, buffer, result, addMark);
            Assert.AreEqual(expected.Length, total);
            Assert.AreEqual(expected, result.ToString());
        }

        [Test]
        [TestCase(true, 0)]
        [TestCase(true, 1)]
        [TestCase(true, 10)]
        [TestCase(true, 30)]
        [TestCase(true, 70)]
        [TestCase(true, 200)]
        [TestCase(true, 1000)]
        [TestCase(true, 4097)]
        [TestCase(true, 20000)]
        [TestCase(false, 0)]
        [TestCase(false, 1)]
        [TestCase(false, 10)]
        [TestCase(false, 30)]
        [TestCase(false, 70)]
        [TestCase(false, 200)]
        [TestCase(false, 1000)]
        [TestCase(false, 4097)]
        [TestCase(false, 20000)]
        public void TestInputWithRandomNullBytes(bool addMark, int maxNull)
        {
            var rand          = new Random();
            var size          = 4096 * 4096;
            var inputIndex    = 0;
            var input         = new byte[size];
            var expectedIndex = 0;
            var expected      = new byte[size];
            var totalNull     = 0;
            var totalRead     = 0;
            var result        = new StringBuilder();
            var markTemplate  = "[removed {0} null bytes]";

            while (inputIndex < size)
            {
                int numberOfNull = maxNull < 2 ? maxNull : rand.Next(1, Math.Min(maxNull, size - inputIndex));
                for (var i = inputIndex; i < inputIndex + numberOfNull; i++)
                {
                    input[i] = 0;
                    if (numberOfNull < 60)
                    {
                        expected[expectedIndex++] = 0;
                    }
                }
                if (numberOfNull >= 60 && addMark)
                {
                    foreach (var c in string.Format(markTemplate, numberOfNull))
                    {
                        expected[expectedIndex++] = (byte)c;
                    }
                }

                inputIndex += numberOfNull;
                totalNull  += numberOfNull;
                if (inputIndex < size)
                {
                    var numberOfChar = rand.Next(1, Math.Min(10, size - inputIndex));
                    for (var i = inputIndex; i < inputIndex + numberOfChar; i++)
                    {
                        input[i]                  = (byte)'a';
                        expected[expectedIndex++] = (byte)'a';
                    }
                    inputIndex += numberOfChar;
                }
            }

            using (var nullRemovalStream = new NullRemovalStream(new MemoryStream(input), addMark))
            {
                var buffer = new byte[4096];
                var readCount = nullRemovalStream.Read(buffer, 0, 4096);
                while (readCount > 0)
                {
                    totalRead += readCount;
                    result.Append(Encoding.ASCII.GetString(buffer, 0, readCount));
                    readCount = nullRemovalStream.Read(buffer, 0, 4096);
                }
            }

            //File.WriteAllBytes($@"{Path.GetTempPath()}/1-input-{addMark}.txt",input);
            //File.WriteAllBytes($@"{Path.GetTempPath()}/2-expected-{addMark}.txt",expected);
            //File.WriteAllText($@"{Path.GetTempPath()}/3-result-{addMark}.txt",result.ToString());

            // if the number of consecutive null bytes >= threshold, shrinking is guaranteed
            Assert.True(maxNull < 60 ? totalRead == size : totalRead < size);
            Assert.True(totalRead == expectedIndex);
            var totalNumberOfA = input.Count(c => c == 'a');
            Assert.AreEqual(size - totalNull, totalNumberOfA, "(number of a) and (size - number of null) should be equal!");
            var expectedCount = expected.Count(c => c == 'a');
            Assert.AreEqual(totalNumberOfA, expectedCount, "Expected buffer should contain same number of a!");
            var processedCount = result.ToString().Count(c => c == 'a');
            Assert.AreEqual(totalNumberOfA, processedCount, "Result sbuilder should contain same number of a!");
            Assert.AreEqual(Encoding.ASCII.GetString(expected, 0, expectedIndex), result.ToString());
        }

        private int ReadFromNullRemovalStream(byte[] input, byte[] buffer, StringBuilder sb, bool addMark)
        {
            using (var memoryStream = new MemoryStream(input))
            using (var nullRemovalStream = new NullRemovalStream(memoryStream, addMark))
            {
                var readCount = nullRemovalStream.Read(buffer, 0, 4096);
                var total = readCount;
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