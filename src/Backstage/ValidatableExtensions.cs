namespace Backstage
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="IValidatable"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validatable", Justification = "Validatable is a word.")]
    public static class ValidatableExtensions
    {
        /// <summary>
        /// Returns true if the object is valid, false otherwise.
        /// </summary>
        /// <param name="validatable">
        /// The <see cref="IValidatable"/> object.
        /// </param>
        /// <returns>
        /// True if the object is valid, false otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "validatable", Justification = "Validatable is a word.")]
        public static bool IsValid(this IValidatable validatable)
        {
            return !validatable.Validate().Any();
        }

        /// <summary>
        /// Runs data annotations validations.
        /// </summary>
        /// <param name="validatable">
        /// The <see cref="IValidatable"/> object.
        /// </param>
        /// <returns>
        /// The validation results.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "validatable", Justification = "Validatable is a word.")]
        public static IEnumerable<ValidationResult> Validate(this IValidatable validatable)
        {
            if (validatable == null)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            var result = new Collection<ValidationResult>();
            Validator.TryValidateObject(
                validatable,
                new ValidationContext(validatable),
                result,
                true);
            return result;
        }

        /// <summary>
        /// Runs data annotations validations and raises an exception if the object is not valid.
        /// </summary>
        /// <param name="validatable">
        /// The <see cref="IValidatable"/> object.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "validatable", Justification = "Validatable is a word.")]
        public static void EnforceValidation(this IValidatable validatable)
        {
            if (validatable == null)
            {
                return;
            }

            Validator.ValidateObject(
                validatable,
                new ValidationContext(validatable),
                true);
        }
    }
}
