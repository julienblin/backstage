namespace Backstage.Web.Mvc.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// <see cref="HtmlHelper"/> extension methods for <see cref="IPaginationResult"/>.
    /// </summary>
    public static class Pager
    {
        /// <summary>
        /// The page count threshold under which all links will be rendered.
        /// </summary>
        private const int ThresholdForAllLinks = 8;

        /// <summary>
        /// The default page property name.
        /// </summary>
        private const string DefaultPagePropertyName = "page";

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <typeparam name="T">
        /// The type of the model. Must be <see cref="IPaginationResult"/>.
        /// </typeparam>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks<T>(this HtmlHelper<T> htmlHelper)
            where T : IPaginationResult
        {
            return PageLinks(htmlHelper, htmlHelper.ViewData.Model, DefaultPagePropertyName, new RouteValueDictionary());
        }

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        /// <typeparam name="T">
        /// The type of the model. Must be <see cref="IPaginationResult"/>.
        /// </typeparam>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks<T>(this HtmlHelper<T> htmlHelper, string pageProperty)
            where T : IPaginationResult
        {
            return PageLinks(htmlHelper, htmlHelper.ViewData.Model, pageProperty, new RouteValueDictionary());
        }

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes to set for the element.
        /// </param>
        /// <typeparam name="T">
        /// The type of the model. Must be <see cref="IPaginationResult"/>.
        /// </typeparam>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks<T>(this HtmlHelper<T> htmlHelper, string pageProperty, object htmlAttributes)
            where T : IPaginationResult
        {
            return PageLinks(htmlHelper, htmlHelper.ViewData.Model, pageProperty, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="result">
        /// The result to paginate.
        /// </param>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks(this HtmlHelper htmlHelper, IPaginationResult result)
        {
            return PageLinks(htmlHelper, result, DefaultPagePropertyName, new RouteValueDictionary());
        }

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="result">
        /// The result to paginate.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes to set for the element.
        /// </param>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks(this HtmlHelper htmlHelper, IPaginationResult result, object htmlAttributes)
        {
            return PageLinks(htmlHelper, result, DefaultPagePropertyName, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>
        /// Generates pagination links using <a href="http://getbootstrap.com/">Bootstrap</a> pagination tag format.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="result">
        /// The result to paginate.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        /// <param name="htmlAttributes">
        /// An object that contains the HTML attributes to set for the element.
        /// </param>
        /// <returns>
        /// The pagination html.
        /// </returns>
        public static IHtmlString PageLinks(
            this HtmlHelper htmlHelper, 
            IPaginationResult result,
            string pageProperty,
            IDictionary<string, object> htmlAttributes)
        {
            htmlHelper.ThrowIfNull("htmlHelper");
            result.ThrowIfNull("result");
            htmlAttributes.ThrowIfNull("htmlAttributes");

            if (result.PageCount < 2)
            {
                return null;
            }

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.MergeAttributes(htmlAttributes);

            if (result.PageCount < ThresholdForAllLinks)
            {
                RenderAllLinks(htmlHelper, result, ul, pageProperty);
            }
            else
            {
                RenderSummaryLinks(htmlHelper, result, ul, pageProperty);
            }

            return new HtmlString(ul.ToString());
        }

        /// <summary>
        /// Renders all the links.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html Helper.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="baseTag">
        /// The base tag.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        private static void RenderAllLinks(
            HtmlHelper htmlHelper,
            IPaginationResult result,
            TagBuilder baseTag,
            string pageProperty)
        {
            for (var i = 1; i <= result.PageCount; ++i)
            {
                var li = new TagBuilder("li");
                if (i == result.Page)
                {
                    li.AddCssClass("active");
                }

                li.InnerHtml = GeneratePageLink(htmlHelper, i.ToString(CultureInfo.CurrentCulture), i, pageProperty);
                baseTag.InnerHtml += li;
            }
        }

        /// <summary>
        /// Renders a summary of all the links.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html Helper.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="baseTag">
        /// The base tag.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Triggered by &lt; etc. links.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Triggered by &lt; etc. links.")]
        private static void RenderSummaryLinks(
            HtmlHelper htmlHelper,
            IPaginationResult result,
            TagBuilder baseTag,
            string pageProperty)
        {
            if (result.HasPreviousPage)
            {
                baseTag.InnerHtml += new TagBuilder("li") { InnerHtml = GeneratePageLink(htmlHelper, "&lt;&lt;", 1, pageProperty) };
            }
            else
            {
                baseTag.InnerHtml += GenerateDisabledLi("&lt;&lt;");
            }

            if (result.HasPreviousPage)
            {
                baseTag.InnerHtml += new TagBuilder("li") { InnerHtml = GeneratePageLink(htmlHelper, "&lt;", result.Page - 1, pageProperty) };
            }
            else
            {
                baseTag.InnerHtml += GenerateDisabledLi("&lt;");
            }

            int lastPageNumber, firstPageNumber;
            ComputeFirstAndLastPage(result, out firstPageNumber, out lastPageNumber);

            for (var i = firstPageNumber; i <= lastPageNumber; i++)
            {
                var li = new TagBuilder("li");
                if (i == result.Page)
                {
                    li.AddCssClass("active");
                    li.InnerHtml = GeneratePageLink(htmlHelper, string.Format(CultureInfo.CurrentCulture, "{0} / {1}", i, result.PageCount), i, pageProperty);
                }
                else
                {
                    li.InnerHtml = GeneratePageLink(htmlHelper, i.ToString(CultureInfo.CurrentCulture), i, pageProperty);
                }

                baseTag.InnerHtml += li.ToString();
            }

            if (result.HasNextPage)
            {
                baseTag.InnerHtml += new TagBuilder("li") { InnerHtml = GeneratePageLink(htmlHelper, "&gt;", result.Page + 1, pageProperty) };
            }
            else
            {
                baseTag.InnerHtml += GenerateDisabledLi("&gt;");
            }

            if (result.HasNextPage)
            {
                baseTag.InnerHtml += new TagBuilder("li") { InnerHtml = GeneratePageLink(htmlHelper, "&gt;&gt;", result.PageCount, pageProperty) };
            }
            else
            {
                baseTag.InnerHtml += GenerateDisabledLi("&gt;&gt;");
            }
        }

        /// <summary>
        /// Compute the first and last page in case of summary links.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="firstPageNumber">
        /// The first page number.
        /// </param>
        /// <param name="lastPageNumber">
        /// The last page number.
        /// </param>
        private static void ComputeFirstAndLastPage(IPaginationResult result, out int firstPageNumber, out int lastPageNumber)
        {
            firstPageNumber = 1;
            if (result.Page > 2)
            {
                firstPageNumber = result.Page - 1;
                if (result.Page == result.PageCount)
                {
                    firstPageNumber -= 1;
                }
            }

            lastPageNumber = result.PageCount;
            if (result.Page < (result.PageCount - 1))
            {
                lastPageNumber = result.Page + 1;
                if (result.Page == 1)
                {
                    lastPageNumber += 1;
                }
            }
        }

        /// <summary>
        /// Generate a disabled li.
        /// </summary>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GenerateDisabledLi(string linkText)
        {
            var tag = new TagBuilder("li")
            {
                InnerHtml = string.Format(CultureInfo.CurrentCulture, "<a href='#'>{0}</a>", linkText)
            };
            tag.AddCssClass("disabled");
            return tag.ToString();
        }

        /// <summary>
        /// Generates a page link.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html Helper.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="page">
        /// The page number.
        /// </param>
        /// <param name="pageProperty">
        /// The page property name.
        /// </param>
        /// <returns>
        /// The link.
        /// </returns>
        private static string GeneratePageLink(HtmlHelper htmlHelper, string text, int page, string pageProperty)
        {
            htmlHelper.ViewContext.RequestContext.HttpContext.ThrowIfNull("htmlHelper.ViewContext.RequestContext.HttpContext");
            var uriBuilder = new UriBuilder(htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url);
            var queryStringValues = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryStringValues[pageProperty] = page.ToString(CultureInfo.InvariantCulture);
            uriBuilder.Query = queryStringValues.ToString();

            var a = new TagBuilder("a");
            a.MergeAttribute("href", uriBuilder.ToString());
            a.InnerHtml = text;
            return a.ToString();
        }
    }
}
