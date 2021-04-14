using Microsoft.Data.SqlClient;
using System;

namespace Kros.Data.SqlServer
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [Obsolete("Class is deprecated. Use SqlServerIntIdGenerator class instead.")]
    public class SqlServerIdGenerator : SqlServerIntIdGenerator
    {
        public SqlServerIdGenerator(string connectionString, string tableName, int batchSize)
            : base(connectionString, tableName, batchSize)
        {
        }

        public SqlServerIdGenerator(SqlConnection connection, string tableName, int batchSize)
            : base(connection, tableName, batchSize)
        {
        }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
