namespace TestBackendTask.Context.Entities;

public class ReportResult
{
    public Guid Id { get; set; }
    public int CountSignIn { get; set; }

    public Guid ReportId { get; set; }
    public Report Report { get; set; }
}