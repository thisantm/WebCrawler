using WebCrawler.Services;

var crawler = new CrawlerService();
await crawler.RunAsync();

Console.WriteLine("Webcrawler finalizado com sucesso.");
