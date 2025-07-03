using Kros.Data;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    public class SqlServerIntIdGeneratorShould : SqlServerIntIdGeneratorTestsBase<int>
    {
        public SqlServerIntIdGeneratorShould(TestsFixture fixture) : base(fixture)
        {
        }

        protected override string BackendTableName => SqlServerIntIdGenerator.BackendTableNameValue;

        protected override string BackendProcedureName => SqlServerIntIdGenerator.BackendStoredProcedureNameValue;

        protected override IIdGenerator<int> CreateGenerator(SqlConnection connection)
            => new SqlServerIntIdGenerator(connection, "_UnitTestTableName_Int", 1);

        protected override IIdGeneratorFactory<int> CreateGeneratorFactory()
            => new SqlServerIntIdGeneratorFactory(ServerHelper.Connection);

        protected override (string tableScript, string procedureScript) InitBackendInfo()
        {
            (string _, string __, string tableScript, string storedProcedureScript) info = SqlServerIntIdGenerator.GetSqlInfo();
            return (info.tableScript, info.storedProcedureScript);
        }
    }
}
