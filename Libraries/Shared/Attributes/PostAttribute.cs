namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PostAttribute : HttpPostAttribute
{
    public PostAttribute()
    {

    }

    public PostAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}
