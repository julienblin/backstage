namespace Backstage.Web.Mvc
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Custom <see cref="IModelBinder"/> attribute that can load an entity from the current context,
    /// using a parameter as id key.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyController : Controller
    /// {
    ///     public ViewResult Details([GetFromContext]MyEntity theEntity)
    ///     {
    ///         //... theEntity will be populated with the result of Context.GetById([value of id parameter]);
    ///     }
    /// 
    ///     public ViewResult Details2([GetFromContext("id2", typeof(int))]MyEntity theEntity)
    ///     {
    ///         //... theEntity will be populated with the result of Context.GetById([value of id2 parameter as int]);
    ///     }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class GetFromContextAttribute : CustomModelBinderAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextAttribute"/> class,
        /// using <see cref="GetFromContextModelBinder.DefaultIdParameterName"/>.
        /// </summary>
        public GetFromContextAttribute()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextAttribute"/> class.
        /// </summary>
        /// <param name="idParameterName">
        /// The id parameter name.
        /// </param>
        public GetFromContextAttribute(string idParameterName)
            : this(idParameterName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextAttribute"/> class.
        /// </summary>
        /// <param name="idType">
        /// The type of the id parameter.
        /// </param>
        public GetFromContextAttribute(Type idType)
            : this(null, idType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFromContextAttribute"/> class.
        /// </summary>
        /// <param name="idParameterName">
        /// The id parameter name.
        /// </param>
        /// <param name="idType">
        /// The type of the id parameter.
        /// </param>
        public GetFromContextAttribute(string idParameterName, Type idType)
        {
            this.IdParameterName = idParameterName;
            this.IdType = idType;
        }

        /// <summary>
        /// Gets the id parameter name.
        /// </summary>
        public string IdParameterName { get; private set; }

        /// <summary>
        /// Gets the type of the id parameter.
        /// </summary>
        public Type IdType { get; private set; }

        /// <summary>
        /// Retrieves the associated model binder.
        /// </summary>
        /// <returns>
        /// A reference to an object that implements the <see cref="T:System.Web.Mvc.IModelBinder"/> interface.
        /// </returns>
        public override IModelBinder GetBinder()
        {
            return new GetFromContextModelBinder(this.IdParameterName, this.IdType);
        }
    }
}
