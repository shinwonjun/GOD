using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, string> statData { get; private set; }
    public Dictionary<string, List<DATA.ItemData>> itemData = new Dictionary<string, List<DATA.ItemData>>();
    public Dictionary<string, DATA.CharacterData> characterData = new Dictionary<string, DATA.CharacterData>();



    public void InitData()
    {
        LoadStatData();
        LoadItemData();
        LoadCharacterData();
    }
    private void LoadStatData()
    {
        statData = JsonLoader.LoadFromResources<Dictionary<string, string>>("data/stat_data");
        if (statData == null)
        {
            Debug.LogError("DataManager: stat_data.json 로드 실패!");
            statData = new Dictionary<string, string>()
            {
                { "Level", "The current level of the character." },
                { "AttackPower", "The attack power value of the character." },
                { "AttackSpeed", "The attack speed multiplier of the character." },
                { "CriticalChance", "The chance of landing a critical hit." },
                { "CriticalDamage", "The damage multiplier applied on a critical hit." }
            };
        }
    }

    private void LoadItemData()
    {
        // 1) item_data.json을 List<ItemData> 형태로 파싱
        List<DATA.ItemData> allItems = JsonLoader.LoadFromResources<List<DATA.ItemData>>("data/item_data");
        if (allItems == null)
        {
            Debug.LogError("DataManager: item_data.json 로드 실패!");
            return;
        }

        // 2) itemData 딕셔너리 초기화 및 그룹화
        itemData = new Dictionary<string, List<DATA.ItemData>>();
        foreach (DATA.ItemData item in allItems)
        {
            string key = item.Material;
            if (itemData.ContainsKey(key))
            {
                itemData[key].Add(item);
            }
            else
            {
                itemData[key] = new List<DATA.ItemData> { item };
            }
        }

        // 디버그용 로그: 각 재질(Material)별 아이템 개수 확인
        foreach (var kvp in itemData)
        {
            Debug.LogFormat("DataManager: Material = {0}, Count = {1}", kvp.Key, kvp.Value.Count);
        }
    }

    private void LoadCharacterData()
    {
        List<DATA.CharacterData> datas = new List<DATA.CharacterData>();
        datas = JsonLoader.LoadFromResources<List<DATA.CharacterData>>("data/character_data");
        if (datas == null)
        {
            Debug.LogError("DataManager: scharacter_data.json 로드 실패!");
            return;
        }

        foreach (DATA.CharacterData item in datas)
        {
            string key = item.Part;
            characterData[key] = item;
        }
    }
}
