namespace Backstage.NHibernateProvider.Logs
{
    using System;
    using System.Globalization;

    using Common.Logging;

    using NHibernate;

    /// <summary>
    /// NHibernate <see cref="IInternalLogger"/> for <see cref="Common.Logging"/>.
    /// </summary>
    public class NHCommonInternalLogger : IInternalLogger
    {
        /// <summary>
        /// The log.
        /// </summary>
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="NHCommonInternalLogger"/> class.
        /// </summary>
        /// <param name="keyName">
        /// The key name.
        /// </param>
        public NHCommonInternalLogger(string keyName)
        {
            this.log = LogManager.GetLogger(keyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHCommonInternalLogger"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public NHCommonInternalLogger(Type type)
        {
            this.log = LogManager.GetLogger(type);
        }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Error level.
        /// </summary>
        public bool IsErrorEnabled
        {
            get
            {
                return this.log.IsErrorEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Fatal level.
        /// </summary>
        public bool IsFatalEnabled
        {
            get
            {
                return this.log.IsFatalEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Warn level.
        /// </summary>
        public bool IsWarnEnabled
        {
            get
            {
                return this.log.IsWarnEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Info level.
        /// </summary>
        public bool IsInfoEnabled
        {
            get
            {
                return this.log.IsInfoEnabled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this logger is enabled for the Debug level.
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                return this.log.IsDebugEnabled;
            }
        }

        /// <summary>
        /// Logs a message with the Error level.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Error(object message)
        {
            this.log.Error(message);
        }

        /// <summary>
        /// Logs a message with the Error level and an exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Error(object message, Exception exception)
        {
            this.log.Error(message, exception);
        }

        /// <summary>
        /// Logs a message with the Error level.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void ErrorFormat(string format, params object[] args)
        {
            this.log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs a message with the Fatal level.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Fatal(object message)
        {
            this.log.Fatal(message);
        }

        /// <summary>
        /// Logs a message with the Fatal level and an exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Fatal(object message, Exception exception)
        {
            this.log.Fatal(message, exception);
        }

        /// <summary>
        /// Logs a message with the Debug level.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Debug(object message)
        {
            this.log.Debug(message);
        }

        /// <summary>
        /// Logs a message with the Debug level and an exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Debug(object message, Exception exception)
        {
            this.log.Debug(message, exception);
        }

        /// <summary>
        /// Logs a message with the Debug level.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void DebugFormat(string format, params object[] args)
        {
            this.log.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs a message with the Info level.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Info(object message)
        {
            this.log.Info(message);
        }

        /// <summary>
        /// Logs a message with the Info level and an exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Info(object message, Exception exception)
        {
            this.log.Info(message, exception);
        }

        /// <summary>
        /// Logs a message with the Info level.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void InfoFormat(string format, params object[] args)
        {
            this.log.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Logs a message with the Warn level.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Warn(object message)
        {
            this.log.Warn(message);
        }

        /// <summary>
        /// Logs a message with the Warn level and an exception.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Warn(object message, Exception exception)
        {
            this.log.Warn(message, exception);
        }

        /// <summary>
        /// Logs a message with the Warn level.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void WarnFormat(string format, params object[] args)
        {
            this.log.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }
    }
}
