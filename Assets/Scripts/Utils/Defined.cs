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
        Pitching,
        Armor,
        Shoes,
        Gloves,
        Necklace,
        ringL,
        ringR,
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