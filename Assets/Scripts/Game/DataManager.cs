using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public List<DATA.StatData> statData = new List<DATA.StatData>();
    public Dictionary<string, List<DATA.ItemData>> itemData = new Dictionary<string, List<DATA.ItemData>>();
    public Dictionary<string, DATA.EquipslotData> characterData = new Dictionary<string, DATA.EquipslotData>();
    
    public List<DATA.HeroList> heroList = new List<DATA.HeroList>();
    public Dictionary<string, List<DATA.HeroData>> heroData = new Dictionary<string, List<DATA.HeroData>>();



    public void InitData()
    {
        LoadStatData();
        LoadItemData();
        LoadCharacterData();
        LoadHeroList();
        LoadHeroData();
    }
    private void LoadStatData()
    {
        statData = JsonLoader.LoadFromResources<List<DATA.StatData>>("data/stat_data");
        if (statData == null)
        {
            Debug.LogError("DataManager: stat_data.json 로드 실패!");
            return;
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
        List<DATA.EquipslotData> datas = new List<DATA.EquipslotData>();
        datas = JsonLoader.LoadFromResources<List<DATA.EquipslotData>>("data/equipslot_data");
        if (datas == null)
        {
            Debug.LogError("DataManager: scharacter_data.json 로드 실패!");
            return;
        }

        foreach (DATA.EquipslotData item in datas)
        {
            string key = item.Part;
            characterData[key] = item;
        }
    }

    private void LoadHeroList()
    {
        heroList = JsonLoader.LoadFromResources<List<DATA.HeroList>>("data/herolist_data");
        if (heroList == null)
        {
            Debug.LogError("DataManager: hero_list.json 로드 실패!");
            return;
        }

        // 디버그용 로그: 각 재질(Material)별 아이템 개수 확인
        foreach (var kvp in heroList)
        {
            Debug.LogFormat("HeroName = {0}, Type = {1}", kvp.Name, kvp.Type);
        }
    }

    private void LoadHeroData()
    {
        List<DATA.HeroData> allHeros = JsonLoader.LoadFromResources<List<DATA.HeroData>>("data/hero_data");
        if (allHeros == null)
        {
            Debug.LogError("DataManager: hero_data.json 로드 실패!");
            return;
        }

        // 2) heroData 딕셔너리 초기화 및 그룹화
        heroData = new Dictionary<string, List<DATA.HeroData>>();
        foreach (DATA.HeroData item in allHeros)
        {
            string key = item.Type;
            if (heroData.ContainsKey(key))
            {
                heroData[key].Add(item);
            }
            else
            {
                heroData[key] = new List<DATA.HeroData> { item };
            }
        }

        // 디버그용 로그: 각 재질(Material)별 아이템 개수 확인
        foreach (var kvp in heroData)
        {
            Debug.LogFormat("DataManager: Material = {0}, Count = {1}", kvp.Key, kvp.Value.Count);
        }
    }
}
