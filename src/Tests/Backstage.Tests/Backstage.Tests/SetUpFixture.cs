namespace Backstage.Tests
{
    using Common.Logging;

    using NUnit.Framework;

    [SetUpFixture]
    public class SetUpFixture
    {
        [SetUp]
        public void SetUp()
        {
            LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter(LogLevel.Debug, true, true, true, "yyyyMMdd-hh:mm:ss");
        }
    }
}
