namespace Backstage.NHibernateProvider.Tests.Mappings
{
    using Backstage.NHibernateProvider.Tests.Entities;

    public class VacancyMap : NHEntityMap<Vacancy, int>
    {
        public VacancyMap()
        {
            this.Map(x => x.StartDate);
            this.Map(x => x.EndDate);

            this.References(x => x.Employee);
        }
    }
}
