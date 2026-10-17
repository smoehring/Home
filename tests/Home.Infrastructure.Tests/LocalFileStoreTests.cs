using Microsoft.Extensions.Options;
using Smoehring.Home.Infrastructure.Storage;

namespace Smoehring.Home.Infrastructure.Tests;

public sealed class LocalFileStoreTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "home-tests-" + Guid.NewGuid().ToString("N"));

    private LocalFileStore Create(long max = 1024) =>
        new(Options.Create(new StorageOptions { LocalPath = _dir, MaxFileSize = max }));

    public void Dispose() { if (Directory.Exists(_dir)) Directory.Delete(_dir, true); }

    [Fact]
    public async Task Save_and_read_roundtrip_with_hash()
    {
        var sut = Create();
        var stored = await sut.SaveAsync("a.txt", new MemoryStream("hello"u8.ToArray()));
        Assert.Equal(5, stored.Size);
        Assert.Equal("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824", stored.Sha256);
        await using var read = (await sut.OpenReadAsync("a.txt"))!;
        using var reader = new StreamReader(read);
        Assert.Equal("hello", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task Missing_file_returns_null() => Assert.Null(await Create().OpenReadAsync("nope.txt"));

    [Fact]
    public async Task Delete_removes_the_file_and_is_idempotent()
    {
        var sut = Create();
        await sut.SaveAsync("a.txt", new MemoryStream([1]));
        await sut.DeleteAsync("a.txt");
        await sut.DeleteAsync("a.txt");
        Assert.Null(await sut.OpenReadAsync("a.txt"));
    }

    [Theory]
    [InlineData("../evil.txt")]
    [InlineData("sub/evil.txt")]
    [InlineData("..\\evil.txt")]
    [InlineData("/etc/passwd")]
    [InlineData(".hidden")]
    [InlineData("")]
    public async Task Path_traversal_and_invalid_keys_are_rejected(string key) =>
        await Assert.ThrowsAnyAsync<ArgumentException>(() => Create().SaveAsync(key, new MemoryStream([1])));

    [Fact]
    public async Task Files_over_the_limit_are_rejected_and_leave_no_partial_file()
    {
        var sut = Create(max: 10);
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.SaveAsync("big.bin", new MemoryStream(new byte[11])));
        Assert.Null(await sut.OpenReadAsync("big.bin"));
        Assert.Empty(Directory.GetFiles(_dir));
    }

    [Fact]
    public async Task Existing_keys_are_not_overwritten()
    {
        var sut = Create();
        await sut.SaveAsync("a.txt", new MemoryStream([1]));
        await Assert.ThrowsAsync<IOException>(() => sut.SaveAsync("a.txt", new MemoryStream([2])));
    }
}
