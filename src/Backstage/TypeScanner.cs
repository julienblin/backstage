namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Common.Logging;

    /// <summary>
    /// Helper class than scans available assemblies and found types.
    /// </summary>
    public static class TypeScanner
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Find concrete implementations of <typeparamref name="T"/> in the
        /// current available assemblies (referenced by <see cref="AppDomain.CurrentDomain"/> location).
        /// </summary>
        /// <typeparam name="T">
        /// The base type to look for.
        /// </typeparam>
        /// <returns>
        /// The found types.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Non-generic method exists.")]
        public static IEnumerable<Type> FindConcreteImplementationsOf<T>()
        {
            return FindConcreteImplementationsOf(typeof(T));
        }

        /// <summary>
        /// Find concrete implementations of <paramref name="baseType"/> in the
        /// current available assemblies (referenced by <see cref="AppDomain.CurrentDomain"/> location).
        /// </summary>
        /// <param name="baseType">
        /// The base type to look for.
        /// </param>
        /// <returns>
        /// The found types.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "OK here.")]
        public static IEnumerable<Type> FindConcreteImplementationsOf(Type baseType)
        {
            var result = new List<Type>();
            foreach (
                var assemblyFile in
                    Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory)
                             .Where(
                                 x =>
                                 x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                 || x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyFile);
                    if (baseType.IsInterface && baseType.IsGenericTypeDefinition)
                    {
                        result.AddRange(assembly.GetTypes()
                                            .Where(
                                                x =>
                                                !x.IsAbstract && !x.IsInterface
                                                && (x.GetInterface(baseType.FullName) != null)));
                    }
                    else
                    {
                        result.AddRange(assembly.GetTypes()
                                            .Where(
                                                x =>
                                                !x.IsAbstract && !x.IsInterface
                                                && baseType.IsAssignableFrom(x)));
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = string.Join(", ", ex.LoaderExceptions.Select(x => x.ToString()));
                    Log.Warn(string.Format(CultureInfo.InvariantCulture, Resources.ErrorWhileScanningAssembly, assemblyFile, loaderExceptions), ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Find concrete implementations of <typeparamref name="T"/> in the
        /// current available assemblies (referenced by <see cref="AppDomain.CurrentDomain"/> location),
        /// and build them using the default constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The base type.
        /// </typeparam>
        /// <returns>
        /// The built objects.
        /// </returns>
        public static IEnumerable<T> FindAndBuildImplementationsOf<T>()
        {
            return FindAndBuildImplementationsOf(typeof(T)).Select(x => (T)x);
        }

        /// <summary>
        /// Find concrete implementations of <paramref name="baseType"/> in the
        /// current available assemblies (referenced by <see cref="AppDomain.CurrentDomain"/> location),
        /// and build them using the default constructor.
        /// </summary>
        /// <param name="baseType">
        /// The base type.
        /// </param>
        /// <returns>
        /// The built objects.
        /// </returns>
        public static IEnumerable<object> FindAndBuildImplementationsOf(Type baseType)
        {
            var types = FindConcreteImplementationsOf(baseType);
            var result = new List<object>();
            foreach (var type in types)
            {
                try
                {
                    result.Add(Activator.CreateInstance(type));
                }
                catch (Exception ex)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, Resources.ErrorWhileCreatingTypeDefaultConstructor, type);
                    Log.Error(message, ex);
                    throw new BackstageException(message, ex);
                }
            }

            return result;
        }
    }
}
