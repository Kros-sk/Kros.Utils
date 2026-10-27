using Kros.Utils;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Kros.Data
{
    /// <summary>
    /// Helper class for opening database connection. It ensures opening and closing of the connection.
    /// </summary>
    public class ConnectionHelper
        : Suspender
    {
        /// <summary>
        /// If database <paramref name="connection"/> is closed, it is opened immediately. After disposing of returned
        /// object, the connection is closed, but only if it was opened. So when already opened connection is passed
        /// in the parameter, nothing is done with it.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>
        /// Helper object, which closes connection when it is disposed of.
        /// </returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public static IDisposable OpenConnection(IDbConnection connection)
        {
            Check.NotNull(connection, nameof(connection));
            var helper = new ConnectionHelper(connection);
            return helper.Suspend();
        }

        /// <summary>
        /// If database <paramref name="connection"/> is closed, it is opened asynchronously. After disposing of returned
        /// object, the connection is closed, but only if it was opened. So when already opened connection is passed
        /// in the parameter, nothing is done with it.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <returns>
        /// A task whose result is a helper object, which closes connection when it is disposed of.
        /// </returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        public static Task<IDisposable> OpenConnectionAsync(DbConnection connection)
            => OpenConnectionAsync(connection, CancellationToken.None);

        /// <summary>
        /// If database <paramref name="connection"/> is closed, it is opened asynchronously. After disposing of returned
        /// object, the connection is closed, but only if it was opened. So when already opened connection is passed
        /// in the parameter, nothing is done with it.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task whose result is a helper object, which closes connection when it is disposed of.
        /// </returns>
        /// <exception cref="ArgumentNullException">Value of <paramref name="connection"/> is <see langword="null"/>.</exception>
        /// <exception cref="OperationCanceledException">Cancellation was requested before the connection was opened.
        /// </exception>
        public static async Task<IDisposable> OpenConnectionAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            Check.NotNull(connection, nameof(connection));
            cancellationToken.ThrowIfCancellationRequested();

            bool closeConnection = !connection.IsOpened();
            if (closeConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }
            return new ConnectionCloser(connection, closeConnection);
        }

        private sealed class ConnectionCloser : IDisposable
        {
            private readonly IDbConnection _connection;
            private bool _closeConnection;

            public ConnectionCloser(IDbConnection connection, bool closeConnection)
            {
                _connection = connection;
                _closeConnection = closeConnection;
            }

            public void Dispose()
            {
                if (_closeConnection)
                {
                    _connection.Close();
                    _closeConnection = false;
                }
            }
        }

        private readonly IDbConnection _connection;
        private bool _closeConnection = false;

        private ConnectionHelper(IDbConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Open a connection.
        /// </summary>
        protected override void SuspendCore()
        {
            base.SuspendCore();
            if (!_connection.IsOpened())
            {
                _closeConnection = true;
                _connection.Open();
            }
        }

        /// <summary>
        /// Close a connection.
        /// </summary>
        protected override void ResumeCore()
        {
            base.ResumeCore();
            if (_closeConnection)
            {
                _connection.Close();
                _closeConnection = false;
            }
        }
    }
}
