namespace Backstage.Tests
{
    using System.Transactions;

    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultContextTest
    {
        private Mock<IContextProviderFactory> contextProviderFactoryMock;

        private Mock<IContextProvider> contextProviderMock;

        private Mock<IUser> userMock;

        private Mock<ISecurityProvider> securityProviderMock;
            
        [SetUp]
        public void SetUp()
        {
            this.contextProviderFactoryMock = new Mock<IContextProviderFactory>();
            this.contextProviderMock = new Mock<IContextProvider>();
            this.contextProviderFactoryMock.Setup(x => x.CreateContextProvider(It.IsAny<IContext>())).Returns(this.contextProviderMock.Object);

            this.securityProviderMock = new Mock<ISecurityProvider>();
            this.userMock = new Mock<IUser>();
            this.securityProviderMock.Setup(x => x.GetCurrentUser(It.IsAny<IContext>())).Returns(this.userMock.Object);

            ContextFactory.StartNew(new ContextFactoryConfiguration(this.contextProviderFactoryMock.Object)
                                        {
                                            SecurityProvider = this.securityProviderMock.Object
                                        });
        }

        [Test]
        public void It_should_relay_calls_to_context_provider()
        {
            var entity = new Mock<IEntity>().Object;
            var query = new Mock<IQuery<IEntity>>().Object;

            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Add(entity);
                this.contextProviderMock.Verify(x => x.Add(entity));

                context.Remove(entity);
                this.contextProviderMock.Verify(x => x.Remove(entity));

                context.Reload(entity);
                this.contextProviderMock.Verify(x => x.Reload(entity));

                this.contextProviderMock.Setup(x => x.GetById<IEntity>(5)).Returns(entity);
                context.GetById<IEntity>(5).Should().Be(entity);
                this.contextProviderMock.Verify(x => x.GetById<IEntity>(5));

                this.contextProviderMock.Setup(x => x.Fulfill(query)).Returns(entity);
                context.Fulfill(query).Should().Be(entity);
                this.contextProviderMock.Verify(x => x.Fulfill(query));

                context.Commit();
            }
        }

        [Test]
        public void It_should_clone()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.Values["Foo"] = "Bar";

                using (var newContext = context.Clone())
                {
                    newContext.Id.Should().NotBe(context.Id);
                    newContext.Parent.Should().Be(context);
                    newContext.IsReady.Should().BeFalse();
                    newContext.ContextFactory.Should().Be(ContextFactory.Current);
                    newContext.Values["Foo"].Should().Be("Bar");

                    newContext.Start();
                    this.contextProviderFactoryMock.Verify(x => x.CreateContextProvider(newContext));
                }
            }
        }

        [Test]
        public void It_should_not_accept_operations_if_not_started()
        {
            var entity = new Mock<IEntity>().Object;
            var query = new Mock<IQuery<IEntity>>().Object;
            var command = new Mock<ICommand>().Object;
            var commandResult = new Mock<ICommand<IEntity>>().Object;

            using (var parentContext = ContextFactory.Current.StartNewContext())
            {
                var context = parentContext.Clone();

                context.IsReady.Should().BeFalse();

                context.Invoking(x => x.Add(entity))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.Remove(entity))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.Reload(entity))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.GetById<IEntity>(5))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.Fulfill<IEntity>(query))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.Execute(command))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.Execute(commandResult))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.ExecuteAsync(command))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);

                context.Invoking(x => x.ExecuteAsync(commandResult))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("ready", ComparisonMode.Substring);
            }
        }

        [Test]
        public void It_should_not_change_transaction_parameters()
        {
            using (var parentContext = ContextFactory.Current.StartNewContext())
            {
                var context = parentContext.Clone();
                context.Invoking(x => x.Start(TransactionScopeOption.RequiresNew, new TransactionOptions()))
                       .ShouldThrow<BackstageException>()
                       .WithMessage("dependent", ComparisonMode.Substring);
            }
        }

        [Test]
        public void It_should_execute_command_async()
        {
            var commandMock = new Mock<ICommand>();
            var commandResultMock = new Mock<ICommand<object>>();

            using (var context = ContextFactory.Current.StartNewContext())
            {
                var task = context.ExecuteAsync(commandMock.Object);
                task.Wait();

                commandMock.Verify(x => x.Execute(It.Is<IContext>(c => c.Parent == context)));

                var returnValue = new object();
                commandResultMock.Setup(x => x.Execute(It.IsAny<IContext>())).Returns(returnValue);
                var taskResult = context.ExecuteAsync(commandResultMock.Object);
                taskResult.Wait();
                taskResult.Result.Should().Be(returnValue);

                commandMock.Verify(x => x.Execute(It.Is<IContext>(c => c.Parent == context)));
            }
        }

        [Test]
        public void It_should_raise_events()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.MonitorEvents();
                context.Commit();

                context.ShouldRaise("TransactionCommitting")
                       .WithSender(context)
                       .WithArgs<TransactionCommittingEventArgs>(x => x.Transaction != null);

                context.ShouldRaise("TransactionCommitted")
                       .WithSender(context)
                       .WithArgs<TransactionCommittedEventArgs>(x => x.Transaction != null);
            }
        }

        [Test]
        public void It_should_relay_calls_to_security_provider()
        {
            using (var context = ContextFactory.Current.StartNewContext())
            {
                context.CurrentUser.Should().Be(this.userMock.Object);
                this.securityProviderMock.Verify(x => x.GetCurrentUser(context));

                context.IsAuthorized("foo");
                this.securityProviderMock.Verify(x => x.GetAuthorizationResult(context, "foo"));

                var target = new object();
                context.IsAuthorized("foo", target);
                this.securityProviderMock.Verify(x => x.GetAuthorizationResult(context, "foo", target));

                context.IsAuthorized("foo", target, "bar");
                this.securityProviderMock.Verify(x => x.GetAuthorizationResult(context, "foo", target, "bar"));
            }
        }
    }
}
