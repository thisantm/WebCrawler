namespace WebCrawler.Interfaces;

public interface IHTMLSnapshotService
{
    Task SaveHtmlAsync(string html, int pageNumber);
}