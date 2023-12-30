namespace System;
public static class TypeExtensions
{
    public static Type MakeGenericTypes(this Type type, params Type?[] typeArguments)
    {
        List<Type> validTypeArguments = [];

        foreach (var t in typeArguments)
        {
            if (t != null)
            {
                validTypeArguments.Add(t);
            }
        }

        return type.MakeGenericType([.. validTypeArguments]);
    }
}
