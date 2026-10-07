using System;
using Kros.Caching;
using Xunit;

namespace Kros.Utils.UnitTests.Caching
{
    public class CacheShould
    {
        [Fact]
        public void GetValue()
        {
            var cache = new Cache<int, Foo>();
            var expected = "111";

            var actual = cache.Get(111, () => new Foo() { Value = "111" });

            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void ReturnCachedValue()
        {
            var cache = new Cache<string, Foo>();

            var actual = cache.Get("car", ()=> new Foo() { Value = "car" });

            actual = cache.Get("car", () => new Foo() { Value = "car_new"});

            Assert.Equal("car", actual.Value);
        }

        [Fact]
        public void UseFactoryForGetingValueAfterClear()
        {
            var cache = new Cache<string, Foo>();

            var actual = cache.Get("car", () => new Foo() { Value = "car" });

            cache.Clear();

            actual = cache.Get("car", () => new Foo() { Value = "car_new" });

            Assert.Equal("car_new", actual.Value);
        }

        [Fact]
        public void UseCustomEqualityComparer()
        {
            var cache = new Cache<string, int>(StringComparer.InvariantCultureIgnoreCase);

            cache.Get("test", () => 111);

            var actual = cache.Get("TeSt", () => 999);

            Assert.Equal(111, actual);
        }

        class Foo
        {
            public string Value { get; set; } = string.Empty;
        }
    }
}
