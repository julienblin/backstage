namespace Backstage.Tests.Implementation
{
    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultContextFactoryTest
    {
        [Test]
        public void It_should_start_and_dispose_the_context_provider_factory()
        {
            var contextFactoryProviderMock = new Mock<IContextProviderFactory>();
            var contextFactory = ContextFactory.StartNew(new ContextFactoryConfiguration(contextFactoryProviderMock.Object));

            ContextFactory.Current.Should().Be(contextFactory);
            ContextFactory.Current.Dispose();

            contextFactoryProviderMock.Verify(x => x.Start(contextFactory));
            contextFactoryProviderMock.Verify(x => x.Dispose());
        }

        [Test]
        public void It_should_create_context_providers()
        {
            var contextFactoryProviderMock = new Mock<IContextProviderFactory>();
            var contextProviderMock = new Mock<IContextProvider>();
            contextFactoryProviderMock.Setup(x => x.CreateContextProvider(It.IsAny<IContext>())).Returns(contextProviderMock.Object);
            ContextFactory.StartNew(new ContextFactoryConfiguration(contextFactoryProviderMock.Object));

            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Should().NotBeNull();
                contextFactoryProviderMock.Verify(x => x.CreateContextProvider(context));
            }
        }
    }
}
