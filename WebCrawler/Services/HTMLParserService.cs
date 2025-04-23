using HtmlAgilityPack;
using WebCrawler.Interfaces;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class HTMLParserService : IHTMLParserService
{
    private static readonly List<string> RequiredColumnNames = ["IP Address", "Port", "Country", "Protocol"];
    private static readonly string headerXPath = Environment.GetEnvironmentVariable("HEADER_XPATH") ??
        throw new Exception("HEADER_XPATH must be set in enviroment variables");
    private static readonly string rowsXPath = Environment.GetEnvironmentVariable("ROWS_XPATH") ??
        throw new Exception("ROWS_XPATH must be set in enviroment variables");

    public List<ProxyInfo> Parse(HtmlDocument doc)
    {
        var proxies = new List<ProxyInfo>();
        var headerIndexes = GetColumnIndexes(doc);
        if (headerIndexes.Count < RequiredColumnNames.Count)
        {
            Console.WriteLine("Nem todas as colunas necessárias foram encontradas. Abortando parsing.");
            return proxies;
        }

        var rows = doc.DocumentNode.SelectNodes(rowsXPath);
        if (rows == null) return proxies;

        int ipIndex = headerIndexes["IP Address"];
        int portIndex = headerIndexes["Port"];
        int countryIndex = headerIndexes["Country"];
        int protocolIndex = headerIndexes["Protocol"];

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count <= protocolIndex) continue;

            proxies.Add(new ProxyInfo
            {
                IPAddress = HtmlEntity.DeEntitize(cells[ipIndex].InnerText.Trim()),
                Port = HtmlEntity.DeEntitize(cells[portIndex].InnerText.Trim()),
                Country = HtmlEntity.DeEntitize(cells[countryIndex].InnerText.Trim()),
                Protocol = HtmlEntity.DeEntitize(cells[protocolIndex].InnerText.Trim())
            });
        }

        return proxies;
    }

    private static Dictionary<string, int> GetColumnIndexes(HtmlDocument doc)
    {
        var headerRow = doc.DocumentNode
            .SelectNodes(headerXPath)
            ?.FirstOrDefault();

        if (headerRow == null)
        {
            Console.WriteLine("Cabeçalho da tabela não encontrado!");
            return [];
        }

        var headerCells = headerRow.SelectNodes("th")?.ToList() ?? [];
        var columnIndexes = new Dictionary<string, int>();

        foreach (var columnName in RequiredColumnNames)
        {
            var index = headerCells.FindIndex(cell =>
                cell.InnerText.Contains(columnName, StringComparison.OrdinalIgnoreCase));

            if (index != -1)
            {
                columnIndexes[columnName] = index;
            }
            else
            {
                Console.WriteLine($"Coluna '{columnName}' não encontrada!");
            }
        }

        return columnIndexes;
    }
}
