namespace TestBackendTask.Context.Entities;

public class Report
{
    public Guid Id { get; set; }
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public DateTimeOffset CreationDate { get; private set; }

    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }
    public Guid UserId { get; set; }

    public ReportResult? Result { get; set; }

    public Report()
    {
        CreationDate = DateTimeOffset.UtcNow;
    }
}