using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using TestBackendTask;
using TestBackendTask.Endpoints;
using TestBackendTask.Endpoints.Abstractions;

var host = CreateHostBuilder(args).Build();
host.Services.GetService<HttpServer>().Start();
Console.ReadKey(); //TODO: fix server cancellation

static IHostBuilder CreateHostBuilder(string[] args)
{
    var hostBuilder = Host.CreateDefaultBuilder(args)
                          .ConfigureAppConfiguration((_, builder) =>
                          {
                              builder.SetBasePath(Directory.GetCurrentDirectory());
                          })
                          .ConfigureServices((context, services) =>
                          {
                              services.Configure<HttpOptions>(options =>
                              {
                                  var httpSection = context.Configuration.GetSection(HttpOptions.SectionName);
                                  if(!httpSection.Exists())
                                  {
                                      throw new ArgumentNullException(nameof(httpSection),
                                                                      $"Add '{HttpOptions.SectionName}' section to setting");
                                  }
                                  httpSection.Bind(options);
                              });
                              
                              services.AddSingleton<HttpServer>();
                              
                              services.AddTransient<Endpoint, NonPrimitiveObjectEndpoint>(); //TODO: endpoint discovery?
                              services.AddTransient<Endpoint, PrimitiveObjectEndpoint>();
                              services.AddTransient<Endpoint, PostEndpoint>();
                              
                              services.AddScoped<ILogger, Logger>(_ => new LoggerConfiguration()
                                                                         .WriteTo.Console()
                                                                         .CreateLogger());
                          });

    return hostBuilder;
}
