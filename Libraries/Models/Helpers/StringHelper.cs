namespace Models.Helpers;

public static class StringHelper
{
    public static (string Surname, string GivenName) ConvertName(this string fullName)
    {
        var arrListStr = fullName.Split([' ']);

        var surname = arrListStr?[0] ?? string.Empty;

        var givenName = arrListStr?.Length >= 1 ? string.Join(" ", arrListStr.Skip(1)) : string.Empty;

        return (surname, givenName);
    }
}
