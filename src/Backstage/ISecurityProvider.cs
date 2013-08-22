namespace Backstage
{
    /// <summary>
    /// Provides all the security information.
    /// </summary>
    public interface ISecurityProvider
    {
        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <param name="context">
        /// The callable context.
        /// </param>
        /// <returns>
        /// The <see cref="IUser"/>.
        /// </returns>
        IUser GetCurrentUser(IContext context);

        /// <summary>
        /// Gets the <see cref="AuthorizationResult"/> for the current context and current user.
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
        AuthorizationResult GetAuthorizationResult(IContext context, string operation);

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
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        AuthorizationResult GetAuthorizationResult(IContext context, string operation, object target);

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
        AuthorizationResult GetAuthorizationResult(IContext context, string operation, object target, string field);
    }
}
