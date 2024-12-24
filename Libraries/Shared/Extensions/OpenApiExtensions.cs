using Microsoft.OpenApi.Any;

namespace Shared.Extensions;

public static class OpenApiExtensions
{
    public static List<OpenApiTag> GenerateDefaultOpenApiTags(this string route)
    {
        var baseName = route.RemoveApiVersionPrefix().Split('/')[0] ?? route;
        return [new OpenApiTag { Name = baseName.ToTitleCase() }];
    }

    public static string[] DefaultTags(this string route, Assembly assembly)
    {
        var tag = route
            .RemoveApiVersionPrefix()
            .Split('/')[0];

        return [
            string.IsNullOrEmpty(tag)
            ? assembly.GetName().Name!
            : tag.Pascalize()
        ];
    }

    public static List<OpenApiParameter> GenerateParameters(Type routeType)
    {
        List<OpenApiParameter> openApiParameters = [];

        if (routeType.IsRecord())
        {
            // 1. Handle primary constructor parameters
            ConstructorInfo ctor = routeType.GetConstructors()[0];
            ParameterInfo[] parameters = ctor.GetParameters();

            foreach (var parameter in parameters)
            {
                var fromRoute = parameter.GetCustomAttribute<FromRouteAttribute>();
                bool isRequired = parameter.IsParameterRequired();

                openApiParameters.Add(new OpenApiParameter
                {
                    Name = parameter.Name?.ToLower(),
                    In = fromRoute != null ? ParameterLocation.Path : ParameterLocation.Query,
                    Required = isRequired,
                    Schema = GetOpenApiSchema(parameter.ParameterType)
                });
            }

            // 2. Handle properties defined in the record body
            var properties = routeType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !parameters.Any(x => x.Name == p.Name)); 
            // Exclude properties that are already handled as constructor parameters

            foreach (var property in properties)
            {
                var fromRoute = property.GetCustomAttribute<FromRouteAttribute>();
                bool isRequired = property.IsPropertyRequired();

                openApiParameters.Add(new OpenApiParameter
                {
                    Name = property.Name,
                    In = fromRoute != null ? ParameterLocation.Path : ParameterLocation.Query,
                    Required = isRequired,
                    Schema = new OpenApiSchema
                    {
                        Type = property.PropertyType.GetTypeCodeName()
                    }
                });
            }
        }
        else
        {
            // Handle regular classes
            var properties = routeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var fromRoute = property.GetCustomAttribute<FromRouteAttribute>();
                bool isRequired = property.IsPropertyRequired();

                openApiParameters.Add(new OpenApiParameter
                {
                    Name = property.Name,
                    In = fromRoute != null ? ParameterLocation.Path : ParameterLocation.Query,
                    Required = isRequired,
                    Schema = new OpenApiSchema
                    {
                        Type = property.PropertyType.GetTypeCodeName()
                    }
                });
            }
        }

        return openApiParameters;
    }

    private static OpenApiSchema GetOpenApiSchema(Type type)
    {
        var schema = new OpenApiSchema
        {
            Type = type.GetTypeCodeName()
        };

        // Add more details based on type
        if (type.IsEnum)
        {
            schema.Enum = [.. Enum.GetNames(type).Select(name => new OpenApiString(name)).Cast<IOpenApiAny>()];
        }
        else if (type.IsArray)
        {
            schema.Type = "array";
            schema.Items = GetOpenApiSchema(type.GetElementType()!);
        }
        else if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            // Handle Lists (if needed)
            schema.Type = "array";
            schema.Items = GetOpenApiSchema(type.GetGenericArguments()[0]);
        }
        else if (type == typeof(DateTime))
        {
            schema.Format = "date-time";
        }
        else if (type == typeof(Guid))
        {
            schema.Format = "uuid";
        }
        // Specific format handling for numeric types (as in the image)
        else if (Type.GetTypeCode(type) == TypeCode.Int64 || Type.GetTypeCode(type) == TypeCode.UInt64)
        {
            schema.Format = "int64";
        }
        else if (Type.GetTypeCode(type) == TypeCode.Single)
        {
            schema.Format = "float";
        }
        else if (Type.GetTypeCode(type) == TypeCode.Double || Type.GetTypeCode(type) == TypeCode.Decimal)
        {
            schema.Format = "double";
        }

        return schema;
    }
}
