using Kros.Data;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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

        [Fact]
        public async Task OpenConnectionAsynchronouslyOnStartAndCloseItAtTheEnd()
        {
            ServerHelper.Connection.Close();

            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
            using (await ConnectionHelper.OpenConnectionAsync(ServerHelper.Connection, TestContext.Current.CancellationToken))
            {
                Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            }
            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
        }

        [Fact]
        public async Task KeepConnectionOpenedAtTheEndWhenOpenedAsynchronously()
        {
            if (!ServerHelper.Connection.IsOpened())
            {
                ServerHelper.Connection.Open();
            }

            Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            using (await ConnectionHelper.OpenConnectionAsync(ServerHelper.Connection, TestContext.Current.CancellationToken))
            {
                Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            }
            Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
        }

        [Fact]
        public async Task OpenConnectionAsynchronouslyWithoutToken()
        {
            ServerHelper.Connection.Close();

            using (await ConnectionHelper.OpenConnectionAsync(ServerHelper.Connection, TestContext.Current.CancellationToken))
            {
                Assert.Equal(ConnectionState.Open, ServerHelper.Connection.State);
            }
            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
        }

        [Fact]
        public async Task ThrowOperationCanceledExceptionAndLeaveConnectionClosedWhenTokenIsAlreadyCancelled()
        {
            ServerHelper.Connection.Close();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => ConnectionHelper.OpenConnectionAsync(ServerHelper.Connection, new CancellationToken(canceled: true)));

            Assert.Equal(ConnectionState.Closed, ServerHelper.Connection.State);
        }

        [Fact]
        public async Task ThrowArgumentNullExceptionWhenConnectionIsNullInAsyncOpen()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => ConnectionHelper.OpenConnectionAsync(null!, TestContext.Current.CancellationToken));
        }
    }
}
