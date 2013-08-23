namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Backstage.NHibernateProvider;

    /// <summary>
    /// Represents an ordered link between a <see cref="Itinerary"/> and a <see cref="RendezVous"/>.
    /// </summary>
    public class ItineraryRendezVousLink : NHEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the step number (order in the itinerary).
        /// </summary>
        public virtual int StepNumber { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Itinerary"/>.
        /// </summary>
        [Required]
        public virtual Itinerary Itinerary { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RendezVous"/>.
        /// </summary>
        [Required]
        public virtual RendezVous RendezVous { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}->{1}", this.Itinerary != null ? this.Itinerary.Id.ToString() : "Unknown itinerary", this.RendezVous);
        }
    }
}
