namespace Backstage.Tests
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DomainEventsTest
    {
        [Test]
        public void It_should_register_assemblies_on_ContextFactory_creation_and_unsubscribe_on_Dispose()
        {
            var contextFactoryProviderMock = new Mock<IContextProviderFactory>();
            using (ContextFactory.StartNew(
                new ContextFactoryConfiguration(contextFactoryProviderMock.Object)
                    {
                        DomainEventHandlersAssemblies = new[] { typeof(DomainEventsTest).Assembly }
                    }))
            {
                DomainEvents.GetSubscriptions().Should().HaveCount(1);
            }

            DomainEvents.GetSubscriptions().Should().HaveCount(0);
        }

        [Test]
        public void It_should_raise_events()
        {
            DomainEvents.Subscribe<DomainEventHandlerSample, DomainEventSample>();
            DomainEvents.Raise(new DomainEventSample());

            DomainEventHandlerSample.ReceivedEvents.Should().HaveCount(1);

            DomainEvents.Unsubscribe<DomainEventHandlerSample, DomainEventSample>();
            DomainEvents.Raise(new DomainEventSample());

            DomainEventHandlerSample.ReceivedEvents.Should().HaveCount(1);
        }

        public class DomainEventSample : IDomainEvent
        {
        }

        public class DomainEventHandlerSample : IHandleDomainEvent<DomainEventSample>
        {
            public static readonly IList<DomainEventSample> ReceivedEvents = new List<DomainEventSample>();

            public void Handle(DomainEventSample @event)
            {
                ReceivedEvents.Add(@event);
            }
        }
    }
}
