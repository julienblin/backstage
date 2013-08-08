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
        public void It_should_register_assemblies_on_ContextFactory_creation()
        {
            var contextFactoryProviderMock = new Mock<IContextProviderFactory>();
            using (ContextFactory.StartNew(
                new ContextFactoryConfiguration(contextFactoryProviderMock.Object)
                    {
                        DomainEventHandlersAssemblies = new[] { typeof(DomainEventsTest).Assembly }
                    }))
            {
                DomainEvents.GetHandlers<DomainEventSample>().Should().HaveCount(1);
            }
        }

        [Test]
        public void It_should_raise_events()
        {
            var eventHandler = new DomainEventHandlerSample();
            DomainEvents.Subscribe(eventHandler);
            DomainEvents.Raise(new DomainEventSample());

            DomainEventHandlerSample.ReceivedEvents.Should().HaveCount(1);

            DomainEvents.Unsubscribe(eventHandler);
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
