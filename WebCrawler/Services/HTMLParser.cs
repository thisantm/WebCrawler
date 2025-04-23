using HtmlAgilityPack;
using WebCrawler.Models;

namespace WebCrawler.Services;

public class HtmlParser
{
    public static List<ProxyInfo> Parse(string html)
    {
        var proxies = new List<ProxyInfo>();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var headerIndexes = GetColumnIndexes(doc);
        var rows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'table table-hover')]/tbody/tr");

        if (rows == null) return proxies;

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");
            if (cells == null || cells.Count < 4) continue;

            proxies.Add(new ProxyInfo
            {
                IPAddress = cells[headerIndexes["IP Address"]].InnerText.Trim(),
                Port = cells[headerIndexes["Port"]].InnerText.Trim(),
                Country = cells[headerIndexes["Country"]].InnerText.Trim(),
                Protocol = cells[headerIndexes["Protocol"]].InnerText.Trim()
            });
        }

        return proxies;
    }

    private static Dictionary<string, int> GetColumnIndexes(HtmlDocument doc)
    {
        var headerRows = doc.DocumentNode.SelectNodes("//table[contains(@class, 'table table-hover')]/thead/tr");
        var headerRow = headerRows?.FirstOrDefault();
        if (headerRow == null)
        {
            Console.WriteLine("Cabeçalho da tabela não encontrado!");
            return [];
        }

        var headerCells = headerRow.SelectNodes("th")!.ToList();
        var columnNames = new List<string> { "IP Address", "Port", "Country", "Protocol" };
        var columnIndexes = new Dictionary<string, int>();
        foreach (var columnName in columnNames)
        {
            var index = headerCells.FindIndex(cell => cell.InnerText.Contains(columnName, StringComparison.OrdinalIgnoreCase));
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
