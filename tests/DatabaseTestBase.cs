namespace Kros.Utils.UnitTests
{
    /// <summary>
    /// Základná trieda pre databázové integračné testy.
    /// </summary>
    public class DatabaseTestBase
        : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        protected override string BaseConnectionString => "Server=CENSQL\\SQL16ENT;Persist Security Info=True;User ID=KrosPlus;Password=7040;";
    }
}
