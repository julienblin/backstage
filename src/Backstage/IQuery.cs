namespace Backstage
{
    /// <summary>
    /// The Query interface.
    /// A query object allows the interrogation of the data repository.
    /// Concrete implementation and base types are provided by context providers.
    /// </summary>
    /// <typeparam name="T">
    /// The returned result type
    /// </typeparam>
    public interface IQuery<T>
    {
    }
}
