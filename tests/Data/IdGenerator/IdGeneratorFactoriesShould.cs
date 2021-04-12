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
        [Fact]
        public void GetFactoryByConnection()
        {
            using (var conn = new SqlConnection())
            {
                var factory = IdGeneratorFactories.GetFactory(typeof(int), conn);

                factory.Should().NotBeNull();
            }
        }

        [Fact]
        public void GetFactoryByAdoClientName()
        {
            var factory = IdGeneratorFactories.GetFactory(typeof(int), "connectionstring", SqlServerDataHelper.ClientId);

            factory.Should().NotBeNull();
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
