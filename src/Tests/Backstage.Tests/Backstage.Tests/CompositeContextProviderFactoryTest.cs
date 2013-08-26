namespace Backstage.Tests
{
    using System.Linq;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CompositeContextProviderFactoryTest
    {
        [Test]
        public void It_should_delegate_to_the_appropriate_provider()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IContext>();

            var contextProviderMock1 = new Mock<IContextProvider>();
            var contextFactoryProviderMock1 = new Mock<IContextProviderFactory>();
            contextFactoryProviderMock1.Setup(x => x.CreateContextProvider(contextMock.Object))
                                       .Returns(contextProviderMock1.Object);

            var contextProviderMock2 = new Mock<IContextProvider>();
            var contextFactoryProviderMock2 = new Mock<IContextProviderFactory>();
            contextFactoryProviderMock2.Setup(x => x.CreateContextProvider(contextMock.Object))
                                       .Returns(contextProviderMock2.Object);

            using (var compositeContextFactory = new CompositeContextProviderFactory(
                (providers, type) =>
                    {
                        if ((type == typeof(Entity1)) || (type == typeof(Query1)))
                        {
                            return providers.First();
                        }

                        return providers.Skip(1).First();
                    },
                contextFactoryProviderMock1.Object,
                contextFactoryProviderMock2.Object))
            {
                compositeContextFactory.Start(contextFactoryMock.Object);
                contextFactoryProviderMock1.Verify(x => x.Start(contextFactoryMock.Object));
                contextFactoryProviderMock2.Verify(x => x.Start(contextFactoryMock.Object));

                var contextProvider = compositeContextFactory.CreateContextProvider(contextMock.Object);
                contextFactoryProviderMock1.Verify(x => x.CreateContextProvider(contextMock.Object));
                contextFactoryProviderMock2.Verify(x => x.CreateContextProvider(contextMock.Object));

                var entity1 = new Entity1();
                var entity2 = new Entity2();
                var query1 = new Query1();

                contextProvider.Add(entity1);
                contextProviderMock1.Verify(x => x.Add(entity1));

                contextProvider.Remove(entity2);
                contextProviderMock2.Verify(x => x.Remove(entity2));

                contextProvider.Reload(entity1);
                contextProviderMock1.Verify(x => x.Reload(entity1));

                contextProvider.GetById(typeof(Entity2), 5);
                contextProviderMock2.Verify(x => x.GetById(typeof(Entity2), 5));

                contextProvider.Fulfill(query1);
                contextProviderMock1.Verify(x => x.Fulfill(query1));
            }
        }

        [Test]
        public void It_should_throw_if_selection_function_returns_null()
        {
            var contextFactoryMock = new Mock<IContextFactory>();
            var contextMock = new Mock<IContext>();

            using (var compositeContextFactory = new CompositeContextProviderFactory((providers, type) => null))
            {
                compositeContextFactory.Start(contextFactoryMock.Object);
                var contextProvider = compositeContextFactory.CreateContextProvider(contextMock.Object);

                var entity = new Mock<IEntity>();

                contextProvider.Invoking(x => x.Add(entity.Object))
                               .ShouldThrow<BackstageException>()
                               .WithMessage("*selection*");
            }
        }

        private class Entity1 : IEntity
        {
        }

        private class Entity2 : IEntity
        {
        }

        private class Query1 : IQuery<Entity1>
        {
        }
    }
}
