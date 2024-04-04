using System.ComponentModel;

namespace StarRail.Core.Enums;

public enum GachaType
{
    [Description("Permanent Wish")]
    Regular = 1,
    [Description("Permanent Novice")]
    Novice = 2,
    [Description("Character Event Wish")]
    CharLimited = 11,
    [Description("Weapon Event Wish")]
    LightCone = 12
}