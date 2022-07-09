using System.Net;
using System.Net.Http.Json;
using TestBackendTask.App.Endpoints.Report;
using TestBackendTask.App.Tests.Base;
using TestBackendTask.Server.Responses;

namespace TestBackendTask.App.Tests.Endpoints;

[Collection(TestsCollection.CollectionName)]
public class GetReportTests : IClassFixture<BaseTestsFixture>
{
    public const string Endpoint = "/report/info";
    private readonly BaseTestsFixture _setup;

    public GetReportTests(BaseTestsFixture setup)
    {
        _setup = setup;
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("1234567")]
    public async Task InvalidGuid__BadRequest(string guid)
    {
        var response = await _setup.HttpClient.GetAsync($"{Endpoint}?id={guid}");
        var body = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        
        Assert.NotNull(body);
        Assert.Contains("invalid guid", body.Errors);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task NotExistingId__BadRequest()
    {
        var guid = Guid.NewGuid();
        var response = await _setup.HttpClient.GetAsync($"{Endpoint}?id={guid}");
        var body = await response.Content.ReadFromJsonAsync<BadRequestResponse>();
        
        Assert.NotNull(body);
        Assert.Contains("entity not found", body.Errors);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task ExistingId__Ok()
    {
        var guid = await CreateReport();
        var response = await _setup.HttpClient.GetAsync($"{Endpoint}?id={guid}");
        var body = await response.Content.ReadFromJsonAsync<GetReportResponse>();
        
        Assert.NotNull(body);
        Assert.Equal(guid, body.Query);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> CreateReport()
    {
        var request = new CreateReportRequest
        {
            UserId = Guid.NewGuid(),
            FromDate = DateTimeOffset.UtcNow,
            ToDate = DateTimeOffset.UtcNow.AddMinutes(5)
        };
        var response = await _setup.HttpClient.PostAsJsonAsync(CreateReportTests.Endpoint, request);

        var body = await response.Content.ReadAsStringAsync();
        return Guid.Parse(body);
    }
}