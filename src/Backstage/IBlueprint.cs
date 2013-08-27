namespace Backstage
{
    using System;

    /// <summary>
    /// Marker interface for <see cref="IBlueprint{T}"/>.
    /// Please use the generic version when implementing.
    /// </summary>
    public interface IBlueprint
    {
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
        object Build(Type targetType);
    }

    /// <summary>
    /// A blueprint is the way to tell the <see cref="Factory"/> how to build objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of objects built.
    /// </typeparam>
    public interface IBlueprint<out T> : IBlueprint
    {
    }
}
