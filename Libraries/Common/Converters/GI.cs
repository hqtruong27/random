using Common.Enum.Hoyoverse;
namespace Common.Converters;

public static partial class GI
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
