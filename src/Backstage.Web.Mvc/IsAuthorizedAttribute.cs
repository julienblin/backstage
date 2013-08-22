namespace Backstage.Web.Mvc
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// <see cref="ActionFilterAttribute"/> that uses the <see cref="Context.Current"/> IsAuthorized method.
    /// It is not a <see cref="AuthorizeAttribute"/> because we can reuse the model binding values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class IsAuthorizedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsAuthorizedAttribute"/> class.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        public IsAuthorizedAttribute(string operation)
            : this(operation, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAuthorizedAttribute"/> class.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="targetParameterName">
        /// The target parameter name.
        /// </param>
        /// <example>
        /// <code>
        /// public class MyController : Controller
        /// {
        ///     [IsAuthorized("Read", "theEntity")]
        ///     public ViewResult Details([GetFromContext]MyEntity theEntity)
        ///     {
        ///         // will succeed if Context.Current.IsAuthorized("Read", theEntity) is true.
        ///     }
        /// }
        /// </code>
        /// </example>
        public IsAuthorizedAttribute(string operation, string targetParameterName)
        {
            operation.ThrowIfNullOrEmpty("operation");
            this.Operation = operation;
            this.TargetParameterName = targetParameterName;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public string Operation { get; private set; }

        /// <summary>
        /// Gets the target parameter name.
        /// </summary>
        public string TargetParameterName { get; private set; }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AuthorizationResult authResult = null;
            if (string.IsNullOrEmpty(this.TargetParameterName))
            {
                authResult = Context.Current.IsAuthorized(this.Operation);
            }
            else
            {
                authResult = Context.Current.IsAuthorized(this.Operation, !filterContext.ActionParameters.ContainsKey(this.TargetParameterName) ? null : filterContext.ActionParameters[this.TargetParameterName]);
            }

            if (!authResult)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
