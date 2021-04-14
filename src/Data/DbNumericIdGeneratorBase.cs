using Kros.Utils;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace Kros.Data
{
    /// <summary>
    /// Base class for simple creation of <see cref="IIdGenerator"/>, which generates <c>int</c> values
    /// and uses databse as backend.
    /// </summary>
    /// <seealso cref="IIdGenerator" />
    public abstract class DbNumericIdGeneratorBase<T> : IIdGenerator<T> where T : struct, IComparable<T>
    {
        private readonly bool _disposeOfConnection = false;

        /// <summary>
        /// Creates an instance of ID generator for table <paramref name="tableName"/> in database <paramref name="connection"/>
        /// and with specified <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connection">Database connection.</param>
        /// <param name="tableName">Table name, for which IDs are generated.</param>
        /// <param name="batchSize">IDs batch size. Saves round trips to database for IDs.</param>
        /// <exception cref="ArgumentNullException">
        /// Value of <paramref name="connection"/> or <paramref name="tableName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">Value of <paramref name="batchSize"/> is less or equal than 0.</exception>
        protected DbNumericIdGeneratorBase(DbConnection connection, string tableName, int batchSize)
            : this(tableName, batchSize)
        {
            Connection = Check.NotNull(connection, nameof(connection));
        }

        /// <summary>
        /// Creates an instance of ID generator for table <paramref name="tableName"/> in database
        /// <paramref name="connectionString"/> and with specified <paramref name="batchSize"/>.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
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
        protected DbNumericIdGeneratorBase(string connectionString, string tableName, int batchSize)
            : this(tableName, batchSize)
        {
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Connection = CreateConnection(connectionString);
            _disposeOfConnection = true;
        }

        private DbNumericIdGeneratorBase(string tableName, int batchSize)
        {
            TableName = Check.NotNull(tableName, nameof(tableName));
            BatchSize = Check.GreaterThan(batchSize, 0, nameof(batchSize));
            _nextId = default;
            _nextAccessToDb = AddValue(_nextId, -1);
        }

        /// <summary>
        /// Creates a database connection instance.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns>Specific instance of <see cref="DbConnection"/>.</returns>
        protected abstract DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Table name for which IDs are generated.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Batch size - saves roundtrips into database.
        /// </summary>
        public int BatchSize { get; }

        /// <summary>
        /// Database connection.
        /// </summary>
        protected DbConnection Connection { get; }

        private T _nextId;
        private T _nextAccessToDb;

        /// <inheritdoc/>
        public virtual T GetNext()
        {
            if (_nextAccessToDb.CompareTo(_nextId) <= 0)
            {
                _nextId = GetNewIdFromDb();
                _nextAccessToDb = AddValue(_nextId, BatchSize);
            }
            T result = _nextId;
            _nextId = AddValue(_nextId, 1);
            return result;
        }

        /// <inheritdoc cref="IIdGenerator.GetNext"/>
        object IIdGenerator.GetNext() => GetNext();

        /// <summary>
        /// Sums <paramref name="increment"/> and <paramref name="baseValue"/>.
        /// </summary>
        /// <param name="baseValue">Number 1.</param>
        /// <param name="increment">Number 2.</param>
        /// <returns>Sum of <paramref name="baseValue"/> and <paramref name="increment"/>.</returns>
        protected abstract T AddValue(T baseValue, int increment);

        private T GetNewIdFromDb()
        {
            using (ConnectionHelper.OpenConnection(Connection))
            {
                return GetNewIdFromDbCore();
            }
        }

        /// <summary>
        /// Returns new ID from database. In this method is ensured, that the <see cref="Connection"/> is opened.
        /// </summary>
        /// <returns>Next ID.</returns>
        protected virtual T GetNewIdFromDbCore()
        {
            using (var cmd = Connection.CreateCommand() as SqlCommand)
            {
                cmd.CommandText = BackendStoredProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@TableName", SqlDbType.NVarChar) { Value = TableName });
                cmd.Parameters.Add(new SqlParameter("@NumberOfItems", SqlDbType.Int) { Value = BatchSize });

                return (T)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Databse type for generator value.
        /// </summary>
        public abstract string BackendDataType { get; }

        /// <summary>
        /// Database table name for generator values.
        /// </summary>
        public abstract string BackendTableName { get; }

        /// <summary>
        /// Database stored procedure name for getting generator value.
        /// </summary>
        public abstract string BackendStoredProcedureName { get; }

        /// <summary>
        /// Script to create database table for generator.
        /// </summary>
        public virtual string BackendTableScript => GetScript("Kros.Resources.NumericIdGeneratorTable.sql");

        /// <summary>
        /// Script to create database stored procedure for generator.
        /// </summary>
        public virtual string BackendStoredProcedureScript => GetScript("Kros.Resources.NumericIdGeneratorStoredProcedure.sql");

        private string GetScript(string scriptName)
            => ResourceHelper.GetResourceContent(scriptName)
                .Replace("{{DataType}}", BackendDataType)
                .Replace("{{TableName}}", BackendTableName)
                .Replace("{{StoredProcedureName}}", BackendStoredProcedureName);

        /// <inheritdoc/>
        public virtual void InitDatabaseForIdGenerator()
        {
            using (ConnectionHelper.OpenConnection(Connection))
            using (DbCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = BackendTableScript;
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    $"IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = '{BackendStoredProcedureName}' AND type = 'P')"
                    + Environment.NewLine
                    + $"EXEC('{BackendStoredProcedureScript.Replace("'", "''")}');";
                cmd.ExecuteNonQuery();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (_disposeOfConnection)
                {
                    Connection.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
