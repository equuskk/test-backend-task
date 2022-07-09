using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Options;
using Serilog;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.Server;

public class HttpServer
{
    private const string LocalhostServerUrl = "http://localhost:{0}/";

    private readonly HttpListener _listener;
    private readonly HttpOptions _options;
    private readonly ILogger _logger;
    private readonly IEnumerable<Endpoint> _endpoints;

    public HttpServer(IOptions<HttpOptions> options, ILogger logger, IEnumerable<Endpoint> endpoints)
    {
        if(!HttpListener.IsSupported)
        {
            throw new ArgumentOutOfRangeException(nameof(Environment.OSVersion.Version), Environment.OSVersion.VersionString,
                                                  "Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
        }

        _listener = new HttpListener();
        _options = options.Value;
        _logger = logger;
        _endpoints = endpoints;
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
        while(true)
        {
            var context = _listener.GetContext();
            ListenerCallback(context);
        }
    }

    private void ListenerCallback(HttpListenerContext context)
    {
        if(!_listener.IsListening)
        {
            return;
        }

        Task.Factory.StartNew(async () =>
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.Information("Request starting {HttpMethod} {HttpUrl}",
                                context.Request.HttpMethod, context.Request.Url);

            var selectedEndpoint = _endpoints.FirstOrDefault(x => x.Method == context.Request.HttpMethod &&
                                                                  x.Path == context.Request.Url.AbsolutePath);
            if(selectedEndpoint is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
            else
            {
                try
                {
                    await selectedEndpoint.Execute(context);
                }
                catch(Exception)
                {
                    if(context.Response.StatusCode == 0)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Close();
                    }
                }
            }

            stopWatch.Stop();

            _logger.Information("Request finished {HttpMethod} {HttpUrl} - {ResponseStatusCode}. Elapsed {ElapsedTime}ms",
                                context.Request.HttpMethod, context.Request.Url,
                                context.Response.StatusCode, stopWatch.ElapsedMilliseconds);
        });
    }
}