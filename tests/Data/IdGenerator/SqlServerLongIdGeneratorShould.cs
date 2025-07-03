using Kros.Data;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    public class SqlServerLongGeneratorShould : SqlServerIntIdGeneratorTestsBase<long>
    {
        public SqlServerLongGeneratorShould(TestsFixture fixture) : base(fixture)
        {
        }

        protected override string BackendTableName => SqlServerLongIdGenerator.BackendTableNameValue;

        protected override string BackendProcedureName => SqlServerLongIdGenerator.BackendStoredProcedureNameValue;

        protected override IIdGenerator<long> CreateGenerator(SqlConnection connection)
            => new SqlServerLongIdGenerator(connection, "_UnitTestTableName_Int", 1);

        protected override IIdGeneratorFactory<long> CreateGeneratorFactory()
            => new SqlServerLongIdGeneratorFactory(ServerHelper.Connection);

        protected override (string tableScript, string procedureScript) InitBackendInfo()
        {
            (string _, string __, string tableScript, string storedProcedureScript) info = SqlServerLongIdGenerator.GetSqlInfo();
            return (info.tableScript, info.storedProcedureScript);
        }
    }
}
