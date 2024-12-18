namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PutAttribute : HttpPutAttribute
{
    public PutAttribute()
    {

    }

    public PutAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}