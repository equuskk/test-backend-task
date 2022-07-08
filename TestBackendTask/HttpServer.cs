using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Serilog;

namespace TestBackendTask;

public class HttpServer
{
    private readonly HttpListener _listener;
    private readonly HttpOptions _options;
    private readonly ILogger _logger;
    private const string LocalhostServerUrl = "http://localhost:{0}/";

    public HttpServer(IOptions<HttpOptions> options, ILogger logger)
    {
        if(!HttpListener.IsSupported)
        {
            throw new ArgumentOutOfRangeException(nameof(Environment.OSVersion.Version), Environment.OSVersion.VersionString,
                                                  "Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
        }

        _listener = new HttpListener();
        _options = options.Value;
        _logger = logger;
    }

    public void Start()
    {
        var url = string.Format(LocalhostServerUrl, _options.Port);
        _listener.Prefixes.Add(url);
        _listener.Start();
        _logger.Information("Server started at {Url}", url);
        Receive();
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void Receive()
    {
        _listener.BeginGetContext(ListenerCallback, _listener);
    }

    private void ListenerCallback(IAsyncResult result)
    {
        if(!_listener.IsListening)
        {
            return;
        }

        var context = _listener.EndGetContext(result);

        //TODO: track request time
        _logger.Information("Request starting {HttpMethod} {HttpUrl}",
                            context.Request.HttpMethod, context.Request.Url);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.OutputStream.Write(Encoding.UTF8.GetBytes("test response"));
        context.Response.Close();

        _logger.Information("Request finished {HttpMethod} {HttpUrl} - {ResponseStatusCode}",
                            context.Request.HttpMethod, context.Request.Url, context.Response.StatusCode);

        Receive();
    }
}