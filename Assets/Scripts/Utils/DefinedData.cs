using Newtonsoft.Json;

namespace GAME_DATA
{
    public class GameMyData : Singleton<GameMyData>
    {
    }
}

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
}