using FluentAssertions;
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
        public void GetFactoryByConnection(Type dataType)
        {
            using (var conn = new SqlConnection())
            {
                var factory = IdGeneratorFactories.GetFactory(dataType, conn);

                factory.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(long))]
        public void GetFactoryByAdoClientName(Type dataType)
        {
            var factory = IdGeneratorFactories.GetFactory(dataType, "connectionstring", SqlServerDataHelper.ClientId);

            factory.Should().NotBeNull();
        }

        [Fact]
        public void ThrowExceptionWhenDataTypeIsNotRegistered()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(DateTime), conn); };

                action.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void ThrowExceptionWhenConnectionIsNotRegisterd()
        {
            using (var conn = new CustomConnection())
            {
                Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(int), conn); };

                action.Should().Throw<InvalidOperationException>()
                    .WithMessage("*CustomConnection*");
            }
        }

        [Fact]
        public void ThrowExceptionWhenAdoClientNameIsNotRegistered()
        {
            Action action = () => { var factory = IdGeneratorFactories.GetFactory(typeof(int), "constring", "System.Data.CustomClient"); };

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("*System.Data.CustomClient*");
        }
    }
}
