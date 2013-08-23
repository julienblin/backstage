namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Backstage.NHibernateProvider;

    /// <summary>
    /// A location is a stop on a journey, such as cargo origin or destination, or carrier movement endpoints.
    /// It is uniquely identified by a <see cref="UNLocationCode"/>.
    /// </summary>
    public class Location : NHEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the actual name of the location.
        /// </summary>
        [Required]
        [StringLength(255)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the United Nations location code.
        /// </summary>
        [Required]
        public virtual UNLocationCode UNLocationCode { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, this.UNLocationCode);
        }
    }
}
