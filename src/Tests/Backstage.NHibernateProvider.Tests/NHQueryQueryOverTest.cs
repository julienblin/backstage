namespace Backstage.NHibernateProvider.Tests
{
    using System;
    using System.Linq;

    using Backstage.NHibernateProvider.Tests.Entities;

    using FluentAssertions;

    using NHibernate;
    using NHibernate.Criterion;

    using NUnit.Framework;

    [TestFixture]
    public class NHQueryQueryOverTest
    {
        [Test]
        public void It_should_execute_and_paginate()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample { NameLike = "F" }).Paginate();

                result.TotalItems.Should().Be(1);
                result.Result.Should().HaveCount(1);
                result.Result.First().Name.Should().Be("Foo");
            }
        }

        [Test]
        public void It_should_execute_and_list()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample { NameLike = "F" }).List();

                result.Should().HaveCount(1);
                result.First().Name.Should().Be("Foo");
            }
        }

        [Test]
        public void It_should_execute_and_count()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample { NameLike = "F" }).Count();

                result.Should().Be(1);
            }
        }

        [Test]
        public void It_should_execute_and_FirstOrDefault()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample { NameLike = "F" }).FirstOrDefault();
                result.Name.Should().Be("Foo");

                result = context.Fulfill(new NHQueryQueryOverSample { NameLike = "asdf" }).FirstOrDefault();
                result.Should().BeNull();
            }
        }

        [Test]
        public void It_should_execute_and_Fetch()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                var employee = new Employee { Name = "Foo" };
                context.Add(employee);
                context.Add(new Employee { Name = "Bar" });

                context.Add(new Vacancy { Employee = employee, StartDate = DateTime.Now });

                var result = context.Fulfill(new NHQueryQueryOverSample()).Fetch(x => x.Vacancies).List();

                result.Should().HaveCount(2);
            }
        }

        [Test]
        public void It_should_execute_and_OrderBy()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample()).OrderBy(x => x.Name).List();
                result.First().Name.Should().Be("Bar");

                result = context.Fulfill(new NHQueryQueryOverSample()).OrderBy("Name", OrderType.Desc).List();
                result.First().Name.Should().Be("Foo");
            }
        }

        [Test]
        public void It_should_execute_and_Distinct()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQueryQueryOverSample()).OrderBy(x => x.Name).Distinct<string>(x => x.Name);
                result.Should().HaveCount(2);
                result.First().Should().Be("Bar");
            }
        }

        private class NHQueryQueryOverSample : NHQueryQueryOver<Employee>
        {
            public string NameLike { get; set; }

            protected override IQueryOver<Employee, Employee> BuildQueryOver(IContextProvider contextProvider, ISession session)
            {
                var query = session.QueryOver<Employee>();

                if (!string.IsNullOrEmpty(this.NameLike))
                {
                    query.Where(
                        Restrictions.Like(Projections.Property<Employee>(x => x.Name), this.NameLike, MatchMode.Anywhere));
                }

                return query;
            }
        }
    }
}
