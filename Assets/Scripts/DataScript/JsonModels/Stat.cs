using Newtonsoft.Json;

public class Stat
{
    [JsonProperty("levelUpgrade")]
    public int Level { get; set; }

    [JsonProperty("attackPowerUpgrade")]
    public int AttackPower { get; set; }

    [JsonProperty("attackSpeedUpgrade")]
    public float AttackSpeed { get; set; }

    [JsonProperty("criticalChanceUpgrade")]
    public float CriticalChance { get; set; }

    [JsonProperty("criticalDamageUpgrade")]
    public float CriticalDamage { get; set; }
}
