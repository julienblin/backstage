namespace Backstage.NHibernateProvider.Tests.Mappings
{
    using System;

    using Backstage.NHibernateProvider.Tests.Entities;

    public class EmployeeMap : NHEntityMap<Employee, Guid>
    {
        public EmployeeMap()
        {
            this.Map(x => x.Name);
            this.Map(x => x.Age);

            this.References(x => x.Manager);

            this.HasMany(x => x.Vacancies)
                .Inverse()
                .AsSet()
                .Cascade.DeleteOrphan();
        }
    }
}
