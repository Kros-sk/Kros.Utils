using FluentAssertions;
using Kros.Extensions;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class NumberExtensionsShould
    {
        #region decimal
        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.13)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDecimalToTwoPlace(decimal input, decimal expected)
        {   
            decimal actual = input.Round(2);            
            actual.Should().Be(expected);            
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(11.1, 11)]
        [InlineData(11.2, 11)]
        [InlineData(11.3, 11)]
        [InlineData(11.4, 11)]
        [InlineData(11.5, 12)]
        [InlineData(11.6, 12)]
        [InlineData(11.7, 12)]
        [InlineData(11.8, 12)]
        [InlineData(11.9, 12)]
        public void ReturnRoundedDecimalTIintegralPart(decimal input, decimal expected)
        {
            decimal actual = input.Round();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.12)]
        [InlineData(11.127, 11.12)]
        [InlineData(11.128, 11.12)]
        [InlineData(11.129, 11.12)]
        public void ReturnRoundedDecimalToZeroTwoPlace(decimal input, decimal expected)
        {
            decimal actual = input.Round(2, System.MidpointRounding.ToZero);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDecimalToEvenTwoPlace(decimal input, decimal expected)
        {
            decimal actual = input.Round(2, System.MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.12)]
        [InlineData(11.127, 11.12)]
        [InlineData(11.128, 11.12)]
        [InlineData(11.129, 11.12)]
        public void ReturnRoundedDecimalToNegativeInfinityTwoPlace(decimal input, decimal expected)
        {
            decimal actual = input.Round(2, System.MidpointRounding.ToNegativeInfinity);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.13)]
        [InlineData(11.122, 11.13)]
        [InlineData(11.123, 11.13)]
        [InlineData(11.124, 11.13)]
        [InlineData(11.125, 11.13)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDecimalToPositiveInfinityTwoPlace(decimal input, decimal expected)
        {
            decimal actual = input.Round(2, System.MidpointRounding.ToPositiveInfinity);
            actual.Should().Be(expected);
        }
        #endregion

        #region double
        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.13)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDoubleToTwoPlace(double input, double expected)
        {
            double actual = input.Round(2);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10, 10)]
        [InlineData(11.1, 11)]
        [InlineData(11.2, 11)]
        [InlineData(11.3, 11)]
        [InlineData(11.4, 11)]
        [InlineData(11.5, 12)]
        [InlineData(11.6, 12)]
        [InlineData(11.7, 12)]
        [InlineData(11.8, 12)]
        [InlineData(11.9, 12)]
        public void ReturnRoundedDoubleTIintegralPart(double input, double expected)
        {
            double actual = input.Round();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.12)]
        [InlineData(11.127, 11.12)]
        [InlineData(11.128, 11.12)]
        [InlineData(11.129, 11.12)]
        public void ReturnRoundedDoubleToZeroTwoPlace(double input, double expected)
        {
            double actual = input.Round(2, System.MidpointRounding.ToZero);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDoubleToEvenTwoPlace(double input, double expected)
        {
            double actual = input.Round(2, System.MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.12)]
        [InlineData(11.122, 11.12)]
        [InlineData(11.123, 11.12)]
        [InlineData(11.124, 11.12)]
        [InlineData(11.125, 11.12)]
        [InlineData(11.126, 11.12)]
        [InlineData(11.127, 11.12)]
        [InlineData(11.128, 11.12)]
        [InlineData(11.129, 11.12)]
        public void ReturnRoundedDoubleToNegativeInfinityTwoPlace(double input, double expected)
        {
            double actual = input.Round(2, System.MidpointRounding.ToNegativeInfinity);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(11.12, 11.12)]
        [InlineData(11.121, 11.13)]
        [InlineData(11.122, 11.13)]
        [InlineData(11.123, 11.13)]
        [InlineData(11.124, 11.13)]
        [InlineData(11.125, 11.13)]
        [InlineData(11.126, 11.13)]
        [InlineData(11.127, 11.13)]
        [InlineData(11.128, 11.13)]
        [InlineData(11.129, 11.13)]
        public void ReturnRoundedDoubleToPositiveInfinityTwoPlace(double input, double expected)
        {
            double actual = input.Round(2, System.MidpointRounding.ToPositiveInfinity);
            actual.Should().Be(expected);
        }
        #endregion
    }
}
