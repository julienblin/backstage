namespace Backstage
{
    /// <summary>
    /// An entity.
    /// An entity is a unit that can be persisted by an <see cref="IContextProvider"/> through an <see cref="IContext"/>.
    /// </summary>
    public interface IEntity : IValidatable
    {
    }
}
