namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper class for generating random values.
    /// Uniform distribution is NOT a priority, nor this class should be considered
    /// cryptographically safe.
    /// It is useful for factories, <see cref="IBlueprint"/> and unit tests.
    /// </summary>
    public static class Rand
    {
        /// <summary>
        /// <c>lorem ipsum</c> sample.
        /// </summary>
        private const string LoremIpsumConst = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        /// <summary>
        /// The random generator.
        /// </summary>
        [ThreadStatic]
        private static Random random;

        /// <summary>
        /// Gets the random generator. Thread static.
        /// </summary>
        private static Random Random
        {
            get
            {
                return random ?? (random = new Random());
            }
        }

        /// <summary>
        /// Returns a random <see cref="bool"/> value.
        /// </summary>
        /// <returns>
        /// The generated <see cref="bool"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool", Justification = "OK here.")]
        public static bool Bool()
        {
            return Int32(1) == 1;
        }

        /// <summary>
        /// Returns a random <see cref="byte"/> value.
        /// </summary>
        /// <returns>
        /// The generated <see cref="byte"/>.
        /// </returns>
        public static byte Byte()
        {
            var result = new byte[1];
            Random.NextBytes(result);
            return result[0];
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes to contain random numbers.
        /// </param>
        public static void Bytes(byte[] buffer)
        {
            Random.NextBytes(buffer);
        }

        /// <summary>
        /// Returns a random <see cref="char"/> value.
        /// </summary>
        /// <returns>
        /// The generated <see cref="char"/>.
        /// </returns>
        public static char Char()
        {
            return (char)Int32(char.MaxValue);
        }

        /// <summary>
        /// Returns a random nonnegative <see cref="decimal"/> value.
        /// </summary>
        /// <returns>
        /// The generated <see cref="decimal"/>.
        /// </returns>
        public static decimal Decimal()
        {
            return Convert.ToDecimal(Double(0.0, Convert.ToDouble(decimal.MaxValue)));
        }

        /// <summary>
        /// Returns a random nonnegative <see cref="decimal"/> value
        /// less than the specified <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="decimal"/>.
        /// </returns>
        public static decimal Decimal(decimal maxValue)
        {
            return Convert.ToDecimal(Double(0.0, Convert.ToDouble(maxValue)));
        }

        /// <summary>
        /// Returns a random <see cref="decimal"/> within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="decimal"/>.
        /// </returns>
        public static decimal Decimal(decimal minValue, decimal maxValue)
        {
            return Convert.ToDecimal(Double(Convert.ToDouble(minValue), Convert.ToDouble(maxValue)));
        }

        /// <summary>
        /// Returns a random <see cref="double"/> between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The generated <see cref="double"/>.
        /// </returns>
        public static double Double()
        {
            return Random.NextDouble();
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="double"/>
        /// less than the specified <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="double"/>.
        /// </returns>
        public static double Double(double maxValue)
        {
            return Double(0.0, maxValue);
        }

        /// <summary>
        /// Returns a random <see cref="double"/> within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="double"/>.
        /// </returns>
        public static double Double(double minValue, double maxValue)
        {
            return (Random.NextDouble() * (maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Returns a random value chosen within a specified enumeration of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of enumeration to chose values from.
        /// </typeparam>
        /// <returns>
        /// The chosen value.
        /// </returns>
        public static T Pick<T>()
            where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new BackstageException(Resources.MustBeEnum.Format(typeof(T)));
            }

            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Next(values.Length));
        }

        /// <summary>
        /// Returns a random <see cref="float"/> between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// The generated <see cref="float"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float", Justification = "OK here.")]
        public static float Float()
        {
            return (float)Double(0.0, 1.0);
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="float"/>
        /// less than the specified <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="float"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float", Justification = "OK here.")]
        public static float Float(float maxValue)
        {
            return (float)Double(0.0, maxValue);
        }

        /// <summary>
        /// Returns a random <see cref="float"/> within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="float"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float", Justification = "OK here.")]
        public static float Float(float minValue, float maxValue)
        {
            return (float)Double(minValue, maxValue);
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="int"/>.
        /// </summary>
        /// <returns>
        /// The generated <see cref="int"/>.
        /// </returns>
        public static int Int32()
        {
            return Random.Next();
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="int"/>
        /// less than the specified <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="int"/>.
        /// </returns>
        public static int Int32(int maxValue)
        {
            return Random.Next(maxValue);
        }

        /// <summary>
        /// Returns a random <see cref="int"/> within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="int"/>.
        /// </returns>
        public static int Int32(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="long"/>.
        /// </summary>
        /// <returns>
        /// The generated <see cref="long"/>.
        /// </returns>
        public static long Int64()
        {
            return Int64(0, long.MaxValue);
        }

        /// <summary>
        /// Returns a nonnegative random <see cref="long"/>
        /// less than the specified <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="long"/>.
        /// </returns>
        public static long Int64(long maxValue)
        {
            return Int64(0, maxValue);
        }

        /// <summary>
        /// Returns a random <see cref="long"/> within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value.
        /// </param>
        /// <returns>
        /// The generated <see cref="long"/>.
        /// </returns>
        public static long Int64(long minValue, long maxValue)
        {
            var buffer = new byte[8];
            Random.NextBytes(buffer);
            var longRand = BitConverter.ToInt64(buffer, 0);

            return Math.Abs(longRand % (maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Returns a random string with a specified length.
        /// </summary>
        /// <param name="length">
        /// The length. Defaults to 8.
        /// </param>
        /// <returns>
        /// The generated <see cref="string"/>.
        /// </returns>
        public static string String(int length = 8)
        {
            var builder = new StringBuilder(length);
            while (builder.Length < length)
            {
                builder.Append(Path.GetRandomFileName().Replace(".", string.Empty));
            }

            return builder.ToString().Substring(0, length);
        }

        /// <summary>
        /// Returns a random <c>Lorem Ipsum</c> text with a specified length.
        /// </summary>
        /// <param name="length">
        /// The length. Defaults to 2000
        /// </param>
        /// <returns>
        /// The generated <see cref="string"/>.
        /// </returns>
        public static string Text(int length = 2000)
        {
            var builder = new StringBuilder(length);
            while (builder.Length < length)
            {
                builder.AppendLine(LoremIpsumConst);
            }

            return builder.ToString().Substring(0, length);
        }

        /// <summary>
        /// Returns a random value from the available <paramref name="values"/>.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <typeparam name="T">
        /// The type of elements
        /// </typeparam>
        /// <returns>
        /// The chosen value.
        /// </returns>
        public static T Pick<T>(IEnumerable<T> values)
        {
// ReSharper disable PossibleMultipleEnumeration
            values.ThrowIfNull("values");
            var index = Int32(0, values.Count());
            return values.ElementAt(index);
// ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Returns a random <see cref="Color"/>.
        /// </summary>
        /// <returns>
        /// The generated <see cref="Color"/>.
        /// </returns>
        public static Color Color()
        {
            return System.Drawing.Color.FromArgb(Int32(255), Int32(255), Int32(255));
        }

        /// <summary>
        /// Returns a random <see cref="System.DateTime"/> between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <param name="from">
        /// The minimum date.
        /// </param>
        /// <param name="to">
        /// The maximum date.
        /// </param>
        /// <returns>
        /// The generated <see cref="System.DateTime"/>.
        /// </returns>
        public static DateTime DateTime(DateTime from, DateTime to)
        {
            var timeSpan = to - from;
            return from.AddDays(Int32(1, (int)timeSpan.TotalDays - 1)).AddSeconds(Int32(1, 86400));
        }

        /// <summary>
        /// Returns a random <see cref="System.DateTime"/> between now - <paramref name="rangeOfDaysFromNow"/> and now + <paramref name="rangeOfDaysFromNow"/>.
        /// </summary>
        /// <param name="rangeOfDaysFromNow">
        /// The range of days to consider. The total range is <paramref name="rangeOfDaysFromNow"/> * 2.
        /// </param>
        /// <returns>
        /// The generated <see cref="System.DateTime"/>.
        /// </returns>
        public static DateTime DateTime(int rangeOfDaysFromNow)
        {
            return DateTime(System.DateTime.Now.AddDays(-1 * rangeOfDaysFromNow), System.DateTime.Now.AddDays(rangeOfDaysFromNow));
        }

        /// <summary>
        /// Returns a random <see cref="System.DateTime"/> between now - 5 days and now + 5 days.
        /// if <paramref name="onlyInTheFuture"/> is true, the range will be from now to now + 10 days.
        /// </summary>
        /// <param name="onlyInTheFuture">
        /// True to only generate dates in the future, false otherwise.
        /// </param>
        /// <returns>
        /// The generated <see cref="System.DateTime"/>.
        /// </returns>
        public static DateTime DateTime(bool onlyInTheFuture = false)
        {
            return onlyInTheFuture ? DateTime(System.DateTime.Now, System.DateTime.Now.AddDays(10)) : DateTime(5);
        }

        /// <summary>
        /// Returns a random <see cref="System.TimeSpan"/> less than an hour.
        /// </summary>
        /// <returns>
        /// The generated <see cref="System.TimeSpan"/>.
        /// </returns>
        public static TimeSpan TimeSpan()
        {
            return new TimeSpan(0, 0, Int32(59), Int32(59));
        }
    }
}
