using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestBackendTask.App.Context;
using TestBackendTask.App.Context.Entities;
using TestBackendTask.App.Options;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.App.Endpoints.Report;

public class GetReportEndpoint : Endpoint
{
    private readonly ReportDbContext _dbContext;
    private readonly Random _random;
    private readonly ReportOptions _options;

    public GetReportEndpoint(ReportDbContext dbContext, IOptions<ReportOptions> options, Random random,
                             IJsonSerializer serializer) : base(serializer)
    {
        _dbContext = dbContext;
        _random = random;
        _options = options.Value;
        Method = HttpMethod.Get.Method;
        Path = "/report/info";
    }

    public override async Task Handle(HttpListenerRequest request)
    {
        var id = _context.Request.QueryString.Get("id");
        if(string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
        {
            await SendBadRequest("invalid guid");
            return;
        }

        var entity = await _dbContext.Reports
                               .Include(x => x.Result)
                               .FirstOrDefaultAsync(x => x.Id == guid);
        if(entity is null)
        {
            await SendBadRequest("entity not found");
            return;
        }

        if(entity.Result is not null)
        {
            await SendOk(MapToResponse(entity));
            return;
        }

        var diffInSeconds = (DateTimeOffset.UtcNow - entity.CreationDate).TotalSeconds;
        var percent = (int)((diffInSeconds / _options.Delay) * 100);
        if(percent < 100)
        {
            await SendOk(MapToResponse(entity, percent));
            return;
        }

        entity.Result = new ReportResult
        {
            CountSignIn = _random.Next(10, 200)
        };
        await _dbContext.SaveChangesAsync();
        await SendOk(MapToResponse(entity));
    }

    private GetReportResponse MapToResponse(Context.Entities.Report report, int percent = 100)
    {
        var response = new GetReportResponse
        {
            Percent = percent,
            Query = report.Id,
        };

        if(report.Result is not null)
        {
            response.Result = new GetReportResponse.ResultResponse
            {
                UserId = report.UserId,
                CountSignIn = report.Result.CountSignIn
            };
        }

        return response;
    }
}

public class GetReportResponse
{
    public Guid Query { get; set; }
    public int Percent { get; set; }

    public ResultResponse Result { get; set; }

    public class ResultResponse
    {
        public Guid UserId { get; set; }
        public int CountSignIn { get; set; }
    }
}