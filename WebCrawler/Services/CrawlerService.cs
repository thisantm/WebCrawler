using HtmlAgilityPack;
using Microsoft.Playwright;
using WebCrawler.Data.Entities;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class CrawlerService
{
    private const int MaxThreads = 3;
    private readonly StorageService _storageService = new();
    private readonly HtmlSnapshotService _snapshotService = new();

    public async Task RunAsync()
    {
        var execution = new CrawlerExecutionLog
        {
            StartTime = DateTime.Now
        };

        List<ProxyInfo> allProxies = [];
        List<string> pageUrls = await GetAllPageUrls();

        SemaphoreSlim semaphore = new(MaxThreads);
        List<Task> tasks = [];

        int pageIndex = 0;

        foreach (var url in pageUrls)
        {
            await semaphore.WaitAsync();
            int currentPage = ++pageIndex;

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    string html = await GetHtmlWithPlaywrightAsync(url);
                    await _snapshotService.SaveHtmlAsync(html, currentPage);
                    var proxies = HtmlParser.Parse(html);
                    lock (allProxies) allProxies.AddRange(proxies);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(tasks);

        execution.EndTime = DateTime.Now;
        execution.PageCount = pageUrls.Count;
        execution.TotalRows = allProxies.Count;
        execution.JsonFilePath = await _storageService.SaveToJsonAsync(allProxies);

        //await _storageService.SaveExecutionLogAsync(execution);
    }

    private static async Task<string> GetHtmlWithPlaywrightAsync(string url)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
        var page = await browser.NewPageAsync();
        await page.GotoAsync(url);
        await page.WaitForSelectorAsync("table");
        return await page.ContentAsync();
    }

    private static async Task<List<string>> GetAllPageUrls()
    {
        List<string> urls = [];
        string baseUrl = "https://proxyservers.pro/proxy/list/order/updated/order_dir/desc/page/";
        int currentPage = 1;

        while (true)
        {
            string url = baseUrl + currentPage;
            using HttpClient client = new();
            string html = await client.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'table table-hover')]/tbody/tr");
            if (rows == null || rows.Count == 0)
                break;

            urls.Add(url);
            currentPage++;
        }

        return urls;
    }
}
