using WebCrawler.Services;

var crawler = new CrawlerService(
    new HTMLParserService(),
    new HTMLSnapshotService(),
    new StorageService()
);
await crawler.RunAsync();

Console.WriteLine("Webcrawler finalizado com sucesso.");
