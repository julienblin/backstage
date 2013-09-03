namespace Backstage.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class TypeScannerTest
    {
        public interface ITest
        {
        }

        [Test]
        public void It_should_find_concrete_implementations()
        {
            TypeScanner.FindConcreteImplementationsOf<ITest>()
                       .Should()
                       .Contain(typeof(ConcreteClass))
                       .And.NotContain(typeof(AbstractClass));
        }

        [Test]
        public void It_should_find_and_build_concrete_implementations()
        {
            TypeScanner.FindAndBuildImplementationsOf<ITest>().Should().OnlyContain(x => x is ConcreteClass);
        }

        public abstract class AbstractClass : ITest
        {
        }

        public class ConcreteClass : AbstractClass
        {
        }
    }
}
