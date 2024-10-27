using System.ComponentModel;

namespace GenshinImpact.Core.EnumTypes;

public enum GachaType
{
    [Description("Novice Wish")]
    Novice = 100,
    [Description("Permanent Wish")]
    Regular = 200,
    [Description("Character Event Wish")]
    CharLimited = 301,
    [Description("Weapon Event Wish")]
    Weapons = 302,
    [Description("Character Event Wish-2")]
    CharLimitedTwo = 400,
}