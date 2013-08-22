namespace Backstage
{
    /// <summary>
    /// <see cref="ISecurityProvider"/> that provides no security.
    /// All operations are allowed, and the user is always <see cref="AnonymousUser"/>.
    /// This is the default <see cref="ISecurityProvider"/> if no other is specified.
    /// </summary>
    public class NoSecurityProvider : ISecurityProvider
    {
        /// <summary>
        /// Gets the current user.
        /// Always returns <see cref="AnonymousUser"/>.
        /// </summary>
        /// <param name="context">
        /// The callable context.
        /// </param>
        /// <returns>
        /// The <see cref="IUser"/>.
        /// </returns>
        public IUser GetCurrentUser(IContext context)
        {
            return AnonymousUser.Instance;
        }

        /// <summary>
        /// Gets the <see cref="AuthorizationResult"/> for the current context and current user.
        /// Always returns a positive authorization.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult GetAuthorizationResult(IContext context, string operation)
        {
            var result = new DefaultAuthorizationResult(this.GetCurrentUser(context), operation);
            result.SetResult(true);
            result.AddReason(Resources.UsingDefaultNoSecurityProvider);
            return result;
        }

        /// <summary>
        /// Gets the <see cref="AuthorizationResult"/> for the current context and current user.
        /// Always returns a positive authorization.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult GetAuthorizationResult(IContext context, string operation, object target)
        {
            var result = new DefaultAuthorizationResult(this.GetCurrentUser(context), operation, target);
            result.SetResult(true);
            result.AddReason(Resources.UsingDefaultNoSecurityProvider);
            return result;
        }

        /// <summary>
        /// Gets the <see cref="AuthorizationResult"/> for the current context and current user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult GetAuthorizationResult(IContext context, string operation, object target, string field)
        {
            var result = new DefaultAuthorizationResult(this.GetCurrentUser(context), operation, target, field);
            result.SetResult(true);
            result.AddReason(Resources.UsingDefaultNoSecurityProvider);
            return result;
        }
    }
}
