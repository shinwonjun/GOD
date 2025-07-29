using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 서버 응답
/// </summary>
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

[System.Serializable]
public class KillEnemyResponse
{
    public bool success;
    public float reward;
    public string message;
    public BigInteger totalCoin;
    public int EnemyId;
    public int Level;
}



public class GameMyData : Singleton<GameMyData>
{
    public FakeUserData UserData { get; set; }

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
        var serverData = JsonConvert.DeserializeObject<FakeUserData>(json);
        UserData = serverData;
        UserData.InitStatArrayFromClass();

        // Currency
        coin = UserData.coin;
        diamond = UserData.diamond;

        Debug.Log("[GameMyData] Loaded successfully.");
    }

    public Boolean hasItem(int id)
    {
        return UserData.ownedItems.Contains(id);
    }

    public Boolean checkEquipedItem(int id)
    {
        return UserData.equippedItems.Values.Contains(id);
    }

    public Boolean hasDex(int id)
    {
        return UserData.ownedHeroIds.Contains(id);
    }

    public Boolean checkEquipedDex(int id)
    {
        return UserData.equippedHeroIds.Contains(id);
    }

    public DATA.ItemData getEquipedPartsID(ITEM.AttachPart parts)
    {
        if (UserData.equippedItems.ContainsKey(parts.ToString()))
        {
            int id = UserData.equippedItems[parts.ToString()];
            return DataManager.Instance.itemData[id];
        }
        return null;
    }
}
