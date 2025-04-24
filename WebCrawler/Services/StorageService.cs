using System.Text.Json;
using WebCrawler.Data;
using WebCrawler.Data.Entities;
using WebCrawler.Interfaces;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class StorageService : IStorageService
{
    private readonly string outputDir = Environment.GetEnvironmentVariable("JSON_FOLDER_PATH") ??
        throw new Exception("JSON_FOLDER_PATH must be set in enviroment variables");
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
    private static readonly AppDbContext dbContext = new();

    public StorageService()
    {
        Directory.CreateDirectory(outputDir);
    }

    public async Task<string> SaveToJsonAsync(List<ProxyInfo> proxies)
    {
        string path = Path.GetFullPath(Path.Combine(outputDir, $"proxies_{DateTime.Now:yyyyMMdd_HHmmss}.json"));
        var json = JsonSerializer.Serialize(proxies, jsonOptions);
        await File.WriteAllTextAsync(path, json);
        return path;
    }

    public async Task SaveExecutionLogAsync(CrawlerExecutionLog log)
    {
        try
        {
            dbContext.CrawlerExecutionLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
