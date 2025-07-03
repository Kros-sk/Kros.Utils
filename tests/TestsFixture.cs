using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using Xunit;

namespace Kros.Utils.UnitTests;

public sealed class TestsFixture : IAsyncDisposable, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;

    public TestsFixture()
    {
        _msSqlContainer = new MsSqlBuilder().Build();
    }

    internal string GetConnectionString() => _msSqlContainer.GetConnectionString();

    public async ValueTask InitializeAsync() => await _msSqlContainer.StartAsync();

    public async ValueTask DisposeAsync() => await _msSqlContainer.DisposeAsync();
}
