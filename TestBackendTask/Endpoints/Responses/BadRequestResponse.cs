namespace TestBackendTask.Endpoints.Responses;

public class BadRequestResponse
{
    public IEnumerable<string> Errors { get; set; }
}