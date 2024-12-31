using System.Text;

namespace Discord.Shared.Helpers;

public static class DiscordEmbedHelper
{
    public static string CreateTable<T>(
         IList<T> query,
         string[] headers,
         Func<T, string[]> mapToRow)
    {
        // 1. Validate inputs
        if (query == null || headers == null || mapToRow == null)
        {
            throw new("Query, headers, and mapToRow cannot be null.");
        }

        // 2. Map data to rows using the provided function
        List<List<string>> rows = [.. query.Select(item => mapToRow(item).ToList())];

        // 3. Create the table (using the internal helper method)
        return CreateTableInternal([.. headers], rows);
    }

    public static string CreateTable<T>(
        IList<T> query,
        Func<T, string[]> mapToRow)
    {
        // 1. Validate inputs
        if (query == null || mapToRow == null)
        {
            throw new("Query and mapToRow cannot be null.");
        }

        // 2. Get headers from property names
        var properties = typeof(T).GetProperties();
        var headers = properties.Select(p => p.Name).ToArray();

        // 3. Map data to rows
        var rows = query.Select(item => mapToRow(item).ToList()).ToList();

        // 4. Create the table
        return CreateTableInternal([.. headers], rows);
    }

    public static string CreateTable<T>(this IList<T> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var properties = typeof(T).GetProperties();
        var headers = properties.Select(p => p.Name).ToArray();

        return CreateTable(
            query,
            headers,
            item => [.. properties.Select(p => p.GetValue(item)?.ToString() ?? "")]
            );
    }

    public static string CreateTableInternal(List<string> headers, List<List<string>> rows)
    {
        if (headers == null || rows == null || headers.Count == 0)
        {
            return string.Empty; // Handle empty cases
        }

        // 1. Find maximum width of each column (including header)
        List<int> columnWidths = [];
        for (int i = 0; i < headers.Count; i++)
        {
            int maxWidth = headers[i].Length;
            foreach (var row in rows)
            {
                if (i < row.Count && row[i].Length > maxWidth)
                {
                    maxWidth = row[i].Length;
                }
            }
            columnWidths.Add(maxWidth);
        }

        // 2. Build the table string using StringBuilder
        StringBuilder tableBuilder = new();

        // 3. Add the header row
        tableBuilder.Append("```\n"); // Start code block for monospace font
        for (int i = 0; i < headers.Count; i++)
        {
            tableBuilder.Append(headers[i].PadRight(columnWidths[i]));
            if (i < headers.Count - 1)
            {
                tableBuilder.Append("  "); // Add spacing between columns
            }
        }
        tableBuilder.Append('\n');

        // 4. Add the separator row (using U+2500 (Box Drawings Light Horizontal)
        for (int i = 0; i < headers.Count; i++)
        {
            tableBuilder.Append(new string('\u2500', columnWidths[i]));
            if (i < headers.Count - 1)
            {
                tableBuilder.Append("\u2500\u2500"); // Add spacing
            }
        }
        tableBuilder.Append('\n');

        // 5. Add the data rows
        foreach (var row in rows)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                string cellValue = (i < row.Count) ? row[i] : ""; // Handle missing cells
                tableBuilder.Append(cellValue.PadRight(columnWidths[i]));
                if (i < headers.Count - 1)
                {
                    tableBuilder.Append("  "); // Add spacing
                }
            }
            tableBuilder.Append('\n');
        }

        tableBuilder.Append("```"); // End code block

        return tableBuilder.ToString();
    }
}
