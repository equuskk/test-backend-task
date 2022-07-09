using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace TestBackendTask.Server.Abstractions;

public class Endpoint : BaseEndpoint
{
    private readonly IJsonSerializer _jsonSerializer;

    public Endpoint(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }
    
    public override async Task Send(int statusCode, object? response)
    {
        _context.Response.StatusCode = statusCode;
        if(response is null)
        {
            _context.Response.Close();
            return;
        }

        var responseType = response.GetType();
        if(!responseType.IsPrimitive && !responseType.IsSealed) //TODO: proper type check?
        {
            var serializedObject = _jsonSerializer.Serialize<object?>(response);
            var responseBytes = Encoding.UTF8.GetBytes(serializedObject);
            _context.Response.ContentType = MediaTypeNames.Application.Json;
            await _context.Response.OutputStream.WriteAsync(responseBytes);
        }
        else
        {
            _context.Response.ContentType = MediaTypeNames.Text.Plain;
            await _context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(response.ToString()));
        }

        _context.Response.Close();
    }

    public override Task Handle(HttpListenerRequest request)
    {
        throw new NotImplementedException();
    }

    public override T? GetDataFromBody<T>() where T : class
    {
        try
        {
            return _context.Request.HasEntityBody
                           ? _jsonSerializer.Deserialize<T>(_context.Request.InputStream)
                           : null;
        }
        catch(JsonException)
        {
            return null;
        }
    }
}