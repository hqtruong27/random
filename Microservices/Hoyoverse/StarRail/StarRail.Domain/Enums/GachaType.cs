using System.ComponentModel;

namespace StarRail.Domain.Enums;

public enum GachaType
{
    [Description("Permanent Wish")]
    Regular = 1,
    [Description("Character Event Wish")]
    CharLimited = 11,
    [Description("Weapon Event Wish")]
    LightCone = 12
}