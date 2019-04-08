namespace Kros.Utils.UnitTests
{
    /// <summary>
    /// Základná trieda pre databázové integračné testy.
    /// </summary>
    public class DatabaseTestBase
        : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        protected override string BaseConnectionString => "Server=tcp:githubbuildserver.database.windows.net,1433;Persist Security Info=True;User ID=kros;Password=21Admin12;";
    }
}
