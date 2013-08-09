namespace Backstage
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the result of an authorization evaluation.
    /// This is an abstract class and not an interface to allow implicit conversions to <see cref="bool"/>.
    /// </summary>
    public abstract class AuthorizationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationResult"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="operation">
        /// The evaluated operation.
        /// </param>
        protected AuthorizationResult(IUser user, string operation)
            : this(user, operation, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationResult"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="operation">
        /// The evaluated operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        protected AuthorizationResult(IUser user, string operation, object target)
        {
            user.ThrowIfNull("user");
            operation.ThrowIfNull("operation");
            this.User = user;
            this.Operation = operation;
            this.Target = target;
        }

        /// <summary>
        /// Gets a value indicating whether the current user can perform the operation.
        /// </summary>
        public abstract bool Result { get; }

        /// <summary>
        /// Gets the user on whom this evaluation was performed.
        /// Can be different than the current user in case of delegation.
        /// </summary>
        public virtual IUser User { get; private set; }

        /// <summary>
        /// Gets the evaluated operation.
        /// </summary>
        public virtual string Operation { get; private set; }

        /// <summary>
        /// Gets the target, if any.
        /// </summary>
        public virtual object Target { get; private set; }

        /// <summary>
        /// Gets the reasons (debug information).
        /// </summary>
        public abstract IEnumerable<string> Reasons { get; }

        /// <summary>
        /// Implicit conversion to <see cref="bool"/>.
        /// </summary>
        /// <param name="authorizationResult">
        /// The authorization result.
        /// </param>
        /// <returns>
        /// <see cref="AuthorizationResult.Result"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Just a convenience evaluation here. ToBool / FromBool would be silly.")]
        public static implicit operator bool(AuthorizationResult authorizationResult)
        {
            return authorizationResult.Result;
        }
    }
}
