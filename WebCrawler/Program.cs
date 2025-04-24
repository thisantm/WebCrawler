using WebCrawler.Services;

var crawler = new CrawlerService(
    new HTMLParserService(),
    new HTMLSnapshotService(),
    new StorageService()
);

Console.WriteLine("Iniciando o webcrawler...");

await crawler.RunAsync();

Console.WriteLine("Webcrawler finalizado com sucesso.");
