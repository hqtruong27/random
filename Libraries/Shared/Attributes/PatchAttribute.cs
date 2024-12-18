namespace Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PatchAttribute : HttpPatchAttribute
{
    public PatchAttribute()
    {

    }

    public PatchAttribute([StringSyntax("Route")] string template) : base(template)
    {

    }
}
