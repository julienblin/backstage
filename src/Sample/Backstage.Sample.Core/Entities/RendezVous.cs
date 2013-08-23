namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using Backstage.NHibernateProvider;

    /// <summary>
    /// A <see cref="RendezVous"/> is the combination of a <see cref="Location"/> and a <see cref="Date"/>.
    /// </summary>
    public class RendezVous : NHEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the location of the <see cref="RendezVous"/>.
        /// </summary>
        [Required]
        public virtual Location Location { get; set; }

        /// <summary>
        /// Gets or sets the date of the <see cref="RendezVous"/>.
        /// </summary>
        [Required]
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}@{1:yyyy-MM-dd}", this.Location, this.Date);
        }
    }
}
