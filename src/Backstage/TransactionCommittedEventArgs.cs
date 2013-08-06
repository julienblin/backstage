namespace Backstage
{
    using System;
    using System.Transactions;

    /// <summary>
    /// The <see cref="EventArgs"/> for transaction committed events.
    /// </summary>
    public class TransactionCommittedEventArgs : EventArgs
    {
        /// <summary>
        /// The transaction.
        /// </summary>
        private readonly Transaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionCommittedEventArgs"/> class.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        public TransactionCommittedEventArgs(Transaction transaction)
        {
            this.transaction = transaction;
        }

        /// <summary>
        /// Gets the committed transaction.
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
