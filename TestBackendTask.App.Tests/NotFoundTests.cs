using System.Net;
using TestBackendTask.App.Tests.Base;

namespace TestBackendTask.App.Tests;

[Collection(TestsCollection.CollectionName)]
public class NotFoundTests : IClassFixture<BaseTestsFixture>
{
    private readonly BaseTestsFixture _setup;

    public NotFoundTests(BaseTestsFixture setup)
    {
        _setup = setup;
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/test")]
    [InlineData("/not-found")]
    [InlineData("/abc123/sdf")]
    [InlineData("/report/test")]
    public async Task Test(string url)
    {
        var response = await _setup.HttpClient.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}