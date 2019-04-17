namespace Kros.Utils.UnitTests
{
    /// <summary>
    /// Základná trieda pre databázové integračné testy.
    /// </summary>
    public class DatabaseTestBase
        : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        protected override string BaseConnectionString
            => ConfigurationHelper.GetConfiguration().GetSection("connectionString").Value;
    }
}
