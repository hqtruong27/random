namespace Shared.Extensions;

public static class InstanceCreator
{
    public static async Task<object> CreateFromHttpContext(Type type, HttpContext context)
    {
        var instance = CreateInstanceWithDefaults(type)
            ?? throw new InvalidOperationException(
                $"Unable to create an instance of type {type.Name}."
                );

        var payload = await context.ReadRequestBodyAsJsonAsync();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            object? value = null;

            if (context.Request.RouteValues.TryGetValue(property.Name, out var routeValue))
            {
                value = routeValue;
            }
            else if (context.Request.Query.TryGetValue(property.Name, out var queryValue))
            {
                value = queryValue;
            }
            else if (payload != null && payload.TryGetValue(property.Name.ToLowerInvariant(), out var bodyValue))
            {
                value = bodyValue;
            }
            else if (context.Request.Headers.TryGetValue(property.Name, out var headerValue))
            {
                value = headerValue;
            }

            if (value != null)
            {
                var convertedValue = Convert.ChangeType(value.ToString(), property.PropertyType);
                property.SetValue(instance, convertedValue);
            }
        }

        return Task.FromResult(instance);
    }

    public static async Task<object> CreateInstance(Type type, HttpContext context)
    {
        // Create an instance of the target type with default values
        var instance = CreateInstanceWithDefaults(type)
            ?? throw new InvalidOperationException($"Unable to create an instance of type {type.Name}.");

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        Dictionary<string, object?>? payload = null;
        // Enable buffering to allow reading the body multiple times
        if (context.Request.ContentLength > 0)
        {
            context.Request.EnableBuffering();
            using var streamReader = new StreamReader(context.Request.Body, leaveOpen: true);
            var bodyText = await streamReader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(bodyText))
            {
                payload = JsonSerializer.Deserialize<Dictionary<string, object?>>(
                    bodyText,
                    options: new()
                    {
                        Converters = { new LowerCaseKeyConverter() }
                    });
            }
        }

        foreach (var property in properties)
        {
            object? value = null;

            if (context.Request.RouteValues.TryGetValue(property.Name, out var routeValue))
            {
                value = routeValue;
            }
            else if (context.Request.Query.TryGetValue(property.Name, out var queryValue))
            {
                value = queryValue;
            }
            else if (payload != null && payload.TryGetValue(property.Name.ToLowerInvariant(), out var bodyValue))
            {
                value = bodyValue;
            }
            else if (context.Request.Headers.TryGetValue(property.Name, out var headerValue))
            {
                value = headerValue;
            }

            if (value != null)
            {
                var convertedValue = Convert.ChangeType(value.ToString(), property.PropertyType);
                property.SetValue(instance, convertedValue);
            }
        }

        return await Task.FromResult(instance);
    }

    private static async Task<Dictionary<string, object?>?> ReadRequestBodyAsJsonAsync(this HttpContext context)
    {
        if (context.Request is { ContentLength: <= 0 }) return null;

        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var bodyText = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        return string.IsNullOrWhiteSpace(bodyText)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(
                bodyText,
                options: new()
                {
                    Converters = { new LowerCaseKeyConverter() }
                });
    }

    private static object? CreateInstanceWithDefaults(Type type)
    {
        if (type.IsValueType) // Handle value types (structs)
        {
            return Activator.CreateInstance(type);
        }

        if (!type.IsClass)
        {
            throw new InvalidOperationException($"Unsupported type: {type.Name}");
        }

        if (HasExplicitParameterlessConstructor(type))
        {
            return Activator.CreateInstance(type); // Use parameterless constructor
        }

        // Find and invoke the constructor with default values if no parameterless constructor
        var constructor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault() ?? throw new InvalidOperationException(
                $"No constructors found for type {type.Name}"
                );

        var parameters = constructor.GetParameters()
            .Select(p => p.ParameterType.IsValueType
                ? Activator.CreateInstance(p.ParameterType)
                : null
                )
            .ToArray();

        return constructor.Invoke(parameters);
    }

    private static bool HasExplicitParameterlessConstructor(Type type)
    {
        // Get all public constructors of the type
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        // Check for an explicit parameterless constructor
        return constructors.Any(c => c.GetParameters().Length == 0);
    }
}
