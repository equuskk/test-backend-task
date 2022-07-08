﻿using System.Net;
using Microsoft.Extensions.Options;
using Serilog;
using TestBackendTask.Endpoints.Abstractions;

namespace TestBackendTask;

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

        var selectedEndpoint = _endpoints.FirstOrDefault(x => x.Method == context.Request.HttpMethod &&
                                                              x.Path == context.Request.RawUrl);
        if(selectedEndpoint is null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
        }
        else
        {
            selectedEndpoint.Execute(context);
        }

        _logger.Information("Request finished {HttpMethod} {HttpUrl} - {ResponseStatusCode}",
                            context.Request.HttpMethod, context.Request.Url, context.Response.StatusCode);

        Receive();
    }
}