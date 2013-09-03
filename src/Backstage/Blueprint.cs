namespace Backstage
{
    using System;

    /// <summary>
    /// A blueprint is the way to tell the <see cref="Factory"/> how to build objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of objects built.
    /// </typeparam>
    public abstract class Blueprint<T> : IBlueprint
    {
        /// <summary>
        /// Gets the (base) <see cref="Type"/> that this blueprint builds.
        /// </summary>
        public Type BuiltType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Builds a minimal valid object.
        /// </summary>
        /// <param name="targetType">
        /// The target type to build. Can be used when the factory is generic and 
        /// can build subtypes.
        /// </param>
        /// <returns>
        /// The created object.
        /// </returns>
        public abstract object Build(Type targetType);
    }
}