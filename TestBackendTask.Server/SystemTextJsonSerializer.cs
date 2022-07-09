using System.Text.Json;
using JorgeSerrano.Json;
using TestBackendTask.Server.Abstractions;

namespace TestBackendTask.Server;

public class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonSerializerOptions _deserializerOptions;

    public SystemTextJsonSerializer()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };
        _deserializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public string Serialize<T>(object? value)
    {
        return JsonSerializer.Serialize(value, _serializerOptions);
    }

    public T Deserialize<T>(Stream stream)
    {
        return JsonSerializer.Deserialize<T>(stream, _deserializerOptions);
    }
}