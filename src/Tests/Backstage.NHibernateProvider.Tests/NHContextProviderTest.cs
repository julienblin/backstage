namespace Backstage.NHibernateProvider.Tests
{
    using System;

    using Backstage.NHibernateProvider.Events;
    using Backstage.NHibernateProvider.Tests.Entities;

    using FluentAssertions;

    using NHibernate;

    using NUnit.Framework;

    [TestFixture]
    public class NHContextProviderTest
    {
        [Test]
        public void It_should_persists_entities()
        {
            var id = Guid.Empty;
            try
            {
                using (var context = ContextFactory.Current.StartNewContext())
                {
                    var employee = new Employee { Name = "Foo", Age = 25 };
                    context.Add(employee);
                    context.Commit();
                    id = employee.Id;
                    id.Should().NotBe(Guid.Empty);
                }
            }
            finally
            {
                if (id != Guid.Empty)
                {
                    using (var context = ContextFactory.Current.StartNewContext())
                    {
                        var employee = context.GetById<Employee>(id);
                        employee.Should().NotBeNull();
                        context.Remove(employee);
                        context.Commit();
                    }

                    using (var context = ContextFactory.Current.StartNewContext())
                    {
                        context.GetById<Employee>(id).Should().BeNull();
                    }
                }
            }
        }

        [Test]
        public void It_should_raise_events()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                EntityInsertedEvent<Employee> employeePersistedEvent = null;
                EntityUpdatedEvent<Employee> employeeModifiedEvent = null;
                EntityDeletedEvent<Employee> employeeDeletedEvent = null;
                DomainEvents.DomainEventRaised += (sender, args) =>
                {
                    if (args.DomainEvent is EntityInsertedEvent<Employee>)
                    {
                        employeePersistedEvent = (EntityInsertedEvent<Employee>)args.DomainEvent;
                    }

                    if (args.DomainEvent is EntityUpdatedEvent<Employee>)
                    {
                        employeeModifiedEvent = (EntityUpdatedEvent<Employee>)args.DomainEvent;
                    }

                    if (args.DomainEvent is EntityDeletedEvent<Employee>)
                    {
                        employeeDeletedEvent = (EntityDeletedEvent<Employee>)args.DomainEvent;
                    }
                };

                var employee = new Employee { Age = 25 };
                context.Add(employee);

                // The query will force a flush.
                context.Fulfill(new DumbQuery()).List();

                employeePersistedEvent.Should().NotBeNull();
                employeePersistedEvent.Entity.Should().Be(employee);

                employee.Name = "Foo";

                // The query will force a flush.
                context.Fulfill(new DumbQuery()).List();

                employeeModifiedEvent.Should().NotBeNull();
                employeeModifiedEvent.Entity.Should().Be(employee);
                employeeModifiedEvent.DirtyPropertyNames.Should().OnlyContain(x => x == "Name");

                context.Remove(employee);

                // The query will force a flush.
                context.Fulfill(new DumbQuery()).List();

                employeeDeletedEvent.Should().NotBeNull();
                employeeDeletedEvent.Entity.Should().Be(employee);
            }
        }

        private class DumbQuery : NHQueryQueryOver<Employee>
        {
            protected override IQueryOver<Employee, Employee> BuildQueryOver(IContextProvider contextProvider, ISession session)
            {
                return session.QueryOver<Employee>();
            }
        }
    }
}
