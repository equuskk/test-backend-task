namespace TestBackendTask.Server.Responses;

public class BadRequestResponse
{
    public IEnumerable<string> Errors { get; set; }
}