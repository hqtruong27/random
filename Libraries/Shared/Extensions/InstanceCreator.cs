using System.Collections;
using System.ComponentModel;
using Microsoft.Extensions.Primitives;
using Shared.Constants;

namespace Shared.Extensions;

public static class InstanceCreator
{
    public static async Task<object> CreateInstance(Type type, HttpContext context)
    {
        // Create an instance of the target type with default values
        var instance = CreateInstanceWithDefaults(type)
            ?? throw new InvalidOperationException(
                string.Format(
                    ExceptionMessage.UnableToCreateInstance,
                    type.Name
                    )
                );

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
                // Handle array-like query parameters here
                if (IsArrayLikeType(property.PropertyType))
                {
                    if (queryValue is StringValues stringValues)
                    {
                        value = HandleStringValues(stringValues, property.PropertyType);
                        SetPropertyValue(instance, property.Name, value);
                        continue;
                    }
                    if (queryValue.Count == 1 && queryValue[0].Contains(','))
                    {
                        // Comma-separated values
                        value = queryValue[0].Split(',');
                    }
                    else
                    {
                        // Multiple values with the same key
                        value = queryValue;
                    }
                }
                else
                {
                    value = queryValue;
                }
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
                SetPropertyValue(instance, property.Name, value);
            }
        }

        return await Task.FromResult(instance);
    }

    public static void SetPropertyValue(object instance, string propertyName, object value)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        ArgumentNullException.ThrowIfNull(instance);

        var property = instance.GetType().GetProperty(propertyName)
            ?? throw new ArgumentException(
                string.Format(
                    ExceptionMessage.PropertyNotFound,
                    propertyName,
                    instance.GetType().Name
                    )
               );

        if (!property.CanWrite) return;

        object convertedValue = ConvertValue(value, property.PropertyType);
        property.SetValue(instance, convertedValue);
    }

    private static bool IsArrayLikeType(Type type)
    {
        return type.IsArray ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) ||
               typeof(ICollection).IsAssignableFrom(type);
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

    private static object ConvertValue(object value, Type targetType)
    {
        // Handle StringValues first - it's a common case from web requests
        if (value is StringValues stringValues)
        {
            value = HandleStringValues(stringValues, targetType);
        }

        if (value == null)
        {
            // Handle null values, especially for nullable types
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                ? throw new ArgumentException(
                    $"Cannot set null to non-nullable type '{targetType.Name}'."
                    )
                : null!;
        }

        var valueType = value.GetType();

        // Direct assignment if types are the same or compatible
        if (targetType.IsAssignableFrom(valueType))
        {
            return value;
        }

        // Enum handling
        if (targetType.IsEnum)
        {
            return ConvertToEnum(value, targetType);
        }

        // Array handling
        if (targetType.IsArray)
        {
            return ConvertToArray(value, targetType);
        }

        // Nullable type handling
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            return ConvertValue(value, underlyingType);
        }

        // List handling (generic collections)
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            return ConvertToList(value, targetType);
        }

        // Handle ICollection (non-generic collections)
        if (typeof(ICollection).IsAssignableFrom(targetType))
        {
            return ConvertToCollection(value, targetType);
        }

        // Using TypeConverter
        return ConvertUsingTypeConverter(value, targetType);
    }

    private static object HandleStringValues(StringValues stringValues, Type targetType)
    {
        if (targetType == typeof(string) || targetType == typeof(object))
        {
            // Treat StringValues as a single string (joining multiple values with a comma)
            return stringValues.ToString();
        }
        else if (targetType.IsArray)
        {
            // Convert StringValues to string[] directly
            return stringValues.ToArray();
        }
        else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
        {
            // Convert StringValues to List<string>
            return stringValues.ToList();
        }
        else
        {
            // For other types, take the first value or throw an exception if empty
            return stringValues.FirstOrDefault() ?? default!;
        }
    }

    private static object ConvertToEnum(object value, Type enumType)
    {
        if (value is string stringValue)
        {
            if (long.TryParse(stringValue, out var longValue))
            {
                // Check if the parsed number is in the valid range for the enum
                var underlyingType = Enum.GetUnderlyingType(enumType);
                try
                {
                    var numericValue = Convert.ChangeType(longValue, underlyingType);
                    if (!Enum.IsDefined(enumType, numericValue))
                    {
                        throw new ArgumentOutOfRangeException(
                            string.Format(
                                ExceptionMessage.EnumNumericValueOutOfRange,
                                longValue,
                                enumType.Name
                                )
                            );
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw;
                }
            }

            try
            {
                return Enum.Parse(enumType, stringValue, ignoreCase: true);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(
                    string.Format(
                        ExceptionMessage.EnumInvalidStringValue,
                        stringValue,
                        enumType.Name
                        ),
                    ex
                    );
            }
        }
        else if (value is IConvertible)
        {
            try
            {
                var underlyingType = Enum.GetUnderlyingType(enumType);
                var numericValue = Convert.ChangeType(value, underlyingType);

                // Check if the numeric value is within the range of defined enum values
                if (!Enum.IsDefined(enumType, numericValue))
                {
                    throw new ArgumentOutOfRangeException(
                           string.Format(
                               ExceptionMessage.EnumNumericValueOutOfRange,
                               numericValue,
                               enumType.Name
                               )
                           );
                }

                return Enum.ToObject(enumType, numericValue);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw; // Re-throw the ArgumentOutOfRangeException
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        ExceptionMessage.EnumConversionFailed,
                        value,
                        enumType.Name
                        ),
                    ex
                    );
            }
        }

        throw new ArgumentException(
            string.Format(
                ExceptionMessage.EnumInvalidValueType,
                value,
                enumType.Name
                )
            );
    }

    private static Array ConvertToArray(object value, Type arrayType)
    {
        var elementType = arrayType.GetElementType()!;

        if (value is IEnumerable enumerable && value is not string) // string is IEnumerable, but we don't want to treat it like an array of chars
        {
            var items = enumerable.Cast<object>().Select(item => ConvertValue(item, elementType)).ToList();
            var array = Array.CreateInstance(elementType, items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                array.SetValue(items[i], i);
            }
            return array;
        }

        return CreateSingleElementArray(value, elementType);
    }

    private static object ConvertToList(object value, Type listType)
    {
        var elementType = listType.GetGenericArguments()[0];
        var list = (IList)Activator.CreateInstance(listType)!;

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                list.Add(ConvertValue(item, elementType));
            }
        }
        else
        {
            list.Add(ConvertValue(value, elementType));
        }

        return list;
    }

    private static object ConvertToCollection(object value, Type collectionType)
    {
        var collection = (ICollection)Activator.CreateInstance(collectionType)!;
        var addMethod = collectionType.GetMethod("Add") 
            ?? throw new InvalidOperationException(
                "Collection type must have an Add method."
                );

        var elementType = addMethod.GetParameters()[0].ParameterType;

        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable)
            {
                addMethod.Invoke(collection, [ConvertValue(item, elementType)]);
            }
        }
        else
        {
            addMethod.Invoke(collection, [ConvertValue(value, elementType)]);
        }

        return collection;
    }

    private static object ConvertUsingTypeConverter(object value, Type targetType)
    {
        var converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(value.GetType()))
        {
            return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value)!;
        }

        converter = TypeDescriptor.GetConverter(value.GetType());
        if (converter.CanConvertTo(targetType))
        {
            return converter.ConvertTo(null, CultureInfo.InvariantCulture, value, targetType)!;
        }

        try
        {
            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            throw new InvalidCastException(
                string.Format(
                    ExceptionMessage.CannotConvertValueToType,
                    value.GetType().Name,
                    targetType.Name
                    )
                );
        }
    }

    private static Array CreateSingleElementArray(object value, Type elementType)
    {
        var array = Array.CreateInstance(elementType, 1);
        array.SetValue(ConvertValue(value, elementType), 0);
        return array;
    }
}
