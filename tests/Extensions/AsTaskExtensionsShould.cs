using FluentAssertions;
using Kros.Extensions;
using System.Collections.Generic;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions
{
    public class AsTaskExtensionsShould
    {
        [Fact]
        public async void CreateTaskFromIEnumerable()
        {
            IEnumerable<int> data = new List<int>() { 1, 2, 5, 6 };

            IEnumerable<int> actual = await data.AsTask();

            actual.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async void CreateTaskFromSingleValue()
        {
            var foo = new
            {
                Name = "foo",
                LastName = "bar"
            };

            var actual = await foo.AsTaskSingleValue();

            actual.Should().BeEquivalentTo(foo);
        }
    }
}
