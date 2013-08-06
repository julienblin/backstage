namespace Backstage
{
    using System;
    using System.Transactions;

    /// <summary>
    /// The <see cref="EventArgs"/> for transaction committing events.
    /// </summary>
    public class TransactionCommittingEventArgs : EventArgs
    {
        /// <summary>
        /// The transaction.
        /// </summary>
        private readonly Transaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionCommittingEventArgs"/> class.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        public TransactionCommittingEventArgs(Transaction transaction)
        {
            this.transaction = transaction;
        }

        /// <summary>
        /// Gets the committing transaction.
        /// </summary>
        public Transaction Transaction
        {
            get
            {
                return this.transaction;
            }
        }
    }
}
