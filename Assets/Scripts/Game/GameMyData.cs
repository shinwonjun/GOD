using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class GameServerResponse
{
    public UserIdWrapper userId;
    public Dictionary<string, int> statLevels;
    public List<int> ownedHeroIds;
    public List<int> equippedHeroIds;
    public Dictionary<string, int> equippedItems;
    public List<int> ownedItems;
}

[System.Serializable]
public class UserIdWrapper
{
    public int userId;
}


public class GameMyData : Singleton<GameMyData>
{
    public int userId { get; set; } = -1;
    public Dictionary<STATUS_UI.Stat, int> dicStatLevel = new Dictionary<STATUS_UI.Stat, int>();
    public List<int> listOwnedHeros = new List<int>();  // 보유하고 있는 히어로
    public List<int> listEquipHeros = new List<int>();
    public Dictionary<ITEM.AttachPart, int> dicdicEquipItems = new Dictionary<ITEM.AttachPart, int>();
    public List<int> listOwnedItems = new List<int>();  // 보유하고 있는 아이템


    public void LoadFromJson(string json)
    {
        var serverData = JsonConvert.DeserializeObject<GameServerResponse>(json);

        userId = serverData.userId.userId;

        // statLevels
        dicStatLevel.Clear();
        foreach (var kv in serverData.statLevels)
        {
            if (System.Enum.TryParse(kv.Key, out STATUS_UI.Stat stat))
            {
                dicStatLevel[stat] = kv.Value;
            }
            else
            {
                Debug.LogWarning($"[GameMyData] Unknown Stat: {kv.Key}");
            }
        }

        // OwenedHeroIds
        listOwnedHeros = new List<int>(serverData.ownedHeroIds);
        
        // equippedHeroIds
        listEquipHeros = new List<int>(serverData.equippedHeroIds);

        // equippedItems
        dicdicEquipItems.Clear();
        foreach (var kv in serverData.equippedItems)
        {
            if (System.Enum.TryParse(kv.Key, out ITEM.AttachPart part))
            {
                dicdicEquipItems[part] = kv.Value;
            }
            else
            {
                Debug.LogWarning($"[GameMyData] Unknown AttachPart: {kv.Key}");
            }
        }

        // OwenedItems
        listOwnedItems = new List<int>(serverData.ownedItems);

        Debug.Log("[GameMyData] Loaded successfully.");
    }

    public Boolean hasItem(int id)
    {
        return listOwnedItems.Contains(id);
    }

    public Boolean checkEquipedItem(int id)
    {
        return dicdicEquipItems.Values.Contains(id);
    }

    public Boolean hasDex(int id)
    {
        return listOwnedHeros.Contains(id);
    }

    public Boolean checkEquipedDex(int id)
    {
        return listEquipHeros.Contains(id);
    }
}
