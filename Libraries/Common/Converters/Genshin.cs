using Common.Enum.Hoyoverse;
namespace Common.Converters;

public static class Genshin
{
    public static ItemType ItemTypeTranslation(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType.Equals(Constants.Genshin.NameTranslation.Weapons, StringComparison.CurrentCultureIgnoreCase) ? ItemType.Weapons : ItemType.Character,
            "en-us" => System.Enum.Parse<ItemType>(itemType),
            _ => System.Enum.Parse<ItemType>(itemType),
        };
    }
}
