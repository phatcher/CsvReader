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
            var expected = new Byte[] { 1, 2, 3 };
            var column = new Column { Name = "A", Type = typeof(Byte[]) };
            var candidate = column.Convert(System.Convert.ToBase64String(expected));
            Assert.IsInstanceOf<Byte[]>(candidate);
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
            var column = new Column { Name = "A", Type = typeof(Boolean) };
            var result = column.Convert("true");
            Assert.IsInstanceOf<Boolean>(result);
            Assert.That((Boolean)result == true);
        }

        [Test]
        public void ConvertBooleanInt()
        {
            var column = new Column { Name = "A", Type = typeof(Boolean) };
            var result = column.Convert("0");
            Assert.IsInstanceOf<Boolean>(result);
            Assert.That((Boolean)result == false);
        }

        [Test]
        public void ConvertBooleanFail()
        {
            var column = new Column { Name = "A", Type = typeof(Boolean) };
            var result = column.Convert("fred");
            Assert.IsInstanceOf<Boolean>(result);
            Assert.That((Boolean)result == false);
        }

        [Test]
        public void ConvertInt32()
        {
            var column = new Column { Name = "A", Type = typeof(Int32) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<Int32>(result);
            Assert.That((Int32)result == 1);
        }

        [Test]
        public void ConvertInt32Fail()
        {
            var column = new Column { Name = "A", Type = typeof(Int32) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<Int32>(result);
            Assert.That((Int32)result == 0);
        }

        [Test]
        public void ConvertInt64()
        {
            var column = new Column { Name = "A", Type = typeof(Int64) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<Int64>(result);
            Assert.That((Int64)result == 1);
        }

        [Test]
        public void ConvertInt64Fail()
        {
            var column = new Column { Name = "A", Type = typeof(Int64) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<Int64>(result);
            Assert.That((Int64)result == 0);
        }

        [Test]
        public void ConvertSingle()
        {
            var column = new Column { Name = "A", Type = typeof(Single) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<Single>(result);
            Assert.That((Single)result == 1);
        }

        [Test]
        public void ConvertSingleFail()
        {
            var column = new Column { Name = "A", Type = typeof(Single) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<Single>(result);
            Assert.That((Single)result == 0);
        }

        [Test]
        public void ConvertDouble()
        {
            var column = new Column { Name = "A", Type = typeof(Double) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<Double>(result);
            Assert.That((Double)result == 1);
        }

        [Test]
        public void ConvertDoubleFail()
        {
            var column = new Column { Name = "A", Type = typeof(Double) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<Double>(result);
            Assert.That((Double)result == 0);
        }

        [Test]
        public void ConvertDecimal()
        {
            var column = new Column { Name = "A", Type = typeof(Decimal) };
            var result = column.Convert("1");
            Assert.IsInstanceOf<Decimal>(result);
            Assert.That((Decimal)result == 1);
        }

        [Test]
        public void ConvertDecimalFail()
        {
            var column = new Column { Name = "A", Type = typeof(Decimal) };
            var result = column.Convert("A");
            Assert.IsInstanceOf<Decimal>(result);
            Assert.That((Decimal)result == 0);
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