using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebCrawler.Data.Entities;

public class CrawlerExecutionLog : IEntityTypeConfiguration<CrawlerExecutionLog>
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int PageCount { get; set; }
    public int TotalRows { get; set; }
    public string? JsonFilePath { get; set; }

    public void Configure(EntityTypeBuilder<CrawlerExecutionLog> builder)
    {
        builder.ToTable("CrawlerExecutionLogs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.StartTime)
               .IsRequired();
        builder.Property(e => e.EndTime)
               .IsRequired();
        builder.Property(e => e.PageCount)
               .IsRequired();
        builder.Property(e => e.TotalRows)
               .IsRequired();
        builder.Property(e => e.JsonFilePath)
               .IsRequired()
               .HasMaxLength(255);
    }
}

