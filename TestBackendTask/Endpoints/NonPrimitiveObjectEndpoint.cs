using System.Net;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask.Endpoints;

public class NonPrimitiveObjectEndpoint : Endpoint
{
    public NonPrimitiveObjectEndpoint()
    {
        Method = HttpMethod.Get.Method;
        Path = "/test";
    }

    public override void Handle(HttpListenerRequest request)
    {
        SendOk(new
        {
            Id = 123,
            Text = "123",
            Guid = Guid.NewGuid(),
            Nested = new
            {
                Title = "Nested Title",
                Sub = "Nested Sub"
            }
        });
    }
}