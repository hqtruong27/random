using System.Text.RegularExpressions;
using Humanizer;

namespace Shared.Helpers;

public static class StringHelper
{
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Camelize();
    }

    public static string RemoveApiVersionPrefix(this string path)
    {
        string pattern = @"^/api/v\d+/";
        return Regex.Replace(path, pattern, "/");
    }
}
