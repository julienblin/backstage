namespace Backstage.Web.Mvc.Tests.Helpers
{
    using Backstage.Web.Mvc.Helpers;

    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class PagerTest
    {
        [Test]
        public void It_should_render_null_when_no_page()
        {
            var result = new PaginationResult { TotalItems = 5, PageSize = 10, Page = 1 };
            TestHelper.CreateHtmlHelper("http://www.example.org?page=1").PageLinks(result).Should().BeNull();
        }

        [Test]
        public void It_should_render_all_links_when_small_number_of_pages()
        {
            var result = new PaginationResult { TotalItems = 18, PageSize = 5, Page = 2 };
            TestHelper.CreateHtmlHelper("http://www.example.org?page=2").PageLinks(result).ToString().Should()
                      .Be("<ul class=\"pagination\"><li><a href=\"http://www.example.org:80/?page=1\">1</a></li><li class=\"active\"><a href=\"http://www.example.org:80/?page=2\">2</a></li><li><a href=\"http://www.example.org:80/?page=3\">3</a></li><li><a href=\"http://www.example.org:80/?page=4\">4</a></li></ul>");
        }

        [Test]
        public void It_should_render_summary_links_when_large_number_of_pages()
        {
            var result = new PaginationResult { TotalItems = 50, PageSize = 5, Page = 6 };
            TestHelper.CreateHtmlHelper("http://www.example.org?page=6").PageLinks(result).ToString().Should()
                      .Be("<ul class=\"pagination\"><li><a href=\"http://www.example.org:80/?page=1\">&lt;&lt;</a></li><li><a href=\"http://www.example.org:80/?page=5\">&lt;</a></li><li><a href=\"http://www.example.org:80/?page=5\">5</a></li><li class=\"active\"><a href=\"http://www.example.org:80/?page=6\">6 / 10</a></li><li><a href=\"http://www.example.org:80/?page=7\">7</a></li><li><a href=\"http://www.example.org:80/?page=7\">&gt;</a></li><li><a href=\"http://www.example.org:80/?page=10\">&gt;&gt;</a></li></ul>");
        }
    }
}
