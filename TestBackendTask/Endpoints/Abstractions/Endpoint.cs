using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace TestBackendTask.Endpoints.Abstractions;

public class Endpoint : BaseEndpoint
{
    public override void Send(int statusCode, object? response)
    {
        _context.Response.StatusCode = statusCode;
        if(response is null)
        {
            _context.Response.Close();
            return;
        }

        if(response.GetType().IsGenericType)
        {
            var serializedObject = JsonSerializer.Serialize(response);
            var responseBytes = Encoding.UTF8.GetBytes(serializedObject);
            _context.Response.AddHeader("Content-Type", MediaTypeNames.Application.Json);
            _context.Response.OutputStream.Write(responseBytes);
        }
        else
        {
            _context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(response.ToString()));
        }

        _context.Response.Close();
    }

    public override void Handle(HttpListenerRequest request)
    {
        throw new NotImplementedException();
    }

    public override T? GetDataFromBody<T>() where T : class
    {
        return _context.Request.HasEntityBody ? JsonSerializer.Deserialize<T>(_context.Request.InputStream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }) : null;
    }
}