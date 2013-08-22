namespace Backstage.Web.Mvc
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;

    using Common.Logging;

    /// <summary>
    /// <see cref="IModelBinder"/> that loads entities from the context based on ids.
    /// </summary>
    public class GetFromContextModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// The default id parameter name - id.
        /// </summary>
        public const string DefaultIdParameterName = "Id";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The id parameter name.
        /// </summary>
        private readonly string idParameterName;

        /// <summary>
        /// The type of the id parameter.
        /// </summary>
        private readonly Type idType;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextModelBinder"/> class.
        /// </summary>
        public GetFromContextModelBinder()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextModelBinder"/> class.
        /// </summary>
        /// <param name="idParameterName">
        /// The id parameter name.
        /// </param>
        public GetFromContextModelBinder(string idParameterName)
            : this(idParameterName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextModelBinder"/> class.
        /// </summary>
        /// <param name="idType">
        /// The type of the id parameter.
        /// </param>
        public GetFromContextModelBinder(Type idType)
            : this(null, idType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextModelBinder"/> class.
        /// </summary>
        /// <param name="idParameterName">
        /// The id parameter name.
        /// </param>
        /// <param name="idType">
        /// The type of the id parameter.
        /// </param>
        public GetFromContextModelBinder(string idParameterName, Type idType)
        {
            this.idParameterName = idParameterName ?? DefaultIdParameterName;
            this.idType = idType;
        }

        /// <summary>
        /// Binds the model by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound object.
        /// </returns>
        /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param><exception cref="T:System.ArgumentNullException">The <paramref name="bindingContext "/>parameter is null.</exception>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var entityType = bindingContext.ModelType;
            var entityIdValueResult = bindingContext.ValueProvider.GetValue(this.idParameterName);
            if (entityIdValueResult == null)
            {
                var message = string.Format(CultureInfo.InvariantCulture, Resources.NoValueForIdParameter, this.idParameterName);
                Log.Warn(message);
                throw new BackstageException(message);
            }

            return Context.Current.GetById(entityType, this.idType == null ? entityIdValueResult.AttemptedValue : entityIdValueResult.ConvertTo(this.idType, CultureInfo.CurrentCulture));
        }
    }
}
