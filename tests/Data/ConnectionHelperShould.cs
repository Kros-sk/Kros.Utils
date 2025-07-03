using FluentAssertions;
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

            ServerHelper.Connection.State.Should().Be(ConnectionState.Closed);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            }
            ServerHelper.Connection.State.Should().Be(ConnectionState.Closed);
        }

        [Fact]
        public void KeepConnectionOpenedAtTheEnd()
        {
            if (!ServerHelper.Connection.IsOpened())
            {
                ServerHelper.Connection.Open();
            }

            ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            {
                ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
            }
            ServerHelper.Connection.State.Should().Be(ConnectionState.Open);
        }
    }
}
