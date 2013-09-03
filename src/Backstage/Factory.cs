namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Common.Logging;

    /// <summary>
    /// Factory - use it to build valid objects. Useful for testing.
    /// Objects are defined using <see cref="IBlueprint"/> and <see cref="Blueprint{T}"/>.
    /// Blueprints are auto-discovered at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// public class Employee
    /// {
    ///     public string Name {get;set;
    /// }
    /// 
    /// public class EmployeeBlueprint : Blueprint<Employee>
    /// {
    ///     public Employee Build()
    ///     {
    ///         return new Employee { Name = "Joe"; }
    ///     }
    /// }
    /// 
    /// Factory.Build<Employee>(); // => Employee { Name = "Joe" }
    /// Factory.Build<Employee>(x => x.Name = "Bill"); // => Employee { Name = "Bill" }
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public static class Factory
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The blueprints.
        /// </summary>
        private static readonly Lazy<IDictionary<Type, IBlueprint>> Blueprints = new Lazy<IDictionary<Type, IBlueprint>>(GetBlueprints, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the list of types that can be built.
        /// </summary>
        /// <returns>
        /// The types that can be built.
        /// </returns>
        public static IEnumerable<Type> BuildableTypes
        {
            get
            {
                return Blueprints.Value.Keys.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Builds an instance of <typeparamref name="T"/> according to a blueprint.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created object.
        /// </returns>
        public static T Build<T>()
        {
            return Build<T>(null);
        }

        /// <summary>
        /// Builds n instances of <typeparamref name="T"/> according to a blueprint.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created object.
        /// </returns>
        public static IEnumerable<T> Build<T>(int numberOfInstances)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                yield return Build<T>(null);
            }
        }

        /// <summary>
        /// Builds an instance of <typeparamref name="T"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created object.
        /// </returns>
        public static T BuildAndAdd<T>()
            where T : IEntity
        {
            return BuildAndAdd<T>(null);
        }

        /// <summary>
        /// Builds n instances of <typeparamref name="T"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<T> BuildAndAdd<T>(int numberOfInstances)
            where T : IEntity
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                yield return BuildAndAdd<T>(null);
            }
        }

        /// <summary>
        /// Builds an instance of <typeparamref name="T"/> according to a blueprint.
        /// </summary>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created object.
        /// </returns>
        public static T Build<T>(Action<T> overrides)
        {
            if (overrides != null)
            {
                return (T)Build(typeof(T), x => overrides((T)x));
            }

            return (T)Build(typeof(T), null);
        }

        /// <summary>
        /// Builds n instances of <typeparamref name="T"/> according to a blueprint.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<T> Build<T>(int numberOfInstances, Action<int, T> overrides)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                var localIndex = i;
                yield return Build<T>(x => overrides(localIndex, x));
            }
        }

        /// <summary>
        /// Builds an instance of <typeparamref name="T"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created object.
        /// </returns>
        public static T BuildAndAdd<T>(Action<T> overrides)
            where T : IEntity
        {
            var built = Build(overrides);
            Context.Current.Add(built);
            return built;
        }

        /// <summary>
        /// Builds n instances of <typeparamref name="T"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <typeparam name="T">
        /// The type.
        /// </typeparam>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<T> BuildAndAdd<T>(int numberOfInstances, Action<int, T> overrides)
            where T : IEntity
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                var localIndex = i;
                yield return BuildAndAdd<T>(x => overrides(localIndex, x));
            }
        }

        /// <summary>
        /// Builds an instance of <paramref name="type"/> according to a blueprint.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The created object.
        /// </returns>
        public static object Build(Type type)
        {
            return Build(type, null);
        }

        /// <summary>
        /// Builds n instances of <paramref name="type"/> according to a blueprint.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<object> Build(int numberOfInstances, Type type)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                yield return Build(type);
            }
        }

        /// <summary>
        /// Builds an instance of <paramref name="type"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The created object.
        /// </returns>
        public static object BuildAndAdd(Type type)
        {
            return BuildAndAdd(type, null);
        }

        /// <summary>
        /// Builds n instances of <paramref name="type"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<object> BuildAndAdd(int numberOfInstances, Type type)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                yield return BuildAndAdd(type);
            }
        }

        /// <summary>
        /// Builds an instance of <paramref name="type"/> according to a blueprint.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <returns>
        /// The created object.
        /// </returns>
        public static object Build(Type type, Action<object> overrides)
        {
            var blueprint = FindBlueprint(type);
            var result = blueprint.Build(type);
            if (overrides != null)
            {
                overrides(result);
            }

            return result;
        }

        /// <summary>
        /// Builds n instances of <paramref name="type"/> according to a blueprint.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <returns>
        /// The created objects.
        /// </returns>
        public static IEnumerable<object> Build(int numberOfInstances, Type type, Action<int, object> overrides)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                var localIndex = i;
                yield return Build(type, x => overrides(localIndex, x));
            }
        }

        /// <summary>
        /// Builds an instance of <paramref name="type"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <returns>
        /// The created objects.
        /// </returns>
        /// <exception cref="BackstageException">
        /// if <paramref name="type"/> doesn't implement <see cref="IEntity"/>.
        /// </exception>
        public static object BuildAndAdd(Type type, Action<object> overrides)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                var message = string.Format(CultureInfo.InvariantCulture, Resources.TypeMustBeIEntity, type);
                Log.Error(message);
                throw new BackstageException(message);
            }

            var built = Build(type, overrides);
            Context.Current.Add((IEntity)built);
            return built;
        }

        /// <summary>
        /// Builds n instances of <paramref name="type"/> according to a blueprint,
        /// and adds it to the <see cref="Context.Current"/>.
        /// </summary>
        /// <param name="numberOfInstances">
        /// The number of instances to build.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="overrides">
        /// Overrides the values.
        /// </param>
        /// <returns>
        /// The created objects.
        /// </returns>
        /// <exception cref="BackstageException">
        /// if <paramref name="type"/> doesn't implement <see cref="IEntity"/>.
        /// </exception>
        public static IEnumerable<object> BuildAndAdd(int numberOfInstances, Type type, Action<int, object> overrides)
        {
            for (var i = 0; i < numberOfInstances; i++)
            {
                var localIndex = i;
                yield return BuildAndAdd(type, x => overrides(localIndex, x));
            }
        }

        /// <summary>
        /// Returns the blueprint associated with <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="IBlueprint"/>.
        /// </returns>
        /// <exception cref="BackstageException">
        /// If not blueprint found.
        /// </exception>
        private static IBlueprint FindBlueprint(Type type)
        {
            // First found exact type.
            if (Blueprints.Value.ContainsKey(type))
            {
                return Blueprints.Value[type];
            }

            // Otherwise, try to find parent types.
            var foundParentBlueprint =
                Blueprints.Value.Keys.FirstOrDefault(objectType => objectType.IsAssignableFrom(type));
            if (foundParentBlueprint != null)
            {
                return Blueprints.Value[foundParentBlueprint];
            }

            var message = string.Format(CultureInfo.InvariantCulture, Resources.UnableToFindABlueprint, type);
            Log.Error(message);
            throw new BackstageException(message);
        }

        /// <summary>
        /// Scans the available assemblies for <see cref="IBlueprint"/> implementations.
        /// </summary>
        /// <returns>
        /// The scan result.
        /// </returns>
        private static IDictionary<Type, IBlueprint> GetBlueprints()
        {
            return TypeScanner.FindAndBuildImplementationsOf<IBlueprint>().ToDictionary(x => x.BuiltType);
        }
    }
}
