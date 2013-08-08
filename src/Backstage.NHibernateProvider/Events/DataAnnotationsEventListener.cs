namespace Backstage.NHibernateProvider.Events
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using NHibernate.Event;

    /// <summary>
    /// NHibernate event listener that validates data annotations.
    /// </summary>
    internal class DataAnnotationsEventListener : IPreInsertEventListener, IPreUpdateEventListener
    {
        /// <summary>
        /// On pre insert.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool OnPreInsert(PreInsertEvent @event)
        {
            Validator.ValidateObject(@event.Entity, new ValidationContext(@event.Entity, null, new Dictionary<object, object> { { "NHibernateProvider", true } }), true);
            return false;
        }

        /// <summary>
        /// On pre update.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validator.ValidateObject(@event.Entity, new ValidationContext(@event.Entity, null, new Dictionary<object, object> { { "NHibernateProvider", true } }), true);
            return false;
        }
    }
}
