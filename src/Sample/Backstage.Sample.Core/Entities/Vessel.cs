namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Validators;

    /// <summary>
    /// A vessel.
    /// </summary>
    public class Vessel : NHEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the imo number.
        /// see <a href="http://en.wikipedia.org/wiki/IMO_numbers#Assignment_and_Structure">Wikipedia</a>.
        /// </summary>
        [Required]
        [StringLength(10)]
        [IMONumber]
        public virtual string IMONumber { get; set; }
    }
}
