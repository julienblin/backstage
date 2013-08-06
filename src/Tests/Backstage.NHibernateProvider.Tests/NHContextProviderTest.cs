namespace Backstage.NHibernateProvider.Tests
{
    using System;

    using Backstage.NHibernateProvider.Tests.Entities;

    using FluentAssertions;

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
    }
}
