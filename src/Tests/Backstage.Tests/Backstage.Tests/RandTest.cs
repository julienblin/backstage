namespace Backstage.Tests
{
    using System;
    using System.Drawing;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class RandTest
    {
        public enum TestEnum
        {
            Value1,

            Value2
        }

        [Test]
        public void It_should_generate_bool()
        {
            Rand.Bool();
        }

        [Test]
        public void It_should_generate_byte()
        {
            Rand.Byte().Should().NotBe(null);
            var buffer = new byte[20];
            Rand.Bytes(buffer);
        }

        [Test]
        public void It_should_generate_char()
        {
            Rand.Char().Should().NotBeNull();
        }

        [Test]
        public void It_should_generate_decimal()
        {
            Rand.Decimal().Should().NotBe(null);
            Rand.Decimal(25m).Should().BeLessThan(25m);
            Rand.Decimal(-100m, 50m).Should().BeInRange(-100m, 50m);
        }

        [Test]
        public void It_should_generate_double()
        {
            Rand.Double().Should().BeInRange(0.0, 1.0);
            Rand.Double(20.0).Should().BeLessThan(20.0);
            Rand.Double(-10.0, 20.0).Should().BeInRange(-10.0, 20.0);
        }

        [Test]
        public void It_should_pick_enum()
        {
            Rand.Pick<TestEnum>().Should().BeOfType<TestEnum>();
        }

        [Test]
        public void It_should_generate_float()
        {
            Rand.Float().Should().BeInRange(0.0f, 1.0f);
            Rand.Float(20.0f).Should().BeLessThan(20.0f);
            Rand.Float(-10.0f, 20.0f).Should().BeInRange(-10.0f, 20.0f);
        }

        [Test]
        public void It_should_generate_int()
        {
            Rand.Int32().Should().BeGreaterOrEqualTo(0);
            Rand.Int32(10).Should().BeInRange(0, 10);
            Rand.Int32(-10, 10).Should().BeInRange(-10, 10);
        }

        [Test]
        public void It_should_generate_long()
        {
            Rand.Int64().Should().BeGreaterOrEqualTo(0);
            Rand.Int64(10).Should().BeInRange(0, 10);
            Rand.Int64(-10, 10).Should().BeInRange(-10, 10);
        }

        [Test]
        public void It_should_generate_string()
        {
            Rand.String().Should().HaveLength(8);
        }

        [Test]
        public void It_should_generate_text()
        {
            Rand.Text().Should().HaveLength(2000);
        }

        [Test]
        public void It_should_pick_enumerable()
        {
            var values = new[] { 1, 2, 3 };
            values.Should().Contain(Rand.Pick(values));
        }

        [Test]
        public void It_should_generate_colors()
        {
            Rand.Color().Should().NotBe(Color.Empty);
        }

        [Test]
        public void It_should_generate_datetimes()
        {
            Rand.DateTime(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(5))
                .Should()
                .BeWithin(TimeSpan.FromDays(10))
                .After(DateTime.Now.AddDays(-5));

            Rand.DateTime(10).Should().BeWithin(TimeSpan.FromDays(20)).After(DateTime.Now.AddDays(-10));

            Rand.DateTime().Should().BeWithin(TimeSpan.FromDays(10)).After(DateTime.Now.AddDays(-5));

            Rand.DateTime(true).Should().BeWithin(TimeSpan.FromDays(10)).After(DateTime.Now);
        }

        [Test]
        public void It_should_generate_timespan()
        {
            Rand.TimeSpan().Should().BeLessOrEqualTo(TimeSpan.FromHours(1));
        }
    }
}
