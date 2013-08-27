namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Common.Logging;

    /// <summary>
    /// Factory - use it to build valid objects. Useful for testing.
    /// Objects are defined using <see cref="IBlueprint{T}"/>.
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
    /// public class EmployeeBlueprint : IBlueprint<Employee>
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
            var blueprint = FoundBlueprint(type);
            var result = blueprint.Build(type);
            if (overrides != null)
            {
                overrides(result);
            }

            return result;
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
        /// The created object.
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
        private static IBlueprint FoundBlueprint(Type type)
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
        /// Scans the available assemblies for <see cref="IBlueprint{T}"/> implementations.
        /// </summary>
        /// <returns>
        /// The scan result.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "OK for assembly scanning")]
        private static IDictionary<Type, IBlueprint> GetBlueprints()
        {
            Log.Debug(Resources.ScanningAssembliesForBlueprints);
            var result = new Dictionary<Type, IBlueprint>();
            foreach (var assemblyFile in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory).Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyFile);
                    var blueprintTypes =
                        assembly.GetTypes()
                                .Where(
                                    x =>
                                    !x.IsAbstract && !x.IsInterface
                                    && typeof(IBlueprint).IsAssignableFrom(x));

                    foreach (var blueprintType in blueprintTypes)
                    {
                        var genericInterface =
                            blueprintType.GetInterfaces().FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IBlueprint<>));
                        if (genericInterface == null)
                        {
                            var message = string.Format(CultureInfo.InvariantCulture, Resources.UsingNonGenericIBlueprint, blueprintType);
                            Log.Error(message);
                            throw new BackstageException(message);
                        }

                        var objectType = genericInterface.GetGenericArguments().First();

                        try
                        {
                            var blueprint = (IBlueprint)Activator.CreateInstance(blueprintType);
                            result.Add(objectType, blueprint);
                        }
                        catch (Exception activatorException)
                        {
                            var message = string.Format(CultureInfo.InvariantCulture, Resources.ErrorWhileCreatingTypeDefaultConstructor, blueprintType);
                            Log.Error(message, activatorException);
                            throw new BackstageException(message, activatorException);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = string.Join(", ", ex.LoaderExceptions.Select(x => x.ToString()));
                    Log.Warn(string.Format(CultureInfo.InvariantCulture, Resources.ErrorWhileScanningAssembly, assemblyFile, loaderExceptions), ex);
                }
            }

            Log.Debug(string.Format(CultureInfo.InvariantCulture, Resources.ScannedAndFoundBlueprints, result.Count));
            return result;
        }
    }
}
