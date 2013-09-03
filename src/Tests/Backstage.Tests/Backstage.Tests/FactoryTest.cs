namespace Backstage.Tests
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class FactoryTest
    {
        private Mock<IContextProviderFactory> contextProviderFactoryMock;
        private Mock<IContextProvider> contextProviderMock;

        [SetUp]
        public void SetUp()
        {
            this.contextProviderFactoryMock = new Mock<IContextProviderFactory>();
            this.contextProviderMock = new Mock<IContextProvider>();
            this.contextProviderFactoryMock.Setup(x => x.CreateContextProvider(It.IsAny<IContext>())).Returns(this.contextProviderMock.Object);
            ContextFactory.StartNew(new ContextFactoryConfiguration(this.contextProviderFactoryMock.Object));
            Context.Current = ContextFactory.Current.StartNewContext();
        }

        [TearDown]
        public void TearDown()
        {
            if (Context.HasCurrent)
            {
                Context.Current.Dispose();
            }

            if (ContextFactory.HasCurrent)
            {
                ContextFactory.Current.Dispose();
            }
        }

        [Test]
        public void It_should_build()
        {
            var built = Factory.Build<TestObject>();
            built.Name.Should().Be("Foo");
        }

        [Test]
        public void It_should_build_n()
        {
            var built = Factory.Build<TestObject>(5);
            built.Should().HaveCount(5);
            built.First().Name.Should().Be("Foo");
        }

        [Test]
        public void It_should_build_with_overrides()
        {
            var built = Factory.Build<TestObject>(x => x.Name = "FooBar");
            built.Name.Should().Be("FooBar");
        }

        [Test]
        public void It_should_build_with_overrides_and_n()
        {
            var built = Factory.Build<TestObject>(10, (i, x) => x.Name = "FooBar" + i);
            built.Should().HaveCount(10);
            built.First().Name.Should().Be("FooBar0");
        }

        [Test]
        public void It_should_build_subclasses()
        {
            var built = Factory.Build<TestObjectsubclass>();
            built.Name.Should().Be("Bar");
        }

        [Test]
        public void It_should_build_and_add()
        {
            var built = Factory.BuildAndAdd<TestObjectsubclass>();
            built.Name.Should().Be("Bar");
            this.contextProviderMock.Verify(x => x.Add(built));
        }

        [Test]
        public void It_should_build_and_add_n()
        {
            var built = Factory.BuildAndAdd<TestObjectsubclass>(5).ToList();
            built.Should().HaveCount(5);
            built.First().Name.Should().Be("Bar");
            this.contextProviderMock.Verify(x => x.Add(It.IsAny<TestObjectsubclass>()), Times.Exactly(5));
        }

        [Test]
        public void It_should_build_and_add_non_generic()
        {
            var built = (TestObjectsubclass)Factory.BuildAndAdd(typeof(TestObjectsubclass));
            built.Name.Should().Be("Bar");
            this.contextProviderMock.Verify(x => x.Add(built));
        }

        [Test]
        public void It_should_throw_exception_on_build_and_add_if_not_IEntity()
        {
            Action act = () => Factory.BuildAndAdd(typeof(TestObject));

            act.ShouldThrow<BackstageException>().WithMessage("*IEntity*");
        }

        [Test]
        public void It_should_return_buildable_types()
        {
            Factory.BuildableTypes.Should().Contain(typeof(TestObject));
        }

        public class TestObject
        {
            public string Name { get; set; }
        }

        public class TestObjectsubclass : TestObject, IEntity
        {
        }

        public class TestObjectBlueprint : Blueprint<TestObject>
        {
            public override object Build(Type targetType)
            {
                if (targetType == typeof(TestObjectsubclass))
                {
                    return new TestObjectsubclass { Name = "Bar" };
                }

                return new TestObject { Name = "Foo" };
            }
        }
    }
}
