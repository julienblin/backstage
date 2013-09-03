namespace Backstage.NHibernateProvider.Tests
{
    using System.Collections.Generic;
    using System.IO;

    using Common.Logging;

    using NUnit.Framework;

    [SetUpFixture]
    public class SetUpFixture
    {
        public const string TestDbFile = "test.db";

        public const string ConnectionString = "Data Source=" + TestDbFile + ";Version=3;New=True;";

        [SetUp]
        public void SetUp()
        {
            LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter(LogLevel.Debug, true, true, true, "yyyyMMdd-hh:mm:ss");

            if (File.Exists(TestDbFile))
            {
                File.Delete(TestDbFile);
            }

            ContextFactory.StartNew(
                new ContextFactoryConfiguration(
                    new NHContextProviderFactory(new NHContextProviderFactoryConfiguration
                                                     {
                                                         DatabaseType = DatabaseType.SQLite,
                                                         ConnectionString = ConnectionString,
                                                         AutoUpdateSchemaOnStart = true,
                                                         RaiseDomainEvents = true,
                                                         NHProperties = new Dictionary<string, string>
                                                                            {
                                                                                { NHibernate.Cfg.Environment.FormatSql, "true" }
                                                                            }
                                                     })));
        }

        [TearDown]
        public void TearDown()
        {
            ContextFactory.Current.Dispose();
        }
    }
}
