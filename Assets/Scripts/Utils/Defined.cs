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
        character,
    }
}

namespace ITEM
{
    public enum AttachPart
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

    public enum AttachMaterial
    {
        None = -1,
        cloth,      // 천
        leather,    // 가죽
        rock,       // 돌
        chain,      // 사슬
        steel,      // 철
    }
}