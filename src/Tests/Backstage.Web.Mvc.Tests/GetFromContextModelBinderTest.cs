namespace Backstage.Web.Mvc.Tests
{
    using System.Collections.Specialized;
    using System.Web.Mvc;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class GetFromContextModelBinderTest
    {
        private Mock<IContextProvider> contextProviderMock;
        private IContext context;

        [SetUp]
        public void SetUp()
        {
            var contextProviderFactoryMock = new Mock<IContextProviderFactory>();
            this.contextProviderMock = new Mock<IContextProvider>();
            contextProviderFactoryMock.Setup(x => x.CreateContextProvider(It.IsAny<IContext>())).Returns(this.contextProviderMock.Object);
            ContextFactory.StartNew(new ContextFactoryConfiguration(contextProviderFactoryMock.Object));
            this.context = ContextFactory.Current.StartNewContext();
            Context.Current = this.context;
        }

        [TearDown]
        public void TearDown()
        {
            if (this.context != null)
            {
                this.context.Dispose();
            }

            if (ContextFactory.HasCurrent)
            {
                ContextFactory.Current.Dispose();
            }
        }

        [Test]
        [TestCase("id", null)]
        [TestCase("foo", "foo")]
        public void It_should_call_current_context_GetById(string idParameterNameInput, string idParameterNameModelBinder)
        {
            var formCollection = new NameValueCollection { { idParameterNameInput, "2" } };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TheEntity));

            var bindingContext = new ModelBindingContext
            {
                ModelName = string.Empty,
                ValueProvider = valueProvider,
                ModelMetadata = modelMetadata
            };

            var modelBinder = new GetFromContextModelBinder(idParameterNameModelBinder);
            var controllerContext = new ControllerContext();

            var returnedObject = new TheEntity();
            this.contextProviderMock.Setup(x => x.GetById(typeof(TheEntity), "2")).Returns(returnedObject);

            var result = modelBinder.BindModel(controllerContext, bindingContext);

            result.Should().Be(returnedObject);
        }

        [Test]
        [TestCase("id", null)]
        [TestCase("foo", "foo")]
        public void It_should_call_current_context_GetById_and_convert_type(string idParameterNameInput, string idParameterNameModelBinder)
        {
            var formCollection = new NameValueCollection { { idParameterNameInput, "2" } };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TheEntity));

            var bindingContext = new ModelBindingContext
            {
                ModelName = string.Empty,
                ValueProvider = valueProvider,
                ModelMetadata = modelMetadata
            };

            var modelBinder = new GetFromContextModelBinder(idParameterNameModelBinder, typeof(int));
            var controllerContext = new ControllerContext();

            var returnedObject = new TheEntity();
            this.contextProviderMock.Setup(x => x.GetById(typeof(TheEntity), 2)).Returns(returnedObject);

            var result = modelBinder.BindModel(controllerContext, bindingContext);

            result.Should().Be(returnedObject);
        }

        [Test]
        public void It_should_throw_if_no_value_found()
        {
            var formCollection = new NameValueCollection { { "foo", "2" } };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);
            var modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TheEntity));

            var bindingContext = new ModelBindingContext
            {
                ModelName = string.Empty,
                ValueProvider = valueProvider,
                ModelMetadata = modelMetadata
            };

            var modelBinder = new GetFromContextModelBinder();
            var controllerContext = new ControllerContext();

            modelBinder.Invoking(x => x.BindModel(controllerContext, bindingContext))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("*id*");
        }

        private class TheEntity : IEntity
        {
        }
    }
}
