﻿namespace Backstage
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A standard Backstage exception.
    /// </summary>
    [Serializable]
    public class BackstageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackstageException"/> class.
        /// </summary>
        public BackstageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackstageException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public BackstageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackstageException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public BackstageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackstageException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="Exception.HResult"/> is zero (0).
        /// </exception>
        protected BackstageException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
