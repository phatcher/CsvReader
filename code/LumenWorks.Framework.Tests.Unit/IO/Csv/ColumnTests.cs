using System;
using System.Collections.Generic;

using LumenWorks.Framework.IO.Csv;

using NUnit.Framework;

namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    [TestFixture]
    public class ColumnTests
    {
        [Test]
        public void ConvertGuid()
        {
            var expected = Guid.NewGuid();
            var column = new Column { Name = "A", Type = typeof(Guid) };
            var candidate = column.Convert(expected.ToString());
            Assert.IsInstanceOf<Guid>(candidate);
            Assert.That(expected == (Guid)candidate);
        }

        [Test]
        public void ConvertByteArray()
        {
            var expected = new byte[] { 1, 2, 3 };
            var column = new Column { Name = "A", Type = typeof(byte[]) };
            var candidate = column.Convert(System.Convert.ToBase64String(expected));
            Assert.IsInstanceOf<byte[]>(candidate);
            Assert.That(ArraysEqual(expected, (byte[])candidate));
        }

        private static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        [Test]
        public void ConvertBoolean()
        {
            var column = new Column { Name = "A", Type = typeof(bool) };
            var result = column.Convert("true");
            Assert.IsInstanceOf<bool>(result);
            Assert.That((bool)result == true);
        }

        [Test]
        public void ConvertBooleanInt()
        {
            var column = new Column { Name = "A", Type = typeof(bool) };
            var result = column.Convert("0");
            Assert.IsInstanceOf<bool>(result);
            Assert.That((bool)result == false);
        }

        [Test]
        public void ConvertBooleanFail()
        {
            var column = new Column { Name = "A", Type = typeof(bool) };
            var result = column.Convert("fred");
            Assert.IsInstanceOf<bool>(result);
            Assert.That((bool)result == false);
        }

        [Test]
        public void ConvertInt32()
        {
            var column = new Column { Name = "A", Type = typeof(int) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<int>(result);
            Assert.That((int)result == 1);
        }

        [Test]
        public void ConvertInt32Fail()
        {
            var column = new Column { Name = "A", Type = typeof(int) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<int>(result);
            Assert.That((int)result == 0);
        }

        [Test]
        public void ConvertInt64()
        {
            var column = new Column { Name = "A", Type = typeof(long) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<long>(result);
            Assert.That((long)result == 1);
        }

        [Test]
        public void ConvertInt64Fail()
        {
            var column = new Column { Name = "A", Type = typeof(long) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<long>(result);
            Assert.That((long)result == 0);
        }

        [Test]
        public void ConvertSingle()
        {
            var column = new Column { Name = "A", Type = typeof(float) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<float>(result);
            Assert.That((float)result == 1);
        }

        [Test]
        public void ConvertSingleFail()
        {
            var column = new Column { Name = "A", Type = typeof(float) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<float>(result);
            Assert.That((float)result == 0);
        }

        [Test]
        public void ConvertDouble()
        {
            var column = new Column { Name = "A", Type = typeof(double) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<double>(result);
            Assert.That((double)result == 1);
        }

        [Test]
        public void ConvertDoubleFail()
        {
            var column = new Column { Name = "A", Type = typeof(double) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<double>(result);
            Assert.That((double)result == 0);
        }

        [Test]
        public void ConvertDecimal()
        {
            var column = new Column { Name = "A", Type = typeof(decimal) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<decimal>(result);
            Assert.That((decimal)result == 1);
        }

        [Test]
        public void ConvertDecimalFail()
        {
            var column = new Column { Name = "A", Type = typeof(decimal) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<decimal>(result);
            Assert.That((decimal)result == 0);
        }

        [Test]
        public void ConvertDateTime()
        {
            var expected = new DateTime(2013, 8, 14, 12, 35, 10);
            var column = new Column { Name = "A", Type = typeof(DateTime) };
            var result = column.Convert(expected.ToString("s"));
            Assert.IsInstanceOf<DateTime>(result);
            Assert.That((DateTime)result == expected);
        }

        [Test]
        public void ConvertDateTimeFail()
        {
            var expected = DateTime.MinValue;
            var column = new Column { Name = "A", Type = typeof(DateTime) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<DateTime>(result);
            Assert.That((DateTime)result == expected);
        }
    }
}