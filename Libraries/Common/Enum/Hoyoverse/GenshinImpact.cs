using System.ComponentModel;

namespace Common.Enum.Hoyoverse;

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

public enum RankType
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5
}

public enum ItemType
{
    Character = 1,
    Weapons = 2,
}