namespace Backstage.Sample.Core.Mappings
{
    using System;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Entities;

    /// <summary>
    /// The <see cref="ItineraryRendezVousLink"/> mappings.
    /// </summary>
    public class ItineraryRendezVousLinkMap : NHEntityMap<ItineraryRendezVousLink, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItineraryRendezVousLinkMap"/> class.
        /// </summary>
        public ItineraryRendezVousLinkMap()
        {
            this.Map(x => x.StepNumber);

            this.References(x => x.Itinerary);
            this.References(x => x.RendezVous);
        }
    }
}
