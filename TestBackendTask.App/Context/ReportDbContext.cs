using Microsoft.EntityFrameworkCore;
using TestBackendTask.App.Context.Entities;

namespace TestBackendTask.App.Context;

public class ReportDbContext : DbContext
{
    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportResult> ReportResults { get; set; }

    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var reportBuilder = modelBuilder.Entity<Report>();
        reportBuilder.HasKey(x => x.Id);
        reportBuilder.HasOne(x => x.Result)
                     .WithOne(x => x.Report)
                     .IsRequired(false);

        var resultBuilder = modelBuilder.Entity<ReportResult>();
        resultBuilder.HasKey(x => x.Id);
    }
}