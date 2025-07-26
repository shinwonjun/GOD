using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class ServerTimeResponse
{
    public string serverTime;
}

[System.Serializable]
public class LastClaimTimeResponse
{
    public string lastClaimTime;
}

[System.Serializable]
public class GameServerResponse
{
    public UserIdWrapper userId;
    public Dictionary<string, int> statLevels;
    public List<int> ownedHeroIds;
    public List<int> equippedHeroIds;
    public Dictionary<string, int> equippedItems;
    public List<int> ownedItems;
    public BigInteger coin;
    public BigInteger diamond;
}

[System.Serializable]
public class UserIdWrapper
{
    public int userId;
}

[System.Serializable]
public class AddCoinResponse
{
    public bool success;
    public long newCoin;
}

[System.Serializable]
public class StatUpgradeResponse
{
    public bool success;
    public int newLevel;
    public long remainingCoin;
    public string message; // 실패 시에만 존재
    public string stat;    // 선택적으로 추가
}


public class GameMyData : Singleton<GameMyData>
{
    public int userId { get; set; } = -1;
    public Dictionary<STATUS_UI.Stat, int> dicStatLevel = new Dictionary<STATUS_UI.Stat, int>();
    public List<int> listOwnedHeros = new List<int>();  // 보유하고 있는 히어로
    public List<int> listEquipHeros = new List<int>();
    public Dictionary<ITEM.AttachPart, int> dicdicEquipItems = new Dictionary<ITEM.AttachPart, int>();
    public List<int> listOwnedItems = new List<int>();  // 보유하고 있는 아이템

    /// <summary>
    /// currency 재화
    /// </summary>
    private BigInteger coin;
    public BigInteger Coin
    {
        get => coin;
        set
        {
            coin = value;
            OnCoinChanged?.Invoke(coin); // 이벤트 발생
        }
    }
    public event Action<BigInteger> OnCoinChanged;


    private BigInteger diamond = 0;
    public BigInteger Diamond
    {
        get => diamond;
        set
        {
            diamond = value;
            OnDiamondChanged?.Invoke(diamond); // 이벤트 발생
        }
    }
    public event Action<BigInteger> OnDiamondChanged;


    ////////////////////////////////////////////////////////////

    public object LoadFromJson(string json, Type cType)
    {
        return JsonConvert.DeserializeObject(json, cType);
    }
    public void LoadGameInfoJson(string json)
    {
        var serverData = JsonConvert.DeserializeObject<GameServerResponse>(json);

        userId = serverData.userId.userId;

        // statLevels
        dicStatLevel.Clear();
        foreach (var kv in serverData.statLevels)
        {
            if (Enum.TryParse(kv.Key, out STATUS_UI.Stat stat))
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
            if (Enum.TryParse(kv.Key, out ITEM.AttachPart part))
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

        // Currency
        coin = serverData.coin;
        diamond = serverData.diamond;

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

    public DATA.ItemData getEquipedPartsID(ITEM.AttachPart parts)
    {
        if (dicdicEquipItems.ContainsKey(parts))
        {
            int id = dicdicEquipItems[parts];
            return DataManager.Instance.itemData[id];
        }
        return null;
    }
}
