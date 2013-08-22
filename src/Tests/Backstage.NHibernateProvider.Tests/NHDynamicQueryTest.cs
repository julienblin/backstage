namespace Backstage.NHibernateProvider.Tests
{
    using System;

    using Backstage.NHibernateProvider.Tests.Entities;

    using FluentAssertions;

    using NHibernate.Criterion;

    using NUnit.Framework;

    [TestFixture]
    public class NHDynamicQueryTest
    {
        [Test]
        public void It_should_filter_Eq()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo", Age = 25 });
                context.Add(new Employee { Name = "Bar", Age = 30 });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Name.Eq("Foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Age.Eq(25);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query.Name.Eq("Bar");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(0);
            }
        }

        [Test]
        public void It_should_filter_Like_and_InsensitiveLike()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "FooBar" });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo", MatchMode.Start);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.Like("Foo", MatchMode.End);
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Name.InsensitiveLike("foo");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(2);

                query = new NHDynamicQuery<Employee>();
                query.Name.InsensitiveLike("bar");
                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }

        [Test]
        public void It_should_handle_joins()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                var now = DateTime.Now;

                var employee = new Employee { Name = "Foo" };
                context.Add(employee);
                context.Add(new Employee { Name = "FooBar" });

                var vacancy = new Vacancy { Employee = employee, StartDate = now, EndDate = now.AddDays(5) };
                context.Add(vacancy);

                dynamic query = new NHDynamicQuery<Employee>();
                query.Vacancies.StartDate.Gt(now.AddDays(-1));
                query.Vacancies.EndDate.Lt(now.AddDays(6));

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }

        [Test]
        public void It_should_handle_references()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                var manager = new Employee { Name = "The Boss." };
                context.Add(manager);

                var subordinate = new Employee { Name = "subordinate", Manager = manager };
                context.Add(subordinate);

                context.Add(new Employee { Name = "another one" });

                dynamic query = new NHDynamicQuery<Employee>();
                query.Manager.IsNotNull();

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);

                query = new NHDynamicQuery<Employee>();
                query.Manager.Name.Eq("The Boss.");
                query.Manager.Manager.IsNull();

                context.Fulfill((NHDynamicQuery<Employee>)query).Count().Should().Be(1);
            }
        }
    }
}
