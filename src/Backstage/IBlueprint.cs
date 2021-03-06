﻿namespace Backstage
{
    using System;

    /// <summary>
    /// Marker interface for blueprints.
    /// You might want to use <see cref="Blueprint{T}"/> instead of this interface.
    /// </summary>
    public interface IBlueprint
    {
        /// <summary>
        /// Gets the (base) <see cref="Type"/> that this blueprint builds.
        /// </summary>
        Type BuiltType { get; }

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
}
