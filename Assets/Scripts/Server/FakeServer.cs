using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;
public class FakeUserData
{
    public UserId userId;
    public StatLevels statLevels;
    public List<int> ownedHeroIds;
    public List<int> equippedHeroIds;
    public Dictionary<string, int> equippedItems;
    public List<int> ownedItems;
    public BigInteger coin;
    public BigInteger diamond;

    public class UserId
    {
        public int userId;
    }

    public class StatLevels
    {
        public int Level;
        public int AttackPower;
        public int AttackSpeed;
        public int CriticalChance;
        public int CriticalDamage;
    }
}
public static class FakeServer
{
    // (requestType, responseData)
    public static Action<string, string> OnReceiveResponse;

    public static FakeUserData UserData { get; private set; }
    static FakeServer()
    {
        string response = @"{
            ""userId"": {
                ""userId"": 99999
            },
            ""statLevels"": {
                ""Level"": 1,
                ""AttackPower"": 1,
                ""AttackSpeed"": 1,
                ""CriticalChance"": 1,
                ""CriticalDamage"": 1
            },
            ""ownedHeroIds"":  [
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                // 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                // 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                // 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                // 41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                // 51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
                // 61
            ],
            ""equippedHeroIds"": [1, 4, 7],
            ""equippedItems"": {
                ""Pitching"": 101,
                ""Armor"": 102,
                ""Shoes"": 104,
                ""Gloves"": 103,
                ""Necklace"": 105,
                ""RingL"": 106,
                ""RingR"": 107
            },
            ""ownedItems"": [
                101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124,
                //201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224,
                //301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324,
                //401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424,
                //501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524
            ],
            ""coin"": 1590000,
            ""diamond"": 100
        }";

        UserData = JsonConvert.DeserializeObject<FakeUserData>(response);
    }
    public static void Request(string requestType, string payload)
    {
        Debug.Log($"[FakeServer] Received request: {requestType} / payload: {payload}");

        switch (requestType)
        {
            case "StartGame":
                StartGame(requestType, payload);
                break;
            case "GetServerTime":
                GetServerTime(requestType, payload);
                break;
            case "GetLastClaimTime":
                GetLastClaimTime(requestType, payload);
                break;
            /////////////////////////
            /// coin add
            ///////////////////////// 
            case "AddCoin":
                AddCoin(requestType, payload);
                break;
            /////////////////////////
            ///////////////////////// 
            /// 

            /////////////////////////
            /// stat upgrade
            ///////////////////////// 
            case "StatUpgrade":
                StatUpgrade(requestType, payload);
                break;
            // case "LevelUpgrade":
            //     LevelUpgrade(requestType, payload);
            //     break;
            // case "AttackPower":
            //     AttackPower(requestType, payload);
            //     break;
            // case "AttackSpeed":
            //     AttackSpeed(requestType, payload);
            //     break;
            // case "CriticalChance":
            //     CriticalChance(requestType, payload);
            //     break;
            // case "CriticalDamage":
            //     CriticalDamage(requestType, payload);
            //     break;
            /////////////////////////
            ///////////////////////// 
            default:
                Debug.LogWarning("[FakeServer] Unknown request type.");
                break;
        }
    }
    private static void StartGame(string responseType, string payload)
    {
        string response = JsonConvert.SerializeObject(UserData);
        OnReceiveResponse?.Invoke(responseType, response);
    }
    private static void GetServerTime(string responseType, string payload)
    {
        var serverTime = DateTime.UtcNow.ToString("o"); // ISO 8601 format
        var response = JsonConvert.SerializeObject(new { serverTime });
        OnReceiveResponse?.Invoke(responseType, response);
    }
    private static void GetLastClaimTime(string responseType, string payload)
    {
        var lastClaimTime = DateTime.UtcNow.AddMinutes(-10).ToString("o"); // 10분 전
        var response = JsonConvert.SerializeObject(new { lastClaimTime });
        OnReceiveResponse?.Invoke(responseType, response);
    }
    private static void AddCoin(string responseType, string payload)
    {
        if (BigInteger.TryParse(payload, out BigInteger coinToAdd))
        {
            UserData.coin += coinToAdd;

            // 응답 내려주기
            var response = JsonConvert.SerializeObject(new
            {
                success = true,
                newCoin = UserData.coin
            });

            OnReceiveResponse?.Invoke(responseType, response);
        }
        else
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "Invalid coin amount"
            });

            OnReceiveResponse?.Invoke(responseType, error);
        }
    }
    private static void StatUpgrade(string responseType, string payload)
    {
        STATUS_UI.Stat statType;
        if (payload == "LevelUpgrade")
            statType = STATUS_UI.Stat.Level;
        else if (payload == "AttackPower")
            statType = STATUS_UI.Stat.AttackPower;
        else if (payload == "AttackSpeed")
            statType = STATUS_UI.Stat.AttackSpeed;
        else if (payload == "CriticalChance")
            statType = STATUS_UI.Stat.CriticalChance;
        else if (payload == "CriticalDamage")
            statType = STATUS_UI.Stat.CriticalDamage;
        else
        {
            var error = JsonConvert.SerializeObject(new { success = false, message = "Invalid stat type" });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        int currentLevel = statType switch
        {
            STATUS_UI.Stat.Level => UserData.statLevels.Level,
            STATUS_UI.Stat.AttackPower => UserData.statLevels.AttackPower,
            STATUS_UI.Stat.AttackSpeed => UserData.statLevels.AttackSpeed,
            STATUS_UI.Stat.CriticalChance => UserData.statLevels.CriticalChance,
            STATUS_UI.Stat.CriticalDamage => UserData.statLevels.CriticalDamage,
            _ => 1
        };

        // 업그레이드 데이터 조회
        if (!DataManager.Instance.statUpgradeTable.TryGetValue(statType, out var upgradeData))
        {
            var error = JsonConvert.SerializeObject(new { success = false, message = "Stat upgrade data not found" });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 최대 레벨 체크
        if (currentLevel >= upgradeData.MaxLevel)
        {
            var fail = JsonConvert.SerializeObject(new { success = false, message = "Max level reached" });
            OnReceiveResponse?.Invoke(responseType, fail);
            return;
        }

        // 비용 계산
        int cost = Mathf.FloorToInt(upgradeData.GetCostAtLevel(currentLevel + 1));

        // 코인 충분한지 확인
        if (UserData.coin < cost)
        {
            var fail = JsonConvert.SerializeObject(new { success = false, message = "Not enough coin" });
            OnReceiveResponse?.Invoke(responseType, fail);
            return;
        }

        // 성공 처리: 레벨 증가
        switch (statType)
        {
            case STATUS_UI.Stat.Level: UserData.statLevels.Level = currentLevel + 1; break;
            case STATUS_UI.Stat.AttackPower: UserData.statLevels.AttackPower = currentLevel + 1; break;
            case STATUS_UI.Stat.AttackSpeed: UserData.statLevels.AttackSpeed = currentLevel + 1; break;
            case STATUS_UI.Stat.CriticalChance: UserData.statLevels.CriticalChance = currentLevel + 1; break;
            case STATUS_UI.Stat.CriticalDamage: UserData.statLevels.CriticalDamage = currentLevel + 1; break;
        }
        // 성공 처리: 코인 차감
        UserData.coin -= cost;

        var successResponse = JsonConvert.SerializeObject(new
        {
            success = true,
            newLevel = currentLevel + 1,
            remainingCoin = UserData.coin,
            stat = payload,
        });

        OnReceiveResponse?.Invoke(responseType, successResponse);
    }    
}
