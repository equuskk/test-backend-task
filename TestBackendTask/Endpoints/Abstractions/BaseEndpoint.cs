using System.Net;
using TestBackendTask.Endpoints.Responses;

namespace TestBackendTask.Endpoints.Abstractions;

public abstract class BaseEndpoint
{
    public string Path { get; init; }
    public string Method { get; init; }

    protected HttpListenerContext _context;

    public void Execute(HttpListenerContext context)
    {
        _context = context;
        Handle(_context.Request);
    }

    public abstract void Send(int statusCode, object? response);
    public abstract void Handle(HttpListenerRequest request);

    public abstract T? GetDataFromBody<T>() where T : class;

    public void SendOk(object? response)
    {
        Send(200, response);
    }
    
    public void SendBadRequest(object? response)
    {
        Send(400, response);
    }
    
    public void SendBadRequest(params string[] errors)
    {
        var response = new BadRequestResponse()
        {
            Errors = errors
        };
        Send(400, response);
    }
}