namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class OptionsAttribute : HttpOptionsAttribute
{
    public OptionsAttribute()
    {

    }

    public OptionsAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}