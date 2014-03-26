namespace LumenWorks.Framework.Tests.Unit.IO.Csv
{
    using System;

    using LumenWorks.Framework.IO.Csv;

    using NUnit.Framework;

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