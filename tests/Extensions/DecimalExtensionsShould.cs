using Kros.Extensions;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class DecimalExtensionsShould
    {
        [Fact]
        public void ReturnNumberRoundedDown()
        {
            decimal input = 10.124m;
            decimal actual = input.Round(2);
            decimal expected = 10.12m;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnNumberRoundedUp()
        {
            decimal input = 10.125m;
            decimal actual = input.Round(2);
            decimal expected = 10.13m;
            Assert.Equal(expected, actual);
        }
    }
}
