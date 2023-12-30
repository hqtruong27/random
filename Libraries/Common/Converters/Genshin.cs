using Common.Enum.Hoyoverse;
namespace Common.Converters;

public static class Genshin
{
    public static ItemType ItemTypeTranslation(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType == Constants.Genshin.NameTranslation.Weapons ? ItemType.Weapons : ItemType.Character,
            _ => ItemType.Character,
        };
    }
}
