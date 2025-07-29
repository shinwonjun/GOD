

using UnityEngine;

public static class SimulationCalc
{
    public static float GetBaseAttack()
    {
        return DataManager.Instance.myCharacterTable.DefaultAttackPower;
    }
    public static float GetAttackPower()
    {
        var table = DataManager.Instance.myCharacterTable;
        var stat = GameMyData.Instance.UserData.statLevelsByIndex;

        int level = stat[(int)STATUS_UI.Stat.AttackPower];
        int charLevel = stat[(int)STATUS_UI.Stat.Level];

        // simulation.js: (기본공격력 + 스탯보너스) * log(캐릭터레벨+1) * 증가상수
        float upgradedBase = table.DefaultAttackPower + (level * 0.2f);
        float levelMultiplier = Mathf.Log(charLevel + 1);
        float final = upgradedBase * levelMultiplier * table.ConstantAttack;
        return final;
    }
    public static float GetAttackSpeed()
    {
        var table = DataManager.Instance.myCharacterTable;
        var stat = GameMyData.Instance.UserData.statLevelsByIndex;

        int charLevel = stat[(int)STATUS_UI.Stat.Level];
        int speedLevel = stat[(int)STATUS_UI.Stat.AttackSpeed];

        return table.DefaultAttackSpeed + (charLevel + speedLevel) / 10f;
    }
    public static float GetCriticalChance()
    {
        var table = DataManager.Instance.myCharacterTable;
        var stat = GameMyData.Instance.UserData.statLevelsByIndex;

        int charLevel = stat[(int)STATUS_UI.Stat.Level];
        int critChanceLevel = stat[(int)STATUS_UI.Stat.CriticalChance];

        return Mathf.Min(100f, table.DefaultCriticalChance + charLevel + critChanceLevel);
    }
    public static float GetCriticalDamage()
    {
        var table = DataManager.Instance.myCharacterTable;
        var stat = GameMyData.Instance.UserData.statLevelsByIndex;

        int charLevel = stat[(int)STATUS_UI.Stat.Level];
        int critDamageLevel = stat[(int)STATUS_UI.Stat.CriticalDamage];

        return table.DefaultCriticalDamage + (charLevel + critDamageLevel) * 0.5f;
    }
    public static float GetFinalAttackPower(out bool isCrit)
    {
        var table = DataManager.Instance.myCharacterTable;
        var stat = GameMyData.Instance.UserData.statLevelsByIndex;
        var enemy = GameMyData.Instance.UserData.enemy;

        float raw = GetAttackPower();
        float def = enemy.GetDefense();
        float reduced = ApplyDefense(raw, def);

        float critChance = GetCriticalChance();
        float critMultiplier = GetCriticalDamage();

        isCrit = UnityEngine.Random.value < (critChance / 100f);
        return isCrit ? reduced * critMultiplier : reduced;
    }
    public static float ApplyDefense(float attack, float defense)
    {
        return attack * (1f / (1f + defense / 100f));
    }
}
