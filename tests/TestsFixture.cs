using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace Kros.Utils.UnitTests;

public sealed class TestsFixture : IAsyncDisposable
{
    private readonly MsSqlContainer _msSqlContainer;

    public TestsFixture()
    {
        _msSqlContainer = CreateMsSqlContainer();
    }

    private static MsSqlContainer CreateMsSqlContainer()
    {
        MsSqlContainer msSqlContainer = new MsSqlBuilder().Build();
        msSqlContainer.StartAsync().Wait();
        return msSqlContainer;
    }

    internal string GetConnectionString() => _msSqlContainer.GetConnectionString();

    public async ValueTask DisposeAsync() => await _msSqlContainer.DisposeAsync();
}
