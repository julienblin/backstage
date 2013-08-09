namespace Backstage
{
    /// <summary>
    /// Represents an anonymous user.
    /// </summary>
    public sealed class AnonymousUser : IUser
    {
        /// <summary>
        /// The unique instance.
        /// </summary>
        private static readonly AnonymousUser UniqueInstance = new AnonymousUser();

        /// <summary>
        /// Prevents a default instance of the <see cref="AnonymousUser"/> class from being created.
        /// </summary>
        private AnonymousUser()
        {
        }

        /// <summary>
        /// Gets the unique instance.
        /// </summary>
        public static AnonymousUser Instance
        {
            get
            {
                return UniqueInstance;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Resources.Anonymous;
        }
    }
}
