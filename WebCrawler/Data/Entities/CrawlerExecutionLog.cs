using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebCrawler.Data.Entities;

public class CrawlerExecutionLog : IEntityTypeConfiguration<CrawlerExecutionLog>
{
	public long Id { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public int PageCount { get; set; }
	public int TotalRows { get; set; }
	public string? JsonFilePath { get; set; }

	public void Configure(EntityTypeBuilder<CrawlerExecutionLog> builder)
	{
		builder.ToTable("CrawlerExecutionLogs");
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id)
			   .HasColumnType("bigserial")
			   .ValueGeneratedOnAdd()
			   .IsRequired();
		builder.Property(e => e.StartTime)
			   .HasColumnType("timestamp without time zone")
			   .IsRequired();
		builder.Property(e => e.EndTime)
			   .HasColumnType("timestamp without time zone")
			   .IsRequired();
		builder.Property(e => e.PageCount)
			   .HasColumnType("int")
			   .IsRequired();
		builder.Property(e => e.TotalRows)
			   .HasColumnType("int")
			   .IsRequired();
		builder.Property(e => e.JsonFilePath)
			   .HasColumnType("varchar")
			   .IsRequired()
			   .HasMaxLength(255);
	}
}

