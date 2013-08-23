namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Backstage.NHibernateProvider;

    /// <summary>
    /// A route is an ordered collection of <see cref="RendezVous"/>.
    /// </summary>
    public class Itinerary : NHEntity<Guid>
    {
        /// <summary>
        /// The itinerary.
        /// </summary>
        private ICollection<ItineraryRendezVousLink> rendezVousLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Itinerary"/> class.
        /// </summary>
        public Itinerary()
        {
            this.rendezVousLinks = new Collection<ItineraryRendezVousLink>();
        }

        /// <summary>
        /// Gets the itinerary.
        /// </summary>
        public virtual IEnumerable<ItineraryRendezVousLink> RendezVousLinks
        {
            get
            {
                return this.rendezVousLinks;
            }
        }

        /// <summary>
        /// Gets the itinerary, in order.
        /// </summary>
        /// <returns>
        /// The list of <see cref="RendezVous"/>.
        /// </returns>
        public virtual IEnumerable<RendezVous> GetRendezVous()
        {
            return this.RendezVousLinks.OrderBy(x => x.StepNumber).Select(x => x.RendezVous);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var rendezVous = this.GetRendezVous().ToList();
            return string.Format("Itinerary #{0}({1}->{2})", this.Id, rendezVous.FirstOrDefault(), rendezVous.LastOrDefault());
        }
    }
}
