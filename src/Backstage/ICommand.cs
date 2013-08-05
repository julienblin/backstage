namespace Backstage
{
    /// <summary>
    /// A command with no result.
    /// A command allow the execution of code inside a context.
    /// Allows the encapsulation of common behaviors.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        void Execute(IContext context);
    }

    /// <summary>
    /// A command with result.
    /// A command allow the execution of code inside a context.
    /// Allows the encapsulation of common behaviors.
    /// </summary>
    /// <typeparam name="T">
    /// The type of result.
    /// </typeparam>
    public interface ICommand<out T>
    {
        /// <summary>
        /// Executes the command and return the result.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The result.
        /// </returns>
        T Execute(IContext context);
    }
}
