using System.Net;
using System.Net.Http.Json;
using TestBackendTask.App.Endpoints.Report;
using TestBackendTask.App.Tests.Base;
using TestBackendTask.Server.Responses;

namespace TestBackendTask.App.Tests.Endpoints;

[Collection(TestsCollection.CollectionName)]
public class CreateReportTests : IClassFixture<BaseTestsFixture>
{
    public const string Endpoint = "/report/user_statistics";
    public CreateReportTests(BaseTestsFixture setup)
    {
        _setup = setup;
    }

    private readonly BaseTestsFixture _setup;

    [Fact]
    public async Task InvalidUserId__BadRequest()
    {
        var request = new CreateReportRequest
        {
            UserId = Guid.Empty,
            FromDate = DateTimeOffset.UtcNow,
            ToDate = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var response = await _setup.HttpClient.PostAsJsonAsync(Endpoint, request);

        var body = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        Assert.NotNull(body);
        Assert.Contains("Invalid user id", body.Errors);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FromDateGreaterThanToDate__BadRequest()
    {
        var request = new CreateReportRequest
        {
            UserId = Guid.NewGuid(),
            FromDate = DateTimeOffset.UtcNow,
            ToDate = DateTimeOffset.UtcNow.AddMinutes(-5)
        };

        var response = await _setup.HttpClient.PostAsJsonAsync(Endpoint, request);

        var body = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        Assert.NotNull(body);
        Assert.Contains("FromDate must be great than ToDate", body.Errors);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidRequest__OK()
    {
        var request = new CreateReportRequest
        {
            UserId = Guid.NewGuid(),
            FromDate = DateTimeOffset.UtcNow,
            ToDate = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        var response = await _setup.HttpClient.PostAsJsonAsync(Endpoint, request);

        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(body);
        Assert.True(Guid.TryParse(body, out _));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}