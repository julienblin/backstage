namespace Backstage.Sample.Core.Validators
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Validates that a string conforms to the IMO number requirements.
    /// see <a href="http://en.wikipedia.org/wiki/IMO_numbers#Assignment_and_Structure">Wikipedia</a>.
    /// </summary>
    public class IMONumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// The validation regex.
        /// </summary>
        private static readonly Regex ValidationRegex = new Regex(@"^IMO(?<digits>\d{6})(?<check>\d)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Determines whether the specified value of the object is valid. 
        /// </summary>
        /// <returns>
        /// true if the specified value is valid; otherwise, false.
        /// </returns>
        /// <param name="value">The value of the object to validate. </param>
        public override bool IsValid(object value)
        {
            var checkMatch = ValidationRegex.Match(value as string);
            if (!checkMatch.Success)
            {
                return false;
            }

            var digits = checkMatch.Groups["digits"].Value.Split().Select(x => Convert.ToInt32(x)).ToArray();
            var check = Convert.ToInt32(checkMatch.Groups["check"].Value);
            var total = 0;
            for (var i = 0; i < 6; i++)
            {
                total += digits[i] * (7 - i);
            }

            return check == (total % 10);
        }
    }
}
