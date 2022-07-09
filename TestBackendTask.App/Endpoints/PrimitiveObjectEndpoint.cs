using System.Net;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.App.Endpoints;

public class PrimitiveObjectEndpoint : Endpoint
{
    public PrimitiveObjectEndpoint()
    {
        Method = HttpMethod.Get.Method;
        Path = "/primitive";
    }

    public override void Handle(HttpListenerRequest request)
    {
        SendOk("ok");
    }
}