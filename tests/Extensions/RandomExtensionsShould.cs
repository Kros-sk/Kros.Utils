using FluentAssertions;
using Kros.Extensions;
using System;
using System.Linq;
using Xunit;

namespace Kros.Utils.UnitTests.Extensions;

public class RandomExtensionsShould
{
    [Fact]
    public void EnsureValidIListSelection()
    {
        var rand = new Random();
        var nums = Enumerable.Range(0, 100).ToArray();

        for (var i = 0; i < nums.Length * 100; i++)
        {
            var n = rand.SelectFrom(nums);
            (n >= nums[0] && n <= nums[^1])
                .Should()
                .BeTrue("The IList is a range and the method should only choose from within the IList");
        }
    }

    [Fact]
    public void EnsureProperShuffle()
    {
        var rand = new Random();
        var numsArr = Enumerable.Range(0, 100).ToArray();
        var numsList = ((int[])numsArr.Clone()).ToList();
        numsArr.Should().BeEquivalentTo(numsList, "Cloned and turned into list");

        rand.Shuffle(numsList);

        // What are the chances of shuffle returning the same thing? Haha I hope not
        numsList.Should().NotBeInAscendingOrder(n => n, "Shuffled");
    }
}
