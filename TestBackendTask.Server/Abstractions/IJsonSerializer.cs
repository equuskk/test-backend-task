namespace TestBackendTask.Server.Abstractions;

public interface IJsonSerializer
{
    string Serialize<T>(object value);
    T Deserialize<T>(Stream stream);
}