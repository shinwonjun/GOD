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
}

namespace POPUP
{
    public enum POPUP
    {
        None = -1,
        item,
    }
}