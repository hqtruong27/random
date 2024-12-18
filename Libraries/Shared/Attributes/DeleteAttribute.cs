namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DeleteAttribute : HttpDeleteAttribute
{
    public DeleteAttribute()
    {

    }

    public DeleteAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}