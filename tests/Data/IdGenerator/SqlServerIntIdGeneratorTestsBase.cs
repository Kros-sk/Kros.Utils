using FluentAssertions;
using Kros.Data;
using Kros.Data.SqlServer;
using Kros.UnitTests;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    [Collection(TestsCollection.Name)]
    public abstract class SqlServerIntIdGeneratorTestsBase<T> : DatabaseTestBase
    {
        private readonly TestsFixture _context;

        public SqlServerIntIdGeneratorTestsBase(TestsFixture fixture)
        {
            _context = fixture;
        }

        protected override string BaseConnectionString => _context.GetConnectionString();

        protected abstract string BackendTableName { get; }
        protected abstract string BackendProcedureName { get; }

        private IEnumerable<string>? _databaseInitScripts = null;

        protected override IEnumerable<string> DatabaseInitScripts
        {
            get
            {
                if (_databaseInitScripts is null)
                {
                    (string tableScript, string procedureScript) = InitBackendInfo();
                    _databaseInitScripts = new List<string>
                    {
                        tableScript,
                        procedureScript
                    };
                }
                return _databaseInitScripts;
            }
        }

        [Fact]
        public void GenerateIdsForTable()
        {
            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    idGenerator.GetNext().Should().Be(i + 1);
                }
            }
        }

        [Fact]
        public void GenerateBatchIdsForTable()
        {
            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People", 10))
            {
                for (int i = 0; i < 15; i++)
                {
                    idGenerator.GetNext().Should().Be(i + 1);
                }
            }
        }

        [Fact]
        public void GenerateIdsForTableWhenDataExists()
        {
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            using (SqlCommand cmd = ServerHelper.Connection.CreateCommand())
            {
                cmd.CommandText = $"INSERT INTO {BackendTableName} VALUES ('People', 10)";
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    idGenerator.GetNext().Should().Be(10 + i + 1);
                }
            }
        }

        [Fact]
        public void GenerateBatchIdsForTableWhenDataExists()
        {
            using (ConnectionHelper.OpenConnection(ServerHelper.Connection))
            using (SqlCommand cmd = ServerHelper.Connection.CreateCommand())
            {
                cmd.CommandText = $"INSERT INTO {BackendTableName} VALUES ('People', 10)";
                cmd.CommandType = System.Data.CommandType.Text;

                cmd.ExecuteNonQuery();
            }

            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People", 10))
            {
                for (int i = 0; i < 15; i++)
                {
                    idGenerator.GetNext().Should().Be(10 + i + 1);
                }
            }
        }

        [Fact]
        public void MultipleGenerateIdsForTable()
        {
            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People"))
            {
                idGenerator.GetNext().Should().Be(1);

                using (IIdGenerator<T> nextGenerator = CreateGeneratorFactory().GetGenerator("People", 3))
                {
                    nextGenerator.GetNext().Should().Be(2);
                    idGenerator.GetNext().Should().Be(5);
                    nextGenerator.GetNext().Should().Be(3);
                    nextGenerator.GetNext().Should().Be(4);
                    nextGenerator.GetNext().Should().Be(6);
                }

                idGenerator.GetNext().Should().Be(9);
            }
        }

        [Fact]
        public void GenerateIdsForMoreTable()
        {
            using (IIdGenerator<T> idGenerator = CreateGeneratorFactory().GetGenerator("People"))
            {
                for (int i = 0; i < 10; i++)
                {
                    idGenerator.GetNext().Should().Be(i + 1);

                    using (IIdGenerator<T> addressIdGenerator = CreateGeneratorFactory().GetGenerator("Addresses", 5))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            addressIdGenerator.GetNext().Should().Be(j + 5 * i + 1);
                        }
                    }
                }
            }
        }

        [Fact]
        public void CreateTableAndStoredProcedureForIdGeneratorIfNotExits()
        {
            using (var helper = new SqlServerTestHelper(BaseConnectionString, BaseDatabaseName))
            {
                HasTable(helper.Connection, BackendTableName).Should().BeFalse();
                HasProcedure(helper.Connection, BackendProcedureName).Should().BeFalse();

                IIdGenerator<T> idGenerator = CreateGenerator(helper.Connection);
                idGenerator.InitDatabaseForIdGenerator();

                HasTable(helper.Connection, BackendTableName).Should().BeTrue();
                HasProcedure(helper.Connection, BackendProcedureName).Should().BeTrue();
            }
        }

        [Fact]
        public void NotThrowWhenCreatingTableAndStoredProcedureForIdGeneratorAndTheyExist()
        {
            using (var helper = new SqlServerTestHelper(BaseConnectionString, BaseDatabaseName, DatabaseInitScripts))
            {
                HasTable(helper.Connection, BackendTableName).Should().BeTrue();
                HasProcedure(helper.Connection, BackendProcedureName).Should().BeTrue();

                IIdGenerator<T> idGenerator = CreateGenerator(helper.Connection);
                idGenerator.InitDatabaseForIdGenerator();

                HasTable(helper.Connection, BackendTableName).Should().BeTrue();
                HasProcedure(helper.Connection, BackendProcedureName).Should().BeTrue();
            }
        }

        #region Helpers

        protected abstract (string tableScript, string procedureScript) InitBackendInfo();

        protected abstract IIdGeneratorFactory<T> CreateGeneratorFactory();

        protected abstract IIdGenerator<T> CreateGenerator(SqlConnection connection);

        protected static bool HasTable(SqlConnection connection, string tableName)
        {
            using (ConnectionHelper.OpenConnection(connection))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP 1 1 FROM sys.tables WHERE name='{tableName}' AND type='U'";
                return cmd.ExecuteScalar() is not null;
            }
        }

        protected static bool HasProcedure(SqlConnection connection, string procedureName)
        {
            using (ConnectionHelper.OpenConnection(connection))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT TOP 1 1 FROM sys.procedures WHERE name='{procedureName}' AND type='P'";
                return cmd.ExecuteScalar() is not null;
            }
        }

        #endregion
    }
}
