namespace Backstage.NHibernateProvider.Tests.Entities
{
    using System;

    public class Vacancy : NHEntity<int>
    {
        public virtual Employee Employee { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }
    }
}
