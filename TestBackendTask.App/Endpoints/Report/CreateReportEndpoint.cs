using System.Net;
using TestBackendTask.App.Context;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.App.Endpoints.Report;

public class CreateReportEndpoint : Endpoint
{
    private readonly ReportDbContext _dbContext;

    public CreateReportEndpoint(ReportDbContext dbContext, IJsonSerializer serializer) : base(serializer)
    {
        _dbContext = dbContext;
        Method = HttpMethod.Post.Method;
        Path = "/report/user_statistics";
    }

    public override async Task Handle(HttpListenerRequest request)
    {
        var data = GetDataFromBody<CreateReportRequest>();
        if(data is null)
        {
            await SendBadRequest("Body is empty");
            return;
        }

        if(!ValidateRequest(data, out var errors))
        {
            await SendBadRequest(errors);
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

    private bool ValidateRequest(CreateReportRequest req, out ICollection<string> errors)
    {
        errors = new List<string>();
        if(req.UserId == Guid.Empty)
        {
            errors.Add("Invalid user id");
        }

        if(req.FromDate > req.ToDate)
        {
            errors.Add("FromDate must be great than ToDate");
        }

        return !errors.Any();
    }
}

public class CreateReportRequest
{
    public Guid UserId { get; set; }
    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }
}