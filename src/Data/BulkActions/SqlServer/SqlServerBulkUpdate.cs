using Kros.Utils;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Kros.Data.BulkActions.SqlServer
{
    /// <summary>
    /// Class for fast data update for SQL Server.
    /// </summary>
    /// <example>
    ///   <code source="..\..\..\..\Documentation\Examples\Kros.Utils\BulkUpdateExamples.cs" title="Bulk update" region="BulkUpdate" language="cs" />
    /// </example>
    public class SqlServerBulkUpdate : BulkUpdateBase
    {
        #region Constructors

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with database connection <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be updated, connection has to be opened.
        /// If transaction is running on connection, contructor with defined external transaction has to be used.</param>
        public SqlServerBulkUpdate(SqlConnection connection)
            : this(connection, null)
        {
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with database connection <paramref name="connection"/>,
        /// and external transaction <paramref name="externalTransaction"/>.
        /// </summary>
        /// <param name="connection">Database connection where data will be updated, connection has to be opened.
        /// If transaction is running on connection, transaction has to be defined in <paramref name="externalTransaction"/>.
        /// </param>
        /// <param name="externalTransaction">External transaction, in which bulk update is executed.</param>
        public SqlServerBulkUpdate(SqlConnection connection, SqlTransaction? externalTransaction)
            : base(connection, false)
        {
            ExternalTransaction = externalTransaction;
        }

        /// <summary>
        /// Initialize new instance of <see cref="SqlServerBulkUpdate"/> with <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">Connection string for database connection.</param>
        public SqlServerBulkUpdate(string connectionString)
            : base(new SqlConnection(connectionString), true)
        {
        }

        #endregion

        #region BulkUpdateBase Members

        /// <inheritdoc/>
        protected override IBulkInsert CreateBulkInsert()
        {
            return new SqlServerBulkInsert((SqlConnection)_connection, (SqlTransaction?)ExternalTransaction);
        }

        /// <inheritdoc/>
        protected override void InvokeAction(string tempTableName)
        {
            TempTableAction?.Invoke(_connection, ExternalTransaction, tempTableName);
        }

        /// <inheritdoc/>
        protected override string GetTempTableName() => $"{PrefixTempTable}{DestinationTableName}_{Guid.NewGuid()}";

        /// <inheritdoc/>
        protected override void CreateTempTableCore(IDataReader reader, string tempTableName)
        {
            string identityColumnName = "";
            using (var cmd = _connection.CreateCommand())
            {
                cmd.Transaction = ExternalTransaction;

                cmd.CommandText = "SELECT name FROM sys.identity_columns " +
                    $"WHERE OBJECT_NAME(object_id) = '{DestinationTableName}'";
                identityColumnName = (string)cmd.ExecuteScalar()!;
                if (!Enumerable.Contains(PrimaryKeyColumns, identityColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    identityColumnName = string.Empty;
                }

                cmd.CommandText = $"SELECT {GetColumnNamesForTempTable(reader, identityColumnName)} INTO [{tempTableName}] " +
                    $"FROM [{DestinationTableName}] WHERE (1 = 2)";
                cmd.ExecuteNonQuery();
            }
            CreateTempTablePrimaryKey(tempTableName, identityColumnName);
        }

        /// <summary>
        /// Creates a primary key for temporary table.
        /// </summary>
        /// <param name="tempTableName">Name of the temporary table.</param>
        /// <param name="columnName">Name of the column which must be created in temp table. If the value
        /// is <see langword="null"/>, no column is created, just the primary key.</param>
        private void CreateTempTablePrimaryKey(string tempTableName, string columnName)
        {
            using (var cmd = CreateCommandForPrimaryKey())
            {
                if (columnName != string.Empty)
                {
                    cmd.CommandText = "SELECT data_type FROM information_schema.columns " +
                        $"WHERE table_name = '{DestinationTableName}' AND column_name = '{columnName}'";
                    string dataType = (string)cmd.ExecuteScalar()!;

                    cmd.CommandText = $"ALTER TABLE [{tempTableName}] ADD [{columnName}] [{dataType}] NOT NULL";
                    cmd.ExecuteNonQuery();
                }
                string pkList = string.Join(", ", PrimaryKeyColumns.Select(item => $"[{item}]"));
                cmd.CommandText = $"ALTER TABLE [{tempTableName}] " +
                    $"ADD CONSTRAINT [PK_{tempTableName.Trim(PrefixTempTable)}] " +
                    $"PRIMARY KEY NONCLUSTERED ({pkList})";
                cmd.ExecuteNonQuery();
            }
        }

        /// <inheritdoc/>
        protected override IDbCommand CreateCommandForPrimaryKey()
        {
            var ret = _connection.CreateCommand();

            ret.Transaction = ExternalTransaction;

            return ret;
        }

        /// <inheritdoc/>
        protected async override Task UpdateDestinationTableAsync(IDataReader reader, string tempTableName, bool useAsync)
        {
            using (var cmd = _connection.CreateCommand())
            {
                var innerJoin = new System.Text.StringBuilder();
                foreach (string pkColumn in PrimaryKeyColumns)
                {
                    innerJoin.Append($"([{DestinationTableName}].[{pkColumn}] = [{tempTableName}].[{pkColumn}]) AND ");
                }
                innerJoin.Length -= 5;

                cmd.Transaction = ExternalTransaction;
                cmd.CommandText = $"UPDATE [{DestinationTableName}]\r\n" +
                    $"SET {GetUpdateColumnNames(reader, tempTableName)}\r\n" +
                    $"FROM [{DestinationTableName}]\r\n" +
                    $"INNER JOIN [{tempTableName}] ON ({innerJoin})";

                await ExecuteNonQueryAsync(useAsync, cmd);
            }
        }

        /// <inheritdoc/>
        protected async override Task DoneTempTableAsync(string tempTableName, bool useAsync)
        {
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.Transaction = ExternalTransaction;
                cmd.CommandText = $"DROP TABLE [{tempTableName}]";

                await ExecuteNonQueryAsync(useAsync, cmd);
            }
        }

        private static async Task ExecuteNonQueryAsync(bool useAsync, IDbCommand cmd)
        {
            if (useAsync)
            {
                await ((DbCommand)cmd).ExecuteNonQueryAsync();
            }
            else
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
