namespace Shared.Attributes.OpenApi;

[AttributeUsage(AttributeTargets.Class)]
public class OpenApiSummaryAttribute(string summary) : Attribute
{
    public string Summary { get; set; } = summary;
}
