namespace WebCrawler.Services;

public class HtmlSnapshotService
{
    private readonly string snapshotDir = Path.Combine("output", "snapshots");

    public HtmlSnapshotService()
    {
        Directory.CreateDirectory(snapshotDir);
    }

    public async Task SaveHtmlAsync(string html, int pageNumber)
    {
        string filePath = Path.Combine(snapshotDir, $"page_{pageNumber}.html");
        await File.WriteAllTextAsync(filePath, html);
    }
}

