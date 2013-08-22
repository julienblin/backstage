namespace Backstage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Transactions;

    using Common.Logging;

    /// <summary>
    /// <see cref="IContext"/> implementation.
    /// </summary>
    internal class DefaultContext : IContext
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The context providers.
        /// </summary>
        private IContextProvider contextProvider;

        /// <summary>
        /// The transaction scope.
        /// </summary>
        private TransactionScope transactionScope;

        /// <summary>
        /// The dependent transaction.
        /// </summary>
        private DependentTransaction dependentTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContext"/> class.
        /// </summary>
        /// <param name="contextFactory">
        /// The context factory.
        /// </param>
        public DefaultContext(IContextFactory contextFactory)
        {
            this.Id = Guid.NewGuid();
            this.IsReady = false;
            this.ContextFactory = contextFactory;
            this.Values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContext"/> class by copy.
        /// Copies the Values and ContextFactory - everything else is new.
        /// </summary>
        /// <param name="parentContext">
        /// The parent context.
        /// </param>
        public DefaultContext(DefaultContext parentContext)
        {
            this.Id = Guid.NewGuid();
            this.Parent = parentContext;
            this.IsReady = false;
            this.ContextFactory = parentContext.ContextFactory;
            this.Values = new Dictionary<string, object>(parentContext.Values);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DefaultContext"/> class. 
        /// </summary>
        ~DefaultContext()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// The transaction committing event. Raised synchronously before the transaction is committed.
        /// </summary>
        public event EventHandler<TransactionCommittingEventArgs> TransactionCommitting;

        /// <summary>
        /// The transaction committed event. Raised synchronously after the transaction is committed.
        /// </summary>
        public event EventHandler<TransactionCommittedEventArgs> TransactionCommitted;

        /// <summary>
        /// Gets the unique id of this context.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the parent context, if any.
        /// </summary>
        public IContext Parent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the context is ready (started and not disposed or committed).
        /// </summary>
        public bool IsReady { get; private set; }

        /// <summary>
        /// Gets the <see cref="IContextFactory"/> that this context was created from.
        /// </summary>
        public IContextFactory ContextFactory { get; private set; }

        /// <summary>
        /// Gets the values dictionary associated with the context.
        /// Allows the storage of various contextual information.
        /// </summary>
        public IDictionary<string, object> Values { get; private set; }

        /// <summary>
        /// Gets the current logged-in user.
        /// </summary>
        public IUser CurrentUser
        {
            get
            {
                return this.ContextFactory.SecurityProvider.GetCurrentUser(this);
            }
        }

        /// <summary>
        /// Adds an entity to the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Add(IEntity entity)
        {
            entity.ThrowIfNull("entity");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            this.contextProvider.Add(entity);
        }

        /// <summary>
        /// Removes an entity from the context.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Remove(IEntity entity)
        {
            entity.ThrowIfNull("entity");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            this.contextProvider.Remove(entity);
        }

        /// <summary>
        /// Reloads the <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        public void Reload(IEntity entity)
        {
            entity.ThrowIfNull("entity");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            this.contextProvider.Reload(entity);
        }

        /// <summary>
        /// Gets an entity by its id.
        /// </summary>
        /// <param name="id">
        /// The entity id.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity.
        /// </typeparam>
        /// <returns>
        /// The entity.
        /// </returns>
        public T GetById<T>(object id)
        {
            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            return this.contextProvider.GetById<T>(id);
        }

        /// <summary>
        /// Fulfill a <see cref="IQuery{T}"/>.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// The type or returned result query.
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Fulfill<T>(IQuery<T> query)
        {
            query.ThrowIfNull("query");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            return this.contextProvider.Fulfill(query);
        }

        /// <summary>
        /// Executes a command with no return type.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void Execute(ICommand command)
        {
            command.ThrowIfNull("command");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            command.Execute(this);
        }

        /// <summary>
        /// Executes a command and return the result.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <typeparam name="T">
        /// The type of result
        /// </typeparam>
        /// <returns>
        /// The result.
        /// </returns>
        public T Execute<T>(ICommand<T> command)
        {
            command.ThrowIfNull("command");

            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            return command.Execute(this);
        }

        /// <summary>
        /// Executes the command in an asynchronous manner.
        /// The command is executed in its own disposable cloned context, with a dependant transaction on the current transaction.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="dependentTrans">
        /// True to have the transaction of the inner context dependant on the current, false otherwise.
        /// </param>
        /// <returns>
        /// A Task which executes the work.
        /// </returns>
        public Task ExecuteAsync(ICommand command, bool dependentTrans = true)
        {
            command.ThrowIfNull("command");

            return Task.Factory.StartNew(
                state =>
                {
                    var stateCmd = (AsyncTaskStateCommand)state;
                    try
                    {
                        stateCmd.Context.Start();
                        stateCmd.Context.Execute(stateCmd.Command);
                        stateCmd.Context.Commit();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Resources.ErrorWhileExecutingCommandAsync.Format(stateCmd.Command), ex);
                        throw;
                    }
                    finally
                    {
                        stateCmd.Context.Dispose();
                    }
                },
                new AsyncTaskStateCommand { Context = this.Clone(), Command = command });
        }

        /// <summary>
        /// Executes the command in an asynchronous manner.
        /// The command is executed in its own disposable cloned context, with a dependant transaction on the current transaction.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="dependentTrans">
        /// True to have the transaction of the inner context dependant on the current, false otherwise.
        /// </param>
        /// <typeparam name="T">
        /// The type of result
        /// </typeparam>
        /// <returns>
        /// A <see cref="Task{TResult}"/> which executes the work.
        /// </returns>
        public Task<T> ExecuteAsync<T>(ICommand<T> command, bool dependentTrans = true)
        {
            return Task<T>.Factory.StartNew(
                state =>
                {
                    var stateCmd = (AsyncTaskStateCommandResult<T>)state;
                    try
                    {
                        stateCmd.Context.Start();
                        var result = stateCmd.Context.Execute(stateCmd.Command);
                        stateCmd.Context.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Resources.ErrorWhileExecutingCommandAsync.Format(stateCmd.Command), ex);
                        throw;
                    }
                    finally
                    {
                        stateCmd.Context.Dispose();
                    }
                },
                new AsyncTaskStateCommandResult<T> { Context = this.Clone(dependentTrans), Command = command });
        }

        /// <summary>
        /// Returns the <see cref="AuthorizationResult"/> related to the <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult IsAuthorized(string operation)
        {
            var result = this.ContextFactory.SecurityProvider.GetAuthorizationResult(this, operation);
            if (result == null)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.SecurityProviderReturnedNoResult,
                    this.ContextFactory.SecurityProvider,
                    operation);
                Log.Warn(message);
                var impliedResult = new DefaultAuthorizationResult(this.CurrentUser, operation);
                impliedResult.AddReason(message);
                return impliedResult;
            }

            Log.Debug(result);
            return result;
        }

        /// <summary>
        /// Returns the <see cref="AuthorizationResult"/> related to the <paramref name="operation"/> and
        /// the <paramref name="target"/>.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult IsAuthorized(string operation, object target)
        {
            var result = this.ContextFactory.SecurityProvider.GetAuthorizationResult(this, operation, target);
            if (result == null)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture, 
                    Resources.SecurityProviderReturnedNoResultWithTarget,
                    this.ContextFactory.SecurityProvider,
                    operation,
                    target);
                Log.Warn(message);
                var impliedResult = new DefaultAuthorizationResult(this.CurrentUser, operation, target);
                impliedResult.AddReason(message);
                return impliedResult;
            }

            Log.Debug(result);
            return result;
        }

        /// <summary>
        /// Returns the <see cref="AuthorizationResult"/> related to the <paramref name="operation"/>,
        /// the <paramref name="target"/> and the <paramref name="field"/>.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="field">
        /// The field.</param>
        /// <returns>
        /// The <see cref="AuthorizationResult"/>.
        /// </returns>
        public AuthorizationResult IsAuthorized(string operation, object target, string field)
        {
            var result = this.ContextFactory.SecurityProvider.GetAuthorizationResult(this, operation, target, field);
            if (result == null)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.SecurityProviderReturnedNoResultWithTargetAndField,
                    this.ContextFactory.SecurityProvider,
                    operation,
                    target,
                    field);
                Log.Warn(message);
                var impliedResult = new DefaultAuthorizationResult(this.CurrentUser, operation, target, field);
                impliedResult.AddReason(message);
                return impliedResult;
            }

            Log.Debug(result);
            return result;
        }

        /// <summary>
        /// Starts the context.
        /// </summary>
        public void Start()
        {
            Log.Debug(Resources.Starting.Format(this));
            this.transactionScope = this.dependentTransaction != null ? new TransactionScope(this.dependentTransaction) : new TransactionScope();
            this.contextProvider = this.ContextFactory.ContextProviderFactory.CreateContextProvider(this);
            if (this.contextProvider == null)
            {
                Log.Error(Resources.ContextProviderFactoryReturnedNull.Format(this.ContextFactory.ContextProviderFactory));
                throw new BackstageException(Resources.ContextProviderFactoryReturnedNull.Format(this.ContextFactory.ContextProviderFactory));
            }

            this.IsReady = true;
        }

        /// <summary>
        /// Starts the context.
        /// </summary>
        /// <param name="transactionScopeOption">
        /// The transaction scope option.
        /// </param>
        /// <param name="transactionOptions">
        /// The transaction options.
        /// </param>
        public void Start(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            if (this.dependentTransaction != null)
            {
                throw new BackstageException(Resources.TransactionOptionsCannotBeModified);
            }

            Log.Debug(Resources.Starting.Format(this));
            this.transactionScope = new TransactionScope(transactionScopeOption, transactionOptions);
            this.contextProvider = this.ContextFactory.ContextProviderFactory.CreateContextProvider(this);
            if (this.contextProvider == null)
            {
                Log.Error(Resources.ContextProviderFactoryReturnedNull.Format(this.ContextFactory.ContextProviderFactory));
                throw new BackstageException(Resources.ContextProviderFactoryReturnedNull.Format(this.ContextFactory.ContextProviderFactory));
            }

            this.IsReady = true;
        }

        /// <summary>
        /// Commits the context and thus the underlying transaction.
        /// If a context is not committed before it is disposed, it will rollback.
        /// </summary>
        public void Commit()
        {
            Log.Debug(Resources.Committing.Format(this));
            var stopwatch = Stopwatch.StartNew();
            var currentTransaction = Transaction.Current;
            try
            {
                this.OnTransactionCommitting(new TransactionCommittingEventArgs(currentTransaction));
                this.transactionScope.Complete();

                if (this.dependentTransaction != null)
                {
                    this.dependentTransaction.Complete();
                }

                this.OnTransactionCommitted(new TransactionCommittedEventArgs(currentTransaction));

                stopwatch.Stop();
                Log.Info(Resources.CommittedIn.Format(this, stopwatch.ElapsedMilliseconds));
            }
            catch (Exception ex)
            {
                Log.Error(Resources.ErrorWhileCommitting, ex);
                throw;
            }
            finally
            {
                this.IsReady = false;
            }
        }

        /// <summary>
        /// Clones the current context. Useful for multi-treaded scenarios.
        /// The cloned context is NOT started automatically.
        /// </summary>
        /// <param name="dependentTrans">
        /// True to have the context transaction dependent on the current transaction, false otherwise.
        /// </param>
        /// <returns>
        /// The cloned context.
        /// </returns>
        public IContext Clone(bool dependentTrans = true)
        {
            if (!this.IsReady)
            {
                throw new BackstageException(Resources.ContextNotReady);
            }

            var newContext = new DefaultContext(this);

            if (dependentTrans)
            {
                newContext.dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
            }

            return newContext;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Parent == null ? "Context({0})".Format(this.Id) : "Context({0} <- {1})".Format(this.Id, this.Parent.Id);
        }

        /// <summary>
        /// Dispose appropriate resources.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources must be disposed, false otherwise.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log.Debug(Resources.Disposing.Format(this));
                this.contextProvider.Dispose();
                this.transactionScope.Dispose();

                if (this.dependentTransaction != null)
                {
                    this.dependentTransaction.Dispose();
                }

                this.IsReady = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="TransactionCommitting"/> event.
        /// </summary>
        /// <param name="e">
        /// The arguments.
        /// </param>
        private void OnTransactionCommitting(TransactionCommittingEventArgs e)
        {
            if (this.TransactionCommitting != null)
            {
                this.TransactionCommitting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TransactionCommitted"/> event.
        /// </summary>
        /// <param name="e">
        /// The arguments.
        /// </param>
        private void OnTransactionCommitted(TransactionCommittedEventArgs e)
        {
            if (this.TransactionCommitted != null)
            {
                this.TransactionCommitted(this, e);
            }
        }

        /// <summary>
        /// Arguments for async tasks commands.
        /// </summary>
        private struct AsyncTaskStateCommand
        {
            /// <summary>
            /// Gets or sets the context.
            /// </summary>
            public IContext Context { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            public ICommand Command { get; set; }
        }

        /// <summary>
        /// Arguments for async tasks commands.
        /// </summary>
        /// <typeparam name="T">
        /// The result type.
        /// </typeparam>
        private struct AsyncTaskStateCommandResult<T>
        {
            /// <summary>
            /// Gets or sets the context.
            /// </summary>
            public IContext Context { get; set; }

            /// <summary>
            /// Gets or sets the command.
            /// </summary>
            public ICommand<T> Command { get; set; }
        }
    }
}
