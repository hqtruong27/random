namespace Shared.Convertors;

public class EnvToJsonConverter
{
    public enum NamingConvention
    {
        Default,
        PascalCase,
        SnakeCase,
        CamelCase,
        PascalSnakeCase
    }

    public static MemoryStream ConvertToJsonStream(Dictionary<string, string> variables, NamingConvention namingConvention = NamingConvention.PascalCase)
    {
        var jsonResult = TransformToNestedJson(variables, namingConvention);

        var bytes = JsonSerializer.SerializeToUtf8Bytes(
            jsonResult,
            options: new()
            {
                WriteIndented = true
            });

        return new(bytes);
    }

    public static string ConvertToJson(Dictionary<string, string> variables, NamingConvention namingConvention = NamingConvention.PascalCase)
    {
        var jsonResult = TransformToNestedJson(variables, namingConvention);

        return JsonSerializer.Serialize(jsonResult, options: new()
        {
            WriteIndented = true
        });
    }

    private static Dictionary<string, object> TransformToNestedJson(Dictionary<string, string> variables, NamingConvention namingConvention)
    {
        var result = new Dictionary<string, object>();

        foreach (var (key, value) in variables)
        {
            AddNestedKey(result, ApplyNamingConvention(key.Split("__"), namingConvention), value);
        }

        return result;
    }

    private static void AddNestedKey(Dictionary<string, object> dict, string[] keys, string value)
    {
        if (keys == null || keys.Length == 0)
            throw new ArgumentException("Keys cannot be null or empty.", nameof(keys));

        for (var i = 0; i < keys.Length - 1; i++)
        {
            var currentKey = keys[i];
            if (int.TryParse(keys[i + 1], out var index))
            {
                // Handle array creation or retrieval
                dict[currentKey] = dict.TryGetValue(currentKey, out var existingValue)
                && existingValue is List<object> existingList
                ? existingList
                : [];
                var list = (List<object>)dict[currentKey];

                // Ensure the list has enough capacity for the index
                while (list.Count <= index)
                    list.Add(null!);

                // Initialize the target index as a dictionary if null
                dict = (Dictionary<string, object>)list[index];
                i++; // Skip the next key since it's the array index
            }
            else
            {
                dict[currentKey] = dict.TryGetValue(currentKey, out var existingValue) 
                   && existingValue is Dictionary<string, object> existingDict
                   ? existingDict
                   : [];

                dict = (Dictionary<string, object>)dict[currentKey];
            }
        }

        // Base case: Add the final key with its value
        dict[keys[^1]] = value;
    }

    private static string[] ApplyNamingConvention(string[] keys, NamingConvention namingConvention)
    {
        for (var i = 0; i < keys.Length; i++)
        {
            keys[i] = namingConvention switch
            {
                NamingConvention.PascalCase => ToPascalCase(keys[i]),
                NamingConvention.CamelCase => ToCamelCase(keys[i]),
                NamingConvention.SnakeCase => ToSnakeCase(keys[i]),
                NamingConvention.PascalSnakeCase => ToPascalSnakeCase(keys[i]),
                _ => keys[i]
            };
        }

        return keys;
    }

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return
            CultureInfo
            .InvariantCulture
            .TextInfo
            .ToTitleCase(input.ToLower().Replace("_", " "))
            .Replace(" ", string.Empty);
    }

    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var pascalCase = ToPascalCase(input);
        return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return
            CultureInfo
            .InvariantCulture
            .TextInfo
            .ToLower(input);
    }


    private static string ToPascalSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return CultureInfo
            .InvariantCulture
            .TextInfo
            .ToTitleCase(input.ToLower().Replace("_", " "))
            .Replace(" ", "_");
    }

    public static Dictionary<string, string> ParseEnvFile(string filePath)
    {
        var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                keyValues[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return keyValues;
    }

    public static async Task<Dictionary<string, string>> ParseEnvFileAsync(string filePath)
    {
        var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in await File.ReadAllLinesAsync(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                keyValues[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return keyValues;
    }
}
