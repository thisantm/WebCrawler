using System.Text.Json;
using WebCrawler.Interfaces;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class StorageService : IStorageService
{
    private readonly string outputDir = Path.Combine("output", "results");
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    public StorageService()
    {
        Directory.CreateDirectory(outputDir);

        //using var db = new AppDbContext();
        //db.Database.EnsureCreated();
    }

    public async Task<string> SaveToJsonAsync(List<ProxyInfo> proxies)
    {
        string path = Path.Combine(outputDir, $"proxies_{DateTime.Now:yyyyMMdd_HHmmss}.json");
        var json = JsonSerializer.Serialize(proxies, jsonOptions);
        await File.WriteAllTextAsync(path, json);
        return path;
    }

    //public async Task SaveExecutionLogAsync(CrawlerExecutionLog log)
    //{
    //    using var db = new AppDbContext();
    //    db.ExecutionLogs.Add(log);
    //    await db.SaveChangesAsync();
    //}
}
