namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HeadAttribute : HttpHeadAttribute
{
    public HeadAttribute()
    {

    }

    public HeadAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}
