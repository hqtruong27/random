using GenshinImpact.Core.EnumTypes;
using GenshinImpact.Shared.Constants;

namespace GenshinImpact.Shared.Converters;

public class GachaItemTypeConverter
{
    public static ItemType Convert(string itemType, string lang)
    {
        return lang switch
        {
            "vi-vn" => itemType.Equals(
                NameTranslator.Weapons,
                StringComparison.CurrentCultureIgnoreCase) ? ItemType.Weapons : ItemType.Character,
            "en-us" => Enum.Parse<ItemType>(itemType),
            _ => Enum.Parse<ItemType>(itemType)
        };
    }
}