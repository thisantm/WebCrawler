using Microsoft.EntityFrameworkCore;
using WebCrawler.Data.Entities;

namespace WebCrawler.Data;

public class AppDbContext : DbContext
{
    public DbSet<CrawlerExecutionLog> CrawlerExecutionLogs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING_CRAWLER_DB") ??
                throw new Exception("CONNECTION_STRING_CRAWLER_DB must be set in enviroment variables"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrawlerExecutionLog).Assembly);
    }
}



