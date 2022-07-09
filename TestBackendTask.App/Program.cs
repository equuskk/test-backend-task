using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using TestBackendTask.App.Context;
using TestBackendTask.App.Endpoints.Report;
using TestBackendTask.App.Options;
using TestBackendTask.Server;
using TestBackendTask.Server.Abstractions;

var host = CreateHostBuilder(args).Build();
MigrateDatabase<ReportDbContext>(host.Services);
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
                              
                              services.Configure<ReportOptions>(options =>
                              {
                                  var reportSection = context.Configuration.GetSection(ReportOptions.SectionName);
                                  if(!reportSection.Exists())
                                  {
                                      options.Delay = 60;
                                  }
                                  else
                                  {
                                      reportSection.Bind(options);
                                  }
                              });

                              services.AddDbContext<ReportDbContext>(options =>
                              {
                                  //TODO: add pgsql
                                  var preferDatabase = context.Configuration.GetSection("Database").GetValue<string>("Prefer");
                                  if(preferDatabase.Equals("sqlite", StringComparison.InvariantCultureIgnoreCase))
                                  {
                                      var connectionString = context.Configuration.GetConnectionString("sqlite");
                                      options.UseSqlite(connectionString);
                                  }
                              });

                              services.AddSingleton<HttpServer>();
                              services.AddSingleton<Random>();

                              services.AddTransient<Endpoint, CreateReportEndpoint>();
                              services.AddTransient<Endpoint, GetReportEndpoint>();

                              services.AddScoped<ILogger, Logger>(_ => new LoggerConfiguration()
                                                                       .WriteTo.Console()
                                                                       .CreateLogger());
                          });

    return hostBuilder;
}

static void MigrateDatabase<TContext>(IServiceProvider services) where TContext : DbContext
{
    var context = services.GetRequiredService<TContext>();
    context.Database.Migrate();
}