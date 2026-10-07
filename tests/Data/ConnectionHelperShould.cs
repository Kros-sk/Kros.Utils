using Kros.Data;
using System.Data;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    [Collection(TestsCollection.Name)]
    public class ConnectionHelperShould : Kros.UnitTests.SqlServerDatabaseTestBase
    {
        private readonly TestsFixture _context;

        public ConnectionHelperShould(TestsFixture fixture)
        {
            _context = fixture;
        }

        protected override string BaseConnectionString => _context.GetConnectionString();

        [Fact]
        public void OpenConnectionOnStartAndCloseItAtTheEnd()
        {
            ServerHelper.Connection.Close();

            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            }
            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
        }

        [Fact]
        public void KeepConnectionOpenedAtTheEnd()
        {
            if (!ServerHelper.Connection.IsOpened())
            {
                ServerHelper.Connection.Open();
            }

            Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            }
            Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
        }
    }
}
