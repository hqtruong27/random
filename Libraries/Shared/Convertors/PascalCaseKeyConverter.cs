namespace Shared.Convertors;

public class LowerCaseKeyConverter : JsonConverter<Dictionary<string, object?>>
{
    public override Dictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        Dictionary<string, object?> dictionary = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dictionary;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName token");

            // Convert key to PascalCase
            var propertyName = reader.GetString();
            var pascalCaseKey = ToLowercase(propertyName);
            if (pascalCaseKey == null) continue;

            reader.Read();

            object? value = JsonSerializer.Deserialize<object>(ref reader, options);
            dictionary[pascalCaseKey] = value;
        }

        throw new JsonException("Expected EndObject token");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, object?> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            string propertyName = kvp.Key;
            writer.WritePropertyName(propertyName);
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }

    private static string? ToLowercase(string? text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return 
            CultureInfo
            .InvariantCulture
            .TextInfo
            .ToLower(text);
    }
}
