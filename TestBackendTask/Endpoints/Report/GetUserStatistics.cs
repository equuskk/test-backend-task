using System.Net;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask.Endpoints.Report;

public class GetUserStatistics : Endpoint
{
    public GetUserStatistics()
    {
        Method = HttpMethod.Post.Method;
        Path = "/report/user_statistics";
    }

    public override void Handle(HttpListenerRequest request)
    {
        var data = GetDataFromBody<GetUserStatisticsRequest>();
        if(data is null)
        {
            SendBadRequest("Body is empty");
            return;
        }
        SendOk(data.UserId);
    }
}

public class GetUserStatisticsRequest
{
    public Guid UserId { get; set; }
    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }
}