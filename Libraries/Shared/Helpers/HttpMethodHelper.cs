namespace Shared.Helpers;

public static class HttpMethodHelper
{
    private static readonly HashSet<string> SupportedHttpMethods =
    [
        HttpMethods.Get,
        HttpMethods.Head,
        HttpMethods.Delete,
        HttpMethods.Options
    ];

    public static bool IsHttpMethodSupported(string httpMethod) =>
        SupportedHttpMethods.Contains(httpMethod);
}
