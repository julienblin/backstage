namespace Backstage.Web
{
    /// <summary>
    /// The <see cref="IBackstageWebApplication"/> interface.
    /// A <see cref="System.Web.HttpApplication"/> that uses the <see cref="ContextPerRequestHttpModule"/>
    /// must implement this interface.
    /// </summary>
    public interface IBackstageWebApplication
    {
        /// <summary>
        /// Gets the <see cref="ContextFactoryConfiguration"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ContextFactoryConfiguration"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Marks the execution of code.")]
        ContextFactoryConfiguration GetContextFactoryConfiguration();
    }
}
