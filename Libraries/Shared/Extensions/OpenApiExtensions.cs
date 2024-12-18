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
}
