using FluentAssertions;
using Kros.Data.BulkActions.SqlServer;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Kros.Utils.UnitTests.Data.BulkActions
{
    public class SqlServerBulkActionFactoryShould
    {
        [Fact]
        public void CreateSqlServerBulkInsertByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = new SqlServerBulkActionFactory(conn);
                var bulkInsert = (SqlServerBulkInsert)factory.GetBulkInsert(SqlBulkCopyOptions.UseInternalTransaction);

                bulkInsert.BulkCopyOptions.Should().Be(SqlBulkCopyOptions.UseInternalTransaction);
            }
        }

        [Fact]
        public void CreateSqlServerBulkUpdateByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = new SqlServerBulkActionFactory(conn);
                var bulkUpdate = factory.GetBulkUpdate() as SqlServerBulkUpdate;

                bulkUpdate.Should().NotBeNull();
            }
        }
    }
}
