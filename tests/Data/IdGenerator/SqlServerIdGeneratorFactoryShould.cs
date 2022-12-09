using FluentAssertions;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class SqlServerIdGeneratorFactoryShould
    {
        [Fact]
        public void CreateSqlServerIntIdGeneratorByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = new SqlServerIntIdGeneratorFactory(conn);
                var generator = (SqlServerIntIdGenerator)factory.GetGenerator("Person", 150);

                generator.TableName.Should().Be("Person");
                generator.BatchSize.Should().Be(150);
            }
        }

        [Fact]
        public void CreateSqlServerLongIdGeneratorByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = new SqlServerLongIdGeneratorFactory(conn);
                var generator = (SqlServerLongIdGenerator)factory.GetGenerator("Person", 150);

                generator.TableName.Should().Be("Person");
                generator.BatchSize.Should().Be(150);
            }
        }
    }
}
