using Kros.Data.BulkActions;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class BulkActionFactoriesShould
    {
        [Fact]
        public void GetFactoryByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = BulkActionFactories.GetFactory(conn);

                Assert.NotNull(factory);
            }
        }

        [Fact]
        public void GetFactoryByAdoClientName()
        {
            var factory = BulkActionFactories.GetFactory("connectionstring", SqlServerDataHelper.ClientId);

            Assert.NotNull(factory);
        }

        [Fact]
        public void ThrowExceptionWhenConnectionIsNotRegistered()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = BulkActionFactories.GetFactory(conn); };

                InvalidOperationException ex = Assert.Throws<InvalidOperationException>(action);
                Assert.Contains(typeof(CustomConnection).FullName!, ex.Message);
            }
        }

        [Fact]
        public void ThrowExceptionWhenAdoClientNameIsNotRegistered()
        {
            Action action = () => { var factory = BulkActionFactories.GetFactory("constring", "System.Data.CustomClient"); };

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(action);
            Assert.Contains("System.Data.CustomClient", ex.Message);
        }
    }
}
