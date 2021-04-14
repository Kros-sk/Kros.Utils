using Kros.Data;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    public class SqlServerIntIdGeneratorShould : SqlServerIntIdGeneratorTestsBase<int>
    {
        protected override IIdGenerator<int> CreateGenerator(SqlConnection connection)
            => new SqlServerIntIdGenerator(connection, "_UnitTestTableName_Int", 1);

        protected override IIdGeneratorFactory<int> CreateGeneratorFactory()
            => new SqlServerIntIdGeneratorFactory(ServerHelper.Connection);

        protected override (string tableName, string procedureName, string tableScript, string procedureScript) InitBackendInfo()
            => SqlServerIntIdGenerator.GetSqlInfo();
    }
}
