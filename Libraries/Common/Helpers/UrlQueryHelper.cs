using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Common.Helpers;

public static partial class UrlQueryHelper
{
    public static T Populate<T>(string queryString)
    {
        var queryParameters = ParseQueryString(queryString);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
     
        var model = Activator.CreateInstance<T>();
        foreach (var (key, values) in queryParameters)
        {
            var property = properties.FirstOrDefault(p =>
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attr != null)
                {
                    return attr.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase);
                }

                var snakeCase = ToSnakeCase(p.Name);
                return snakeCase.Equals(key, StringComparison.CurrentCultureIgnoreCase);
            });

            if (property != null)
            {
                var convertedValue = ConvertValue(values, property.PropertyType);
                property.SetValue(model, convertedValue);
            }
        }

        properties.Where(x => x.Name == "QueryString").FirstOrDefault()?.SetValue(model, queryString);

        return model;
    }

    private static Dictionary<string, string> ParseQueryString(string queryString)
    {
        var queryParameters = HttpUtility.ParseQueryString(new Uri(queryString).Query);

        var dictionary = new Dictionary<string, string>();

        foreach (string key in queryParameters)
        {
            dictionary.Add(key, value: queryParameters[key] ?? string.Empty);
        }

        return dictionary;
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

    private static string ToSnakeCase(string input)
    {
        return Regex.Replace(input, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "_$1").ToLower();
    }
}