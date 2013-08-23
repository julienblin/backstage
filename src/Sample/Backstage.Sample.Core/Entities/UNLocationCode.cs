namespace Backstage.Sample.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// United nations location code.
    /// <a href="http://en.wikipedia.org/wiki/UN/LOCODE">See here for more information</a>.
    /// </summary>
    public class UNLocationCode
    {
        /// <summary>
        /// Gets or sets the ISO 3166-1 alpha-2 country code.
        /// <a href="http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2">See here for more information</a>.
        /// </summary>
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public virtual string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the location code.
        /// </summary>
        [Required]
        [StringLength(2)]
        public virtual string LocationCode { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", this.CountryCode, this.LocationCode);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var other = obj as UNLocationCode;
            if (other == null)
            {
                return false;
            }

            return this.CountryCode.Equals(other.CountryCode, StringComparison.InvariantCultureIgnoreCase)
                && this.LocationCode.Equals(other.LocationCode, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return HashCode.Generate(this.CountryCode, this.LocationCode);
        } 
    }
}
