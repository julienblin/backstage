namespace Backstage.Tests
{
    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class NoSecurityProviderTest
    {
        [Test]
        public void It_should_return_AnonymousUser()
        {
            var context = new Mock<IContext>().Object;
            var provider = new NoSecurityProvider();
            provider.GetCurrentUser(context).Should().Be(AnonymousUser.Instance);
        }

        [Test]
        public void It_should_authorize_all_operations()
        {
            var context = new Mock<IContext>().Object;
            var provider = new NoSecurityProvider();
            var result = provider.GetAuthorizationResult(context, "foo");
            result.Result.Should().Be(true);
            result.Operation.Should().Be("foo");
            result.User.Should().Be(AnonymousUser.Instance);
            result.ToString().Should().Contain("No security provider");
        }

        [Test]
        public void It_should_authorize_all_operations_on_all_targets()
        {
            var context = new Mock<IContext>().Object;
            var provider = new NoSecurityProvider();
            var result = provider.GetAuthorizationResult(context, "foo", provider);
            result.Result.Should().Be(true);
            result.Operation.Should().Be("foo");
            result.Target.Should().Be(provider);
            result.User.Should().Be(AnonymousUser.Instance);
            result.ToString().Should().Contain("No security provider");
        }
    }
}
