namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GetAttribute : HttpGetAttribute
{
    public GetAttribute()
    {

    }

    public GetAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}