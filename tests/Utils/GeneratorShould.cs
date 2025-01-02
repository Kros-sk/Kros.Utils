using FluentAssertions;
using NSubstitute.Core;
using NSubstitute.Extensions;
using System;
using Xunit;

namespace Kros.Utils.UnitTests.Utils;

public class GeneratorShould
{
    [Fact]
    public void IEnumerableGeneration()
    {
        const int count = 1_000_000;
        var n = Random.Shared.Next();
        var because = $"All values generated to {n}";

        var enumerable = Generator.GenerateEnumerableEager(_ => n, count);
        enumerable.Should().AllBeEquivalentTo(n, because);

        enumerable = Generator.GenerateEnumerableLazy(_ => n, count);
        enumerable.Should().AllBeEquivalentTo(n, because);
    }

    [Fact]
    public void EnsureStringGenContainsOnlyAllowedChars()
    {
        var rand = new Random();
        var chars = new[] { '1', '2', '3', 'a', 'b', 'c', 'x', 'y', 'z', '\n', '\t', ';', ':', '\'' };
        Array.Sort(chars); // Allow binary searching to reduce test time
        for (var i = 0; i < 3; i++)
        {
            var str = Generator.GenerateString(chars, 1_000, rand);
            foreach (var c in str)
            {
                chars.Should().Contain(c, "c is from chars");
            }
        }
    }
}
