namespace Backstage.NHibernateProvider.Conventions
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.Inspections;

    /// <summary>
    /// The default convention for many to many table names.
    /// </summary>
    public class DefaultManyToManyTableNameConvention : ManyToManyTableNameConvention
    {
        /// <summary>
        /// Bi directional table name.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <param name="otherSide">
        /// The other side.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide)
        {
            return collection.EntityType.Name + "_" + otherSide.EntityType.Name;
        }

        /// <summary>
        /// Single direction table name.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return collection.EntityType.Name + "_" + collection.ChildType.Name;
        }
    }
}
