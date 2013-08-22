namespace Backstage.Web.Mvc.Tests
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class IsAuthorizedAttributeTest
    {
        private Mock<ISecurityProvider> securityProviderMock;
        private IContext context;

        [SetUp]
        public void SetUp()
        {
            var contextProviderFactoryMock = new Mock<IContextProviderFactory>();
            var contextProviderMock = new Mock<IContextProvider>();
            contextProviderFactoryMock.Setup(x => x.CreateContextProvider(It.IsAny<IContext>())).Returns(contextProviderMock.Object);

            this.securityProviderMock = new Mock<ISecurityProvider>();
            ContextFactory.StartNew(new ContextFactoryConfiguration(contextProviderFactoryMock.Object)
                                        {
                                            SecurityProvider = this.securityProviderMock.Object
                                        });
            this.context = ContextFactory.Current.StartNewContext();
            Context.Current = this.context;
        }

        [Test]
        public void It_should_authorize_operations()
        {
            var authResult = new DefaultAuthorizationResult(new Mock<IUser>().Object, "Foo");
            var isAuthAttr = new IsAuthorizedAttribute("Foo");
            this.securityProviderMock.Setup(x => x.GetAuthorizationResult(It.IsAny<IContext>(), "Foo"))
                .Returns(authResult);

            authResult.SetResult(true);
            var filterContext = new ActionExecutingContext();
            isAuthAttr.OnActionExecuting(filterContext);
            this.securityProviderMock.Verify(x => x.GetAuthorizationResult(Context.Current, "Foo"));
            filterContext.Result.Should().BeNull();

            authResult.SetResult(false);
            filterContext = new ActionExecutingContext();
            isAuthAttr.OnActionExecuting(filterContext);
            this.securityProviderMock.Verify(x => x.GetAuthorizationResult(Context.Current, "Foo"));
            filterContext.Result.Should().BeOfType<HttpUnauthorizedResult>();
        }

        [Test]
        public void It_should_authorize_operations_and_targets()
        {
            var target = new object();
            var authResult = new DefaultAuthorizationResult(new Mock<IUser>().Object, "Foo");
            var isAuthAttr = new IsAuthorizedAttribute("Foo", "theTarget");
            this.securityProviderMock.Setup(x => x.GetAuthorizationResult(It.IsAny<IContext>(), "Foo", It.IsAny<object>()))
                .Returns(authResult);

            authResult.SetResult(true);
            var filterContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object> { { "theTarget", target } }
            };
            isAuthAttr.OnActionExecuting(filterContext);
            this.securityProviderMock.Verify(x => x.GetAuthorizationResult(Context.Current, "Foo", target));
            filterContext.Result.Should().BeNull();

            authResult.SetResult(false);
            filterContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object> { { "theTarget", target } }
            };
            isAuthAttr.OnActionExecuting(filterContext);
            this.securityProviderMock.Verify(x => x.GetAuthorizationResult(Context.Current, "Foo", target));
            filterContext.Result.Should().BeOfType<HttpUnauthorizedResult>();

            authResult.SetResult(false);
            filterContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object>()
            };
            isAuthAttr.OnActionExecuting(filterContext);
            this.securityProviderMock.Verify(x => x.GetAuthorizationResult(Context.Current, "Foo", null));
            filterContext.Result.Should().BeOfType<HttpUnauthorizedResult>();
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
    }
}
