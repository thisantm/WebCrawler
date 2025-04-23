using WebCrawler.Models;

namespace WebCrawler.Interfaces;

public interface IStorageService
{
    Task<string> SaveToJsonAsync(List<ProxyInfo> proxies);
}