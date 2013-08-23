namespace Backstage.Sample.Core.Mappings
{
    using System;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Entities;

    /// <summary>
    /// The <see cref="Vessel"/> mappings.
    /// </summary>
    public class VesselMap : NHEntityMap<Vessel, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VesselMap"/> class.
        /// </summary>
        public VesselMap()
        {
            this.Map(x => x.IMONumber);
        }
    }
}
