namespace Backstage.NHibernateProvider.Tests.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    public class Employee : NHEntity<Guid>
    {
        private ICollection<Vacancy> vacancies;

        public Employee()
        {
            this.vacancies = new Collection<Vacancy>();
        }

        [Required]
        [StringLength(50)]
        public virtual string Name { get; set; }

        public virtual int Age { get; set; }

        public virtual Employee Manager { get; set; }

        public virtual IEnumerable<Vacancy> Vacancies
        {
            get
            {
                return this.vacancies;
            }
        }
    }
}
