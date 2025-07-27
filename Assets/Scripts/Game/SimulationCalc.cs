

using UnityEngine;

public static class SimulationCalc
{
    public static float GetAttackPower()
    {
        var table = DataManager.Instance.myCharacterTable;
        int level = GameMyData.Instance.UserData.statLevelsByIndex[(int)STATUS_UI.Stat.AttackPower];
        return table.DefaultAttackPower * Mathf.Log(level + 1) * table.ConstantAttack;
    }

    public static float GetAttackSpeed()
    {
        var table = DataManager.Instance.myCharacterTable;
        int level = GameMyData.Instance.UserData.statLevelsByIndex[(int)STATUS_UI.Stat.AttackSpeed];
        return table.DefaultAttackSpeed + (level / 10f);
    }

    public static float GetCriticalChance()
    {
        var table = DataManager.Instance.myCharacterTable;
        int level = GameMyData.Instance.UserData.statLevelsByIndex[(int)STATUS_UI.Stat.CriticalChance];
        return Mathf.Min(100f, table.DefaultCriticalChance + level * 1f);
    }

    public static float GetCriticalDamage()
    {
        var table = DataManager.Instance.myCharacterTable;
        int level = GameMyData.Instance.UserData.statLevelsByIndex[(int)STATUS_UI.Stat.CriticalDamage];
        return table.DefaultCriticalDamage + level * 0.5f;
    }

    public static float ApplyDefense(float attack, float defense)
    {
        return attack * (1f / (1f + defense / 100f));
    }

    public static float GetFinalAttackPower(out bool isCrit)
    {
        var enemy = GameMyData.Instance.UserData.enemy;
        var table = DataManager.Instance.myCharacterTable;
        var statLevels = GameMyData.Instance.UserData.statLevelsByIndex;

        int levelAtk = statLevels[(int)STATUS_UI.Stat.AttackPower];
        int levelCritChance = statLevels[(int)STATUS_UI.Stat.CriticalChance];
        int levelCritDamage = statLevels[(int)STATUS_UI.Stat.CriticalDamage];

        // 1. 기본 공격력 계산 (로그 성장)
        float rawAtk = table.DefaultAttackPower * Mathf.Log(levelAtk + 1) * table.ConstantAttack;

        // 2. 방어력 적용 (simulation.js 기반)
        float enemyDef = enemy.GetDefense(); // EnemyTable 기반
        float atkAfterDefense = rawAtk * (1f / (1f + enemyDef / 100f));

        // 3. 크리티컬 확률 / 배율
        float critChance = Mathf.Min(100f, table.DefaultCriticalChance + levelCritChance);
        float critMultiplier = table.DefaultCriticalDamage + levelCritDamage * 0.5f;

        // 4. 크리 여부 판단 및 적용
        isCrit = UnityEngine.Random.value < (critChance / 100f);
        float finalDamage = isCrit ? atkAfterDefense * critMultiplier : atkAfterDefense;

        Debug.Log($"[Simulation] raw {rawAtk:F1} → 방어 후 {atkAfterDefense:F1} → 최종 {finalDamage:F1} {(isCrit ? "CRIT!" : "")}");

        return finalDamage;
    }
}
