using Kros.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class AsTaskExtensionsShould
    {
        [Fact]
        public async Task CreateTaskFromIEnumerable()
        {
            IEnumerable<int> data = new List<int>() { 1, 2, 5, 6 };

            IEnumerable<int> actual = await data.AsTask();

            Assert.Equal(data, actual);
        }

        [Fact]
        public async Task CreateTaskFromSingleValue()
        {
            var foo = new
            {
                Name = "foo",
                LastName = "bar"
            };

            var actual = await foo.AsTaskSingleValue();

            Assert.Equal(foo, actual);
        }
    }
}
