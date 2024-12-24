using System.ComponentModel.DataAnnotations;

namespace Shared.Helpers;

public static class PropertyHelper
{
    public static bool IsPropertyRequired(this PropertyInfo propertyInfo)
    {
        // Check for [Required] attribute on the property itself
        if (propertyInfo.GetCustomAttribute<RequiredAttribute>() != null)
            return true;

        // Check for non-nullable reference types (C# 8.0+)
        if (!propertyInfo.PropertyType.IsValueType) // Check if it's a reference type
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullabilityInfo = nullabilityContext.Create(propertyInfo);
            if (nullabilityInfo.WriteState == NullabilityState.NotNull)
                return true;
        }
        // Explicitly handle nullable value types
        else if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null)
        {
            return false;
        }

        // Value types that are NOT nullable are required by default
        return propertyInfo.PropertyType.IsValueType;
    }

    public static bool IsParameterRequired(this ParameterInfo parameterInfo)
    {
        // Check for [Required] attribute on the parameter itself
        if (parameterInfo.GetCustomAttribute<RequiredAttribute>() != null) return true;

        // Handle record types
        if (parameterInfo.Member.DeclaringType != null && IsRecord(parameterInfo.Member.DeclaringType))
        {
            // For record types, check the corresponding property
            var propertyInfo = parameterInfo.Member.DeclaringType.GetProperty(parameterInfo.Name.Pascalize());
            if (propertyInfo != null && propertyInfo.GetCustomAttribute<RequiredAttribute>() != null)
            {
                return true;
            }
        }

        // Check for non-nullable reference types (C# 8.0+)
        if (!parameterInfo.ParameterType.IsValueType)
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullabilityInfo = nullabilityContext.Create(parameterInfo);
            if (nullabilityInfo.WriteState == NullabilityState.NotNull)
                return true;
        }
        // Explicitly handle nullable value types
        else if (Nullable.GetUnderlyingType(parameterInfo.ParameterType) != null)
        {
            // If it's a nullable value type (like long?), it's NOT required by default
            return false;
        }

        // Value types that are NOT nullable are required by default 
        //(e.g., long, int, bool, DateTime, Guid, etc.)
        return parameterInfo.ParameterType.IsValueType;
    }

    public static string? GetTypeCodeName(this Type? type)
    {
        if (type == null)
        {
            return null;
        }
        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type) ?? default!;
        }

        // Handle Enums
        if (type.IsEnum)
        {
            return "string"; // Base type for enums in OpenAPI is string
        }

        // Handle Arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            return elementType.GetTypeCodeName(); // Return element type for array schema
        }

        // Handle Lists (if needed, though arrays are sufficient for your case)
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            var elementType = type.GetGenericArguments()[0];
            return elementType.GetTypeCodeName(); // Return element type for list schema
        }

        // Standard type mapping with char as string
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean: return "boolean";
            case TypeCode.String: return "string";
            case TypeCode.Char: return "string"; // Treat char as string
            case TypeCode.DateTime: return "string";
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.SByte:
            case TypeCode.Byte: return "integer";
            case TypeCode.Int64:
            case TypeCode.UInt64: return "integer";
            case TypeCode.Single: return "number";
            case TypeCode.Double:
            case TypeCode.Decimal: return "number";
            default:
                if (type == typeof(Guid)) return "string";
                return "object"; // Fallback for complex types
        }
    }

    public static bool IsRecord(this Type type)
    {
        return type.GetMethod("<Clone>$") != null;
    }
}
