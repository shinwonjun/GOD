// Enums.cs

namespace STATUS_UI
{
    public enum TAB
    {
        None = -1,
        Stats,
        Inventory,
        Character,
        Store
    }

    public enum Stat
    {
        Level,
        AttackPower,
        AttackSpeed,
        CriticalChance,
        CriticalDamage,
    }

    public enum Character
    {
        None = -1,
        Pitching,
        Armor,
        Shoes,
        Gloves,
        Necklace,
        RingL,
        RingR,
    }
}

namespace POPUP
{
    public enum POPUP
    {
        None = -1,
        item,
    }
}