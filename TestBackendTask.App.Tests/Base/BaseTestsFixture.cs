using System.Diagnostics;
using System.Reflection;
using TestBackendTask.App.Endpoints.Report;

namespace TestBackendTask.App.Tests.Base;

public class BaseTestsFixture : IDisposable
{
    private readonly Process _process;
    internal readonly HttpClient HttpClient;

    public BaseTestsFixture()
    {
        HttpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080"),
            Timeout = TimeSpan.FromSeconds(5)
        };
        _process = new Process();

        var assembly = Assembly.GetAssembly(typeof(GetReportEndpoint));
        var directoryInfo = new DirectoryInfo(assembly.Location);
        _process.StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "TestBackendTask.App.dll",
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = directoryInfo.Parent.FullName
        };
        _process.Start();
    }

    public void Dispose()
    {
        _process.Kill();
    }
}