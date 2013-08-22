namespace Backstage
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Default <see cref="AuthorizationResult"/> implementation.
    /// </summary>
    public class DefaultAuthorizationResult : AuthorizationResult
    {
        /// <summary>
        /// The result.
        /// </summary>
        private bool result;

        /// <summary>
        /// The reasons.
        /// </summary>
        private IList<string> reasons;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationResult"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="operation">
        /// The evaluated Operation.
        /// </param>
        public DefaultAuthorizationResult(IUser user, string operation)
            : base(user, operation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationResult"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="operation">
        /// The evaluated Operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        public DefaultAuthorizationResult(IUser user, string operation, object target)
            : base(user, operation, target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAuthorizationResult"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="operation">
        /// The evaluated Operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        public DefaultAuthorizationResult(IUser user, string operation, object target, string field)
            : base(user, operation, target, field)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether result.
        /// </summary>
        public override bool Result
        {
            get
            {
                return this.result;
            }
        }

        /// <summary>
        /// Gets the reasons (debug information).
        /// </summary>
        public override IEnumerable<string> Reasons
        {
            get
            {
                return this.reasons ?? Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Sets the result.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetResult(bool value)
        {
            this.result = value;
        }

        /// <summary>
        /// Adds a reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void AddReason(string reason)
        {
            if (this.reasons == null)
            {
                this.reasons = new List<string> { reason };
            }
            else
            {
                this.reasons.Add(reason);
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
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}->{1}{2}{3}?: {4} ({5})]",
                this.User,
                this.Operation,
                this.Target != null ? " " + this.Target : string.Empty,
                this.Field != null ? "." + this.Field : string.Empty,
                this.Result ? "YES" : "NO",
                string.Join(", ", this.Reasons));
        }
    }
}
