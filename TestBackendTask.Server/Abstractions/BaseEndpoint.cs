using System.Net;
using TestBackendTask.Server.Responses;

namespace TestBackendTask.Server.Abstractions;

public abstract class BaseEndpoint
{
    public string Path { get; init; }
    public string Method { get; init; }

    protected HttpListenerContext _context;

    public async Task Execute(HttpListenerContext context)
    {
        _context = context;
        await Handle(_context.Request);
    }

    public abstract Task Send(int statusCode, object? response);
    public abstract Task Handle(HttpListenerRequest request);

    public abstract T? GetDataFromBody<T>() where T : class;

    public Task SendOk(object? response)
    {
        return Send(200, response);
    }

    public Task SendBadRequest(object? response)
    {
        return Send(400, response);
    }

    public Task SendBadRequest(params string[] errors)
    {
        var response = new BadRequestResponse
        {
            Errors = errors
        };
        return SendBadRequest(response);
    }
}