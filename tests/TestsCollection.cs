using Xunit;

namespace Kros.Utils.UnitTests;

[CollectionDefinition(Name)]
public class TestsCollection : ICollectionFixture<TestsFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
    public const string Name = "UnitTests";
}
