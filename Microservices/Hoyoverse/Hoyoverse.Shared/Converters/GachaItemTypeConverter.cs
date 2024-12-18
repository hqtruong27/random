//using Hoyoverse.Shared.Constants;

//namespace Hoyoverse.Shared.Converters;

//public class GachaItemTypeConverter
//{
//    public static ItemType Convert(string itemType, string lang)
//    {
//        return lang switch
//        {
//            "vi-vn" => itemType.Equals(
//                NameTranslator.Weapons,
//                StringComparison.CurrentCultureIgnoreCase) ? GenshinImpactItemType.Weapons : ItemType.Character,
//            "en-us" => Enum.Parse<ItemType>(itemType),
//            _ => Enum.Parse<ItemType>(itemType)
//        };
//    }
//}