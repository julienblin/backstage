namespace Backstage.Web
{
    using System;
    using System.Web;

    /// <summary>
    /// A hybrid <see cref="ICurrentContextHolder"/> that stores the current context in the current <see cref="HttpContext"/>.
    /// If an <see cref="HttpContext"/> is not available, falls back to a thread static holder.
    /// </summary>
    public class WebCurrentContextHolder : ICurrentContextHolder
    {
        /// <summary>
        /// The HttpContext key.
        /// </summary>
        private const string HttpContextKey = "Backstage.Web.WebCurrentContextHolder.CurrentContext";

        /// <summary>
        /// The current context with thread static storage.
        /// </summary>
        [ThreadStatic]
        private static IContext threadCurrentContext;

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <returns>
        /// The <see cref="IContext"/>.
        /// </returns>
        public IContext GetCurrentContext()
        {
            if (HttpContext.Current != null)
            {
                return (IContext)HttpContext.Current.Items[HttpContextKey];
            }

            return threadCurrentContext;
        }

        /// <summary>
        /// Sets the current context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void SetCurrentContext(IContext context)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[HttpContextKey] = context;
            }
            else
            {
                threadCurrentContext = context;
            }
        }
    }
}
