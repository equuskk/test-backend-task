using System.Net;
using TestBackendTask.App.Context;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.App.Endpoints.Report;

public class GetReportEndpoint : Endpoint
{
    private readonly ReportDbContext _dbContext;

    public GetReportEndpoint(ReportDbContext dbContext)
    {
        _dbContext = dbContext;
        Method = HttpMethod.Get.Method;
        Path = "/report/info";
    }

    public override void Handle(HttpListenerRequest request)
    {
        var id = _context.Request.QueryString.Get("id");
        if(string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var guid))
        {
            SendBadRequest("invalid guid");
            return;
        }

        var entity = _dbContext.Reports.FirstOrDefault(x => x.Id == guid);
        if(entity is null)
        {
            Send(201, "entity not found");
            return;
        }

        SendOk(new GetReportResponse
        {
            Percent = 42,
            Query = entity.Id,
            Result = new GetReportResponse.ResultResponse
            {
                UserId = entity.UserId,
                CountSignIn = 42
            }
        });
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