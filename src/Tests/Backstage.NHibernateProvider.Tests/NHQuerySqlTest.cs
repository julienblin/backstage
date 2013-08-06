namespace Backstage.NHibernateProvider.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Backstage.NHibernateProvider.Tests.Entities;

    using FluentAssertions;

    using NHibernate;

    using NUnit.Framework;

    [TestFixture]
    public class NHQuerySqlTest
    {
        [Test]
        public void It_should_execute_query()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(new Employee { Name = "Foo" });
                context.Add(new Employee { Name = "Bar" });

                var result = context.Fulfill(new NHQuerySqlSample());
                result.Should().HaveCount(2);
                result.First().Name.Should().Be("Bar");
            }
        }

        private class NHQuerySqlSample : NHQuerySql<IEnumerable<Employee>>
        {
            protected override IEnumerable<Employee> BuildSqlQuery(IContextProvider contextProvider, ISession session)
            {
                return session.CreateSQLQuery("SELECT * FROM Employee ORDER BY Name").AddEntity(typeof(Employee)).List<Employee>();
            }
        }
    }
}
