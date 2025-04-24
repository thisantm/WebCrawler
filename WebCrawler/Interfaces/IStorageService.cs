using WebCrawler.Data.Entities;
using WebCrawler.Models;

namespace WebCrawler.Interfaces;

public interface IStorageService
{
    Task<string> SaveToJsonAsync(List<ProxyInfo> proxies);
    Task SaveExecutionLogAsync(CrawlerExecutionLog log);
}