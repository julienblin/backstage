namespace Backstage.Sample.Core.Mappings
{
    using System;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Entities;

    /// <summary>
    /// The <see cref="Itinerary"/> mappings.
    /// </summary>
    public class ItineraryMap : NHEntityMap<Itinerary, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItineraryMap"/> class.
        /// </summary>
        public ItineraryMap()
        {
            this.HasMany(x => x.RendezVousLinks).Inverse().AsSet().OrderBy("StepNumber").Cascade.AllDeleteOrphan();
        }
    }
}
