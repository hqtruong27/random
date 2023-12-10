using System.Reflection;

namespace Common.Helpers;

public static class QueryStringHelper
{
    public static T Populate<T>(string queryString, T model)
    {
        var queryParameters = ParseQueryString(queryString);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var (key, values) in queryParameters)
        {
            var normalizedKey = key.ToLower();
            var property = properties.FirstOrDefault(p => p.Name.Equals(normalizedKey, StringComparison.CurrentCultureIgnoreCase));

            if (property != null)
            {
                var convertedValue = ConvertValue(values.FirstOrDefault(), property.PropertyType);
                property.SetValue(model, convertedValue);
            }
        }

        return model;
    }

    private static Dictionary<string, List<string>> ParseQueryString(string queryString)
    {
        var pairs = queryString.TrimStart('?').Split('&');
        var queryParameters = new Dictionary<string, List<string>>();

        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            if (parts.Length == 2)
            {
                var key = parts[0];
                var value = Uri.UnescapeDataString(parts[1]);

                if (!queryParameters.TryGetValue(key, out var _))
                {
                    queryParameters[key] = [];
                }

                queryParameters[key].Add(value);
            }
        }

        return queryParameters;
    }

    private static object ConvertValue(string? value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value)) return default!;

        return targetType.Name switch
        {
            nameof(Int32) => int.TryParse(value, out var intValue) ? intValue : 0,
            nameof(Boolean) => bool.TryParse(value, out var boolValue) && boolValue,
            // Add more type conversions as needed
            _ => value,
        };
    }
}