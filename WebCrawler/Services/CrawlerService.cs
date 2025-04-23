using HtmlAgilityPack;
using Microsoft.Playwright;
using WebCrawler.Data.Entities;
using WebCrawler.Interfaces;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class CrawlerService : ICrawlerService
{
    private readonly IHTMLParserService _htmlParser;
    private readonly IHTMLSnapshotService _snapshotService;
    private readonly IStorageService _storageService;

    public CrawlerService(IHTMLParserService htmlParser,
                          IHTMLSnapshotService snapshotService,
                          IStorageService storageService)
    {
        _htmlParser = htmlParser;
        _snapshotService = snapshotService;
        _storageService = storageService;
    }

    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static readonly int MaxThreads = int.Parse(Environment.GetEnvironmentVariable("MAX_THREADS") ??
        throw new Exception("MAX_THREADS must be set in enviroment variables"));
    private static readonly string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
        throw new Exception("BASE_URL must be set in enviroment variables");
    private static readonly string rowsXPath = Environment.GetEnvironmentVariable("ROWS_XPATH") ??
        throw new Exception("ROWS_XPATH must be set in enviroment variables");
    private static readonly string paginationXPath = Environment.GetEnvironmentVariable("PAGINATION_XPATH") ??
        throw new Exception("PAGINATION_XPATH must be set in enviroment variables");

    public async Task RunAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });

        var execution = new CrawlerExecutionLog
        {
            StartTime = DateTime.Now
        };

        List<ProxyInfo> allProxies = [];
        List<Task> tasks = [];
        SemaphoreSlim semaphore = new(MaxThreads);

        int currentPage = 1;
        int totalPages = 0;
        int? lastPage = await GetLastPageNumberAsync();
        Console.WriteLine(lastPage != null
            ? $"Última página detectada: {lastPage}"
            : "Não foi possível detectar a última página. Usando modo automático.");

        bool hasMorePages = true;

        while (true)
        {
            if (lastPage != null && currentPage > lastPage) { break; }
            if (!hasMorePages) { break; }

            await semaphore.WaitAsync();
            var pageIndex = currentPage++;

            var task = Task.Run(async () =>
            {
                try
                {
                    if (!hasMorePages) { return; }

                    string url = baseUrl + pageIndex;
                    Console.WriteLine($"Verificando página: {url}");

                    string html = await GetHtmlWithPlaywrightAsync(url);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var rows = doc.DocumentNode.SelectNodes(rowsXPath);

                    if ((lastPage == null && (rows == null || rows.Count == 0)))
                    {
                        hasMorePages = false;
                        return;
                    }

                    await _snapshotService.SaveHtmlAsync(html, pageIndex);
                    var proxies = _htmlParser.Parse(doc);
                    totalPages++;
                    lock (allProxies) allProxies.AddRange(proxies);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar página {pageIndex}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        execution.EndTime = DateTime.Now;
        execution.PageCount = totalPages;
        execution.TotalRows = allProxies.Count;
        execution.JsonFilePath = await _storageService.SaveToJsonAsync(allProxies);

        Console.WriteLine($"Total de páginas processadas: {execution.PageCount}");
        Console.WriteLine($"Total de proxies encontrados: {allProxies.Count}");
        Console.WriteLine($"Arquivo JSON salvo em: {execution.JsonFilePath}");
        Console.WriteLine($"Tempo total de execução: {execution.EndTime - execution.StartTime}");

        // await _storageService.SaveExecutionLogAsync(execution);
    }

    private static async Task<string> GetHtmlWithPlaywrightAsync(string url)
    {
        if (_browser == null)
        {
            throw new InvalidOperationException("Playwright browser is not initialized.");
        }

        var page = await _browser.NewPageAsync();
        await page.GotoAsync(url);
        await page.WaitForSelectorAsync("table");
        return await page.ContentAsync();
    }

    private static async Task<int?> GetLastPageNumberAsync()
    {
        string url = baseUrl + "1";
        string html = await GetHtmlWithPlaywrightAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var pageLinks = doc.DocumentNode.SelectNodes(paginationXPath);

        if (pageLinks == null) { return null; }

        var pageNumbers = pageLinks
            .Select(link => link.InnerText.Trim())
            .Where(text => int.TryParse(text, out _))
            .Select(int.Parse)
            .ToList();

        if (pageNumbers.Count == 0) { return null; }

        return pageNumbers.Max();
    }
}
