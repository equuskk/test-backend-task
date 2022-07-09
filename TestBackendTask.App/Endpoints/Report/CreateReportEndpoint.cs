using System.Net;
using TestBackendTask.App.Context;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.App.Endpoints.Report;

public class CreateReportEndpoint : Endpoint
{
    private readonly ReportDbContext _dbContext;

    public CreateReportEndpoint(ReportDbContext dbContext)
    {
        _dbContext = dbContext;
        Method = HttpMethod.Post.Method;
        Path = "/report/user_statistics";
    }

    public override async Task Handle(HttpListenerRequest request)
    {
        var data = GetDataFromBody<GetUserStatisticsRequest>();
        if(data is null)
        {
            await SendBadRequest("Body is empty");
            return;
        }

        var entity = new Context.Entities.Report
        {
            Id = Guid.NewGuid(),
            FromDate = data.FromDate,
            ToDate = data.ToDate,
            UserId = data.UserId
        };
        _dbContext.Reports.Add(entity);
        await _dbContext.SaveChangesAsync();

        await SendOk(entity.Id);
    }
}

public class GetUserStatisticsRequest
{
    public Guid UserId { get; set; }
    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }
}