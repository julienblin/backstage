namespace Backstage.Tests
{
    using System;
    using System.Linq.Expressions;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class ExpressionExtensionsTest
    {
        [Test]
        public void It_should_get_property_name()
        {
            Expression<Func<TestExpression, object>> expression = e => e.Name;
            expression.GetPropertyName().Should().Be("Name");

            expression = e => e.Age;
            expression.GetPropertyName().Should().Be("Age");
        }

        private class TestExpression
        {
            public string Name { get; set; }

            public int? Age { get; set; }
        }
    }
}
