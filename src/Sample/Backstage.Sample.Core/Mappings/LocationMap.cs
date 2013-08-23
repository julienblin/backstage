namespace Backstage.Sample.Core.Mappings
{
    using System;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Entities;

    /// <summary>
    /// The <see cref="Location"/> mappings.
    /// </summary>
    public class LocationMap : NHEntityMap<Location, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationMap"/> class.
        /// </summary>
        public LocationMap()
        {
            this.Map(x => x.Name);
            this.Component(
                x => x.UNLocationCode,
                m => { m.Map(x => x.CountryCode); m.Map(x => x.LocationCode); });
        }
    }
}
