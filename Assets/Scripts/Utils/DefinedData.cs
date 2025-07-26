using Newtonsoft.Json;
using UnityEngine;

namespace DATA
{
    public class HeroList
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Type")]
        public string Type;
    }

    public class HeroData
    {
        [JsonProperty("Id")]
        public string Id;

        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Type")]
        public string Type;

        [JsonProperty("Options")]
        public string[] Options;

        [JsonProperty("Description")]
        public string Description;

        [JsonProperty("Sprite")]
        public string Sprite;
    }

    public class ItemData
    {
        [JsonProperty("Id")]
        public string Id;

        [JsonProperty("Material")]
        public string Material;

        [JsonProperty("Part")]
        public string Part;

        [JsonProperty("Description")]
        public string Description;

        [JsonProperty("Sprite")]
        public string Sprite;
    }

    public class StatData
    {
        [JsonProperty("Name")]
        public string Name;

        [JsonProperty("Description")]
        public string Description;

        [JsonProperty("Sprite")]
        public string Sprite;
    }

    public class EquipslotData
    {
        [JsonProperty("Part")]
        public string Part;

        [JsonProperty("Description")]
        public string Description;
    }

    ////////////////////////////////////////////////////////////////////////

    public class StatUpgradeTable
    {
        [JsonProperty("levelUpgrade")]
        public StatUpgradeData LevelUpgrade { get; set; }

        [JsonProperty("attackUpgrade")]
        public StatUpgradeData AttackUpgrade { get; set; }

        [JsonProperty("speedUpgrade")]
        public StatUpgradeData SpeedUpgrade { get; set; }

        [JsonProperty("critChanceUpgrade")]
        public StatUpgradeData CritChanceUpgrade { get; set; }

        [JsonProperty("critDamageUpgrade")]
        public StatUpgradeData CritDamageUpgrade { get; set; }
    }

    public class StatUpgradeData
    {
        [JsonProperty("baseCost")]
        public float BaseCost { get; set; }

        [JsonProperty("costConstant")]
        public float CostConstant { get; set; }

        [JsonProperty("maxLevel")]
        public int MaxLevel { get; set; }

        // ✅ 업그레이드 비용 계산 함수
        public float GetCostAtLevel(int level)
        {
            if (level < 1) level = 1;
            if (level > MaxLevel) level = MaxLevel;

            return BaseCost * Mathf.Pow(CostConstant, level - 1);
        }
    }
}