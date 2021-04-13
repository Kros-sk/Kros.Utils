using Kros.Utils;
using Microsoft.Data.SqlClient;

namespace Kros.Data.SqlServer
{
    /// <summary>
    /// Factory for creating integer ID generators.
    /// </summary>
    /// <seealso cref="SqlServerLongIdGenerator"/>
    /// <seealso cref="IdGeneratorFactories"/>
    /// <example>
    /// <code language="cs" source="..\..\..\Documentation\Examples\Kros.Utils\IdGeneratorExamples.cs" region="IdGeneratorFactory"/>
    /// </example>
    public class SqlServerLongIdGeneratorFactory
        : IIdGeneratorFactory<long>
    {
        private readonly string _connectionString;
        private readonly SqlConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerLongIdGeneratorFactory"/> class.
        /// </summary>
        /// <param name="connection">Database connection. ID generators create IDs for tables in this database.</param>
        public SqlServerLongIdGeneratorFactory(SqlConnection connection)
        {
            _connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerLongIdGeneratorFactory"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string.
        /// ID generators create IDs for tables in this database.</param>
        public SqlServerLongIdGeneratorFactory(string connectionString)
        {
            _connectionString = Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
        }

        /// <inheritdoc/>
        public IIdGenerator<long> GetGenerator(string tableName)
            => GetGenerator(tableName, 1);

        /// <inheritdoc/>
        public IIdGenerator<long> GetGenerator(string tableName, int batchSize)
            => _connection != null ?
                new SqlServerLongIdGenerator(_connection, tableName, batchSize) :
                new SqlServerLongIdGenerator(_connectionString, tableName, batchSize);

        /// <inheritdoc/>
        IIdGenerator IIdGeneratorFactory.GetGenerator(string tableName)
            => GetGenerator(tableName, 1);

        /// <inheritdoc/>
        IIdGenerator IIdGeneratorFactory.GetGenerator(string tableName, int batchSize)
            => GetGenerator(tableName, batchSize);

        /// <summary>
        /// Registers factory methods for creating an instance of factory into <see cref="IdGeneratorFactories"/>.
        /// </summary>
        public static void Register()
            => IdGeneratorFactories.Register<SqlConnection>(
                typeof(int),
                SqlServerDataHelper.ClientId,
                (conn) => new SqlServerLongIdGeneratorFactory(conn as SqlConnection),
                (connString) => new SqlServerLongIdGeneratorFactory(connString));
    }
}
