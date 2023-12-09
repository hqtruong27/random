using System.Text.Json.Serialization;
using System.Text.Json;

namespace Common.Converters;

public class JsonTypeConverter<T> : JsonConverter<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                {
                    var stringValue = reader.GetString();
                    if (string.IsNullOrWhiteSpace(stringValue)) return default!;
                    if (typeToConvert.IsEnum)
                    {
                        return (T)System.Enum.Parse(typeToConvert, stringValue);
                    }

                    if (typeToConvert == typeof(long) || typeToConvert == typeof(int))
                    {
                        return ParseValue(stringValue);
                    }

                    return default;
                }
            case JsonTokenType.Number:
                return default;
            case JsonTokenType.Null:
                return default;
            default:
                throw new JsonException($"Unable to convert JSON value to {typeof(T)}.");
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var stringValue = value?.ToString();
        writer.WriteStringValue(stringValue);
    }

    private static T ParseValue(string value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }

    private static bool TryParseValue(string value, out T result)
    {
        try
        {
            result = (T)Convert.ChangeType(value, typeof(T));
            return true;
        }
        catch (Exception)
        {
            result = default!;
            return false;
        }
    }
}
