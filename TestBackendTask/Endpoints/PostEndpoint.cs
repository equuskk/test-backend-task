using System.Net;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask.Endpoints;

public class PostEndpoint : Endpoint
{
    public PostEndpoint()
    {
        Method = HttpMethod.Post.Method;
        Path = "/post";
    }

    public override void Handle(HttpListenerRequest request)
    {
        var data = GetDataFromBody<object>();
        SendOk(data);
    }
}