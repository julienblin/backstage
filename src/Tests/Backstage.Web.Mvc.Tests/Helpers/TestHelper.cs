namespace Backstage.Web.Mvc.Tests.Helpers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Moq;

    public static class TestHelper
    {
        public static HtmlHelper CreateHtmlHelper()
        {
            return CreateHtmlHelper(new ViewDataDictionary());
        }

        public static HtmlHelper CreateHtmlHelper(ViewDataDictionary viewData)
        {
            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(viewData);

            var viewContext = new ViewContext { ViewData = viewData };

            return new HtmlHelper(viewContext, mockViewDataContainer.Object);
        }

        public static HtmlHelper CreateHtmlHelper(string url)
        {
            var viewData = new ViewDataDictionary();
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.Url).Returns(new Uri(url));
            
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(x => x.Request).Returns(request.Object);

            var mockViewDataContainer = new Mock<IViewDataContainer>();
            mockViewDataContainer.Setup(v => v.ViewData).Returns(viewData);

            var viewContext = new ViewContext { ViewData = viewData, RequestContext = new RequestContext(httpContext.Object, new RouteData()) };

            return new HtmlHelper(viewContext, mockViewDataContainer.Object);
        }
    }
}
