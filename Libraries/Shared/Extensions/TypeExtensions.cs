namespace Shared.Extensions;

public static class TypeExtensions
{
    public static bool IsConcreteImplementationOf<T>(this Type type)
        => type is { IsAbstract: false, IsInterface: false } && typeof(T).IsAssignableFrom(type);

    public static bool HasHttpMethodAttribute(this Type type)
        => type.GetCustomAttribute<HttpMethodAttribute>() != null;

    public static HttpMethodAttribute GetHttpMethodAttributeOfDefault(this Type type)
        => type.GetCustomAttribute<HttpMethodAttribute>() ?? new HttpGetAttribute();

    public static TagsAttribute? GetTagsAttribute(this Type type)
        => type.GetCustomAttribute<TagsAttribute>();

    public static RouteAttribute? GetRouteAttribute(this Type type)
        => type.GetCustomAttribute<RouteAttribute>();

    public static bool HasApiRoute(this Type type)
        => type.GetRouteAttribute() != null || type.HasHttpMethodAttribute();

    public static Type? GetIRequestInterface(this Type type)
        => type.GetInterfaces()
        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

    public static bool ImplementsInterface<T>(this Type type)
        => typeof(T).IsAssignableFrom(type);

    public static Type MakeGenericTypes(this Type type, params Type?[] typeArguments)
    {
        List<Type> validTypeArguments = [];

        foreach (var t in typeArguments)
        {
            if (t != null)
            {
                validTypeArguments.Add(t);
            }
        }

        return type.MakeGenericType([.. validTypeArguments]);
    }
}
