using FluentAssertions;
using Kros.Extensions;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class NumberExtensionsShould
    {
        [Theory]
        [InlineData(10.105, 10.11)]
        [InlineData(10.115, 10.12)]
        [InlineData(10.125, 10.13)]
        [InlineData(10.135, 10.14)]
        [InlineData(10.145, 10.15)]
        public void RoundDecimalToTwoPlaces(decimal input, decimal expected)
        {
            decimal actual = input.Round(2);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.5, 11)]
        [InlineData(11.5, 12)]
        [InlineData(12.5, 13)]
        [InlineData(13.5, 14)]
        [InlineData(14.5, 15)]
        public void RoundDecimalToInteger(decimal input, decimal expected)
        {
            decimal actual = input.Round();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.105, 10.10)]
        [InlineData(10.115, 10.12)]
        [InlineData(10.125, 10.12)]
        [InlineData(10.135, 10.14)]
        [InlineData(10.145, 10.14)]
        public void RoundDecimalToTwoPlaces_MidpointRoundingToEven(decimal input, decimal expected)
        {
            decimal actual = input.Round(2, MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.5, 10)]
        [InlineData(11.5, 12)]
        [InlineData(12.5, 12)]
        [InlineData(13.5, 14)]
        [InlineData(14.5, 14)]
        public void RoundDecimalToInteger_MidpointRoundingToEven(decimal input, decimal expected)
        {
            decimal actual = input.Round(MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.105, 10.11)]
        [InlineData(10.115, 10.12)]
        [InlineData(10.125, 10.13)]
        [InlineData(10.135, 10.14)]
        [InlineData(10.145, 10.15)]
        public void RoundDoubleToTwoPlaces(double input, double expected)
        {
            double actual = input.Round(2);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.5, 11)]
        [InlineData(11.5, 12)]
        [InlineData(12.5, 13)]
        [InlineData(13.5, 14)]
        [InlineData(14.5, 15)]
        public void RoundDoubleToInteger(double input, double expected)
        {
            double actual = input.Round();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.105, 10.10)]
        [InlineData(10.115, 10.12)]
        [InlineData(10.125, 10.12)]
        [InlineData(10.135, 10.14)]
        [InlineData(10.145, 10.14)]
        public void RoundDoubleToTwoPlaces_MidpointRoundingToEven(double input, double expected)
        {
            double actual = input.Round(2, MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(10.5, 10)]
        [InlineData(11.5, 12)]
        [InlineData(12.5, 12)]
        [InlineData(13.5, 14)]
        [InlineData(14.5, 14)]
        public void RoundDoubleToInteger_MidpointRoundingToEven(double input, double expected)
        {
            double actual = input.Round(MidpointRounding.ToEven);
            actual.Should().Be(expected);
        }
    }
}
