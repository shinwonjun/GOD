// Enums.cs

namespace STATUS_UI
{
    public enum TAB
    {
        None = -1,
        Stats,
        Inventory,
        Character,
        Dex,
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
        stat,
        inventory,
        character,
        dex,
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

namespace GAME
{
    public enum HeroType
    {
        None = -1,
        GOD,
        DEMON,
    }

    public enum HeroSkilSlot
    {
        None = -1,
        Slot1_a = 10,
        Slot1_b = 11,
        Slot2 = 20,
        Slot31 = 31,        // GOD 전용스킬
        Slot32 = 32,        // DEMON 전용스킬
    }
    public enum ItemSkilSlot
    {
        None = -1,
        Slot1_a = 10,
        Slot1_b = 11,
        Slot2 = 20,
        Slot3 = 30,
    }
}