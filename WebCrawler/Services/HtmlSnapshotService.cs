using WebCrawler.Interfaces;

namespace WebCrawler.Services;

public class HTMLSnapshotService : IHTMLSnapshotService
{
    private readonly string snapshotDir = Environment.GetEnvironmentVariable("SNAPSHOT_FOLDER_PATH") ??
        throw new Exception("SNAPSHOT_FOLDER_PATH must be set in enviroment variables");

    public HTMLSnapshotService()
    {
        Directory.CreateDirectory(snapshotDir);
    }

    public async Task SaveHtmlAsync(string html, int pageNumber)
    {
        string filePath = Path.Combine(snapshotDir, $"page_{pageNumber}.html");
        await File.WriteAllTextAsync(filePath, html);
    }
}

