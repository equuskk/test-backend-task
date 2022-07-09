using System.Net;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask.Endpoints.Report;

public class GetReportEndpoint : Endpoint
{
    public GetReportEndpoint()
    {
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
        SendOk(new GetReportResponse()
        {
            Percent = 42,
            Query = guid,
            Result = new GetReportResponse.ResultResponse()
            {
                UserId = Guid.NewGuid(),
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