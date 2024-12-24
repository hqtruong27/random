using Microsoft.OpenApi.Any;

namespace Shared.Extensions;

public static class OpenApiComponentGenerator
{
    /// <summary>
    /// Generates OpenApiComponents containing schemas for the provided types.
    /// </summary>
    /// <param name="types">The types to generate schemas for.</param>
    /// <returns>An OpenApiComponents object containing the generated schemas.</returns>
    public static OpenApiComponents GenerateComponents(params Type[] types)
    {
        var components = new OpenApiComponents();
        var generatedSchemas = new Dictionary<string, OpenApiSchema>();

        foreach (var type in types)
        {
            GenerateSchemaForType(type, components, generatedSchemas);
        }

        return components;
    }

    private static void GenerateSchemaForType(Type type, OpenApiComponents components, Dictionary<string, OpenApiSchema> generatedSchemas)
    {
        if (generatedSchemas.ContainsKey(type.FullName!))
        {
            return; // Already generated
        }

        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>()
        };

        generatedSchemas[type.FullName!] = schema;
        components.Schemas[type.Name + "-get"] = schema; // Using Type Name as Schema Name, consider customization

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propertySchema = CreateSchemaForProperty(property, components, generatedSchemas);
            if (propertySchema != null)
            {
                schema.Properties[property.Name] = propertySchema;
            }
        }
    }

    private static OpenApiSchema CreateSchemaForProperty(PropertyInfo property, OpenApiComponents components, Dictionary<string, OpenApiSchema> generatedSchemas)
    {
        var propertyType = property.PropertyType;

        // Handle Nullable Types
        var isNullable = false;
        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        if (underlyingType != null)
        {
            isNullable = true;
            propertyType = underlyingType;
        }

        if (propertyType == typeof(string))
        {
            return new OpenApiSchema { Type = "string", Nullable = isNullable };
        }
        else if (propertyType == typeof(int) || propertyType == typeof(long))
        {
            return new OpenApiSchema { Type = "integer", Format = "int32", Nullable = isNullable }; // Or int64
        }
        else if (propertyType == typeof(double) || propertyType == typeof(float))
        {
            return new OpenApiSchema { Type = "number", Format = "double", Nullable = isNullable };
        }
        else if (propertyType == typeof(bool))
        {
            return new OpenApiSchema { Type = "boolean", Nullable = isNullable };
        }
        else if (propertyType == typeof(DateTime))
        {
            return new OpenApiSchema { Type = "string", Format = "date-time", Nullable = isNullable };
        }
        else if (propertyType.IsEnum)
        {
            var enumSchema = new OpenApiSchema { Type = "string", Enum = new List<IOpenApiAny>() };
            foreach (var enumValue in Enum.GetValues(propertyType))
            {
                enumSchema.Enum.Add(new OpenApiString(enumValue.ToString()));
            }
            return enumSchema;
        }
        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var genericType = propertyType.GetGenericArguments()[0];
            var itemsSchema = CreateSchemaForTypeReference(genericType, components, generatedSchemas);
            return new OpenApiSchema { Type = "array", Items = itemsSchema, Nullable = isNullable };
        }

        // Handle other complex types by creating references
        else if (propertyType.IsClass)
        {
            return CreateSchemaForTypeReference(propertyType, components, generatedSchemas);
        }

        return null!; // Unsupported type
    }

    private static OpenApiSchema CreateSchemaForTypeReference(Type type, OpenApiComponents components, Dictionary<string, OpenApiSchema> generatedSchemas)
    {
        GenerateSchemaForType(type, components, generatedSchemas);
        return new OpenApiSchema
        {
            Reference = new OpenApiReference
            {
                Id = $"#/components/schemas/{type.Name}"
            }
        };
    }
}