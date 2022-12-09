using Microsoft.Data.SqlClient;
using System;
using System.Data.Common;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// The integer ID generator for Microsoft SQL Server.
    /// </summary>
    /// <seealso cref="SqlServerIntIdGeneratorFactory" />
    /// <seealso cref="SqlServerLongIdGeneratorFactory" />
    /// <seealso cref="IdGeneratorFactories" />
    /// <seealso cref="IdGeneratorFactories" />
    /// <remarks>In general, the generator should be created using <see cref="SqlServerLongIdGeneratorFactory"/>.</remarks>
    /// <example>
    /// <code language="cs" source="..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public class SqlServerLongIdGenerator : DbNumericIdGeneratorBase<long>
    {
        /// <summary>
        /// Returns SQL names and scripts for generator.
        /// </summary>
        /// <returns>SQL names and scripts.</returns>
        public static
#if IsOldDotNet
            Tuple<string, string, string, string>
#else
            (string tableName, string storedProcedureName, string tableScript, string storedProcedureScript)
#endif
            GetSqlInfo()
        {
            var generator = new SqlServerLongIdGenerator(new SqlConnection(), "_NonExistingtable", 1);
            string tableScript = generator.BackendTableScript;
            string storedProcedureScript = generator.BackendStoredProcedureScript;
            generator.Dispose();
#if IsOldDotNet
            return Tuple.Create(generator.BackendTableName, generator.BackendStoredProcedureName, tableScript, storedProcedureScript);
#else
            return (generator.BackendTableName, generator.BackendStoredProcedureName, tableScript, storedProcedureScript);
#endif
        }

        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connectionString"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connectionString">Connection string to the database.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connectionString"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><list type="bullet">
        /// <item>Value of <paramref name="connectionString"/> is empty string, or string containing only
        /// whitespace characters.</item>
        /// <item>Value of <paramref name="batchSize"/> is less or equal than 0.</item>
        /// </list></exception>
        public SqlServerLongIdGenerator(string connectionString, string tableName, int batchSize)
            : base(connectionString, tableName, batchSize)
        {
        }

        /// <summary>
        /// Creates a generator for table <paramref name="tableName"/> in database <paramref name="connection"/>
        /// with batch size <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connection"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Value of <paramref name="batchSize"/> is less or equal than 0.</exception>
        public SqlServerLongIdGenerator(SqlConnection connection, string tableName, int batchSize)
            : base(connection, tableName, batchSize)
        {
        }

        /// <inheritdoc/>
        public override string BackendDataType => "bigint";

        /// <inheritdoc/>
        public override string BackendTableName => "IdStoreInt64";

        /// <inheritdoc/>
        public override string BackendStoredProcedureName => "spGetNewIdInt64";

        /// <inheritdoc/>
        protected override DbConnection CreateConnection(string connectionString) => new SqlConnection(connectionString);

        /// <inheritdoc/>
        protected override long AddValue(long baseValue, int increment) => baseValue + increment;
    }
}
