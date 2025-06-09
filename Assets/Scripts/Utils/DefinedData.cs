using Newtonsoft.Json;

namespace DATA
{
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
    }

    public class CharacterData
    {
        [JsonProperty("Part")]
        public string Part;

        [JsonProperty("Description")]
        public string Description;
    }
}