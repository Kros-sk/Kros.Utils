using Kros.Data;
using Kros.Data.SqlServer;
using Microsoft.Data.SqlClient;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Data
{
    public class IdGeneratorFactoriesShould
    {
        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(Guid))]
        public void GetFactoryByConnection(Type dataType)
        {
            using (var conn = new SqlConnection())
            {
                var factory = IdGeneratorFactories.GetFactory(dataType, conn);

                Assert.NotNull(factory);
            }
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        [InlineData(typeof(Guid))]
        public void GetFactoryByAdoClientName(Type dataType)
        {
            var factory = IdGeneratorFactories.GetFactory(dataType, "connectionstring", SqlServerDataHelper.ClientId);

            Assert.NotNull(factory);
        }

        [Fact]
        public void ThrowExceptionWhenDataTypeIsNotRegistered()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(DateTime), conn); };

                Assert.Throws<InvalidOperationException>(action);
            }
        }

        [Fact]
        public void ThrowExceptionWhenConnectionIsNotRegisterd()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(int), conn); };

                InvalidOperationException ex = Assert.Throws<InvalidOperationException>(action);
                Assert.Contains("CustomConnection", ex.Message);
            }
        }

        [Fact]
        public void ThrowExceptionWhenAdoClientNameIsNotRegistered()
        {
            Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(int), "constring", "System.Data.CustomClient"); };

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(action);
            Assert.Contains("System.Data.CustomClient", ex.Message);
        }
    }
}
