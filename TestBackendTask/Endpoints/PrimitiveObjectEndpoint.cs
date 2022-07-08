using System.Net;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask.Endpoints;

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