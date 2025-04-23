using Microsoft.EntityFrameworkCore;
using WebCrawler.Data.Entities;

namespace WebCrawler.Data;

public class AppDbContext : DbContext
{
    public DbSet<CrawlerExecutionLog> ExecutionLogs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //options.UseSqlite("Data Source=crawler.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrawlerExecutionLog).Assembly);
    }
}



