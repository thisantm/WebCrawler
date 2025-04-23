using HtmlAgilityPack;
using WebCrawler.Models;

namespace WebCrawler.Interfaces;

public interface IHTMLParserService
{
    List<ProxyInfo> Parse(HtmlDocument doc);
}

