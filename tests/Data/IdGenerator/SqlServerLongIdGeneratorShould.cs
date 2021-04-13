using Kros.Data;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;

namespace Kros.Utils.UnitTests.Data.IdGenerator
{
    public class SqlServerLongGeneratorShould : SqlServerIntIdGeneratorTestsBase<long>
    {
        protected override IIdGenerator<long> CreateGenerator(SqlConnection connection)
            => new SqlServerLongIdGenerator(connection, "_UnitTestTableName_Int", 1);

        protected override IIdGeneratorFactory<long> CreateGeneratorFactory()
            => new SqlServerLongIdGeneratorFactory(ServerHelper.Connection);

        protected override (string tableName, string procedureName, string tableScript, string procedureScript) InitBackendInfo()
            => SqlServerLongIdGenerator.GetSqlInfo();
    }
}
