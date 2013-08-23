namespace Backstage.Sample.Core.Mappings
{
    using System;

    using Backstage.NHibernateProvider;
    using Backstage.Sample.Core.Entities;

    /// <summary>
    /// The <see cref="RendezVous"/> mappings.
    /// </summary>
    public class RendezVousMap : NHEntityMap<RendezVous, Guid>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendezVousMap"/> class.
        /// </summary>
        public RendezVousMap()
        {
            this.Map(x => x.Date);

            this.References(x => x.Location);
        }
    }
}
