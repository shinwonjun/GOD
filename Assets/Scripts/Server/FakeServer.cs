using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
public class FakeUserData
{
    public UserId userId;
    public Ability ability;
    public StatLevels statLevels;
    [JsonIgnore] public int[] statLevelsByIndex;
    public List<int> ownedHeroIds;
    public Dictionary<string, int> equippedHeroIds;
    public Dictionary<string, int> equippedItems;
    public List<int> ownedItems;
    public BigInteger coin;
    public BigInteger diamond;
    public FakeEnemyData enemy;
    public List<FakeHeroOptions> heroOptions;

    public void InitStatArrayFromClass()
    {
        statLevelsByIndex = new int[Enum.GetValues(typeof(STATUS_UI.Stat)).Length];
        statLevelsByIndex[(int)STATUS_UI.Stat.Level] = statLevels.Level;
        statLevelsByIndex[(int)STATUS_UI.Stat.AttackPower] = statLevels.AttackPower;
        statLevelsByIndex[(int)STATUS_UI.Stat.AttackSpeed] = statLevels.AttackSpeed;
        statLevelsByIndex[(int)STATUS_UI.Stat.CriticalChance] = statLevels.CriticalChance;
        statLevelsByIndex[(int)STATUS_UI.Stat.CriticalDamage] = statLevels.CriticalDamage;
    }

    public class UserId
    {
        public int userId;
    }

    public class Ability
    {
        public int AttackPower;
        public int AttackSpeed;
        public float CostConstant;
        public int CriticalChance;
        public int CriticalDamage;
    }
    public class StatLevels
    {
        public int Level;
        public int AttackPower;
        public int AttackSpeed;
        public int CriticalChance;
        public int CriticalDamage;
    }
    public class FakeEnemyData
    {
        public int EnemyId;
        public int Level;

        /// <summary>
        /// 현재 레벨에 따른 체력 계산
        /// </summary>
        public float GetHP()
        {
            var table = DataManager.Instance.enemyTable;
            if (table == null || Level < 1) return 0;

            return table.DefaultHP * Mathf.Pow(table.RateHP, Level - 1);
        }

        /// <summary>
        /// 현재 레벨에 따른 방어력 계산
        /// </summary>
        public float GetDefense()
        {
            var table = DataManager.Instance.enemyTable;
            if (table == null || Level < 1) return 0;

            return table.DefaultDef * Mathf.Pow(table.NaturalConstant, table.ConstantDef * (Level - 1));
        }

        /// <summary>
        /// 누적 처치 수에 따른 보상 재화 계산
        /// </summary>
        public float GetReward()
        {
            var table = DataManager.Instance.enemyTable;
            if (table == null || Level < 1) return 0;

            return table.DefaultReward * Mathf.Pow(table.ConstantReward, Level - 1);
        }
    }
}

public class FakeHeroOptions
{
    public int Id; // 히어로 id
    public Dictionary<int, Dictionary<int, List<string>>> options = new(); // type , id , option
}

public static class FakeServer
{
    // (requestType, responseData)
    public static Action<string, string> OnReceiveResponse;

    // DB에 저장 해야 될 값을(임시 데이터로 사용)
    public static FakeUserData UserData { get; private set; }

    static FakeServer()
    {
        string response = @"{
            ""userId"": {
                ""userId"": 99999
            },
            ""ability"": {
                ""AttackPower"": 10,
                ""AttackSpeed"": 1,
                ""CostConstant"": 0.02,
                ""CriticalChance"": 0,
                ""CriticalDamage"": 10,
            },
            ""statLevels"": {
                ""Level"": 1,
                ""AttackPower"": 0,
                ""AttackSpeed"": 0,
                ""CriticalChance"": 0,
                ""CriticalDamage"": 0
            },
            ""ownedHeroIds"":  [
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
                51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
                61
            ],
            ""equippedHeroIds"": {             
                ""1"": 1,
                ""2"": -1,
                ""3"": -1
            },
            ""equippedItems"": {
                ""Pitching"": 101,
                ""Armor"": 105,
                ""Shoes"": 113,
                ""Gloves"": 109,
                ""Necklace"": 117,
                ""RingL"": 121,
                ""RingR"": 122
            },
            ""ownedItems"": [
                101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124,
                201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224,
                301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324,
                401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424,
                501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524
            ],
            ""coin"": 1590000,
            ""diamond"": 100,
            ""enemy"": {
                ""EnemyId"": 39,        // 39 ~ 61번이 데몬 진영 캐릭터임 악당으로 쓰자 - 나중에 데몬 진영에서 랜덤하게 뽑도록 적용
                ""Level"": 1,           // 테스트
            },
            ""heroOptions"": [
                {""Id"":1,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":2,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":3,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":4,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":5,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":6,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":7,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":8,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":9,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":10,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":11,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":12,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":13,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":14,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":15,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":16,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":17,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":18,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":19,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":20,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":21,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":22,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":23,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":24,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":25,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":26,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":27,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":28,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":29,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":30,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":31,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":32,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":33,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":34,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":35,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":36,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":37,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":38,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":39,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":40,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":41,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":42,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":43,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":44,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":45,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":46,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":47,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":48,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":49,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":50,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":51,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":52,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":53,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":54,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":55,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":56,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":57,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":58,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":59,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":60,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}},
                {""Id"":61,""options"":{""10"":{""1000"":[""-1"",""-1""],""1999"":[""-1"",""-1""]},""20"":{""2000"":[""-1"",""-1""]},""30"":{""3000"":[""-1"",""-1""]}}}
            ]
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



            /////////////////////////
            /// stat upgrade
            ///////////////////////// 
            case "UpgradeStat":
                UpgradeStat(requestType, payload);
                break;
            /////////////////////////
            ///////////////////////// 



            /////////////////////////
            /// simulation
            ///////////////////////// 
            case "KillEnemy":
                KillEnemy(requestType, payload);
                break;
            /////////////////////////
            ///////////////////////// 



            /////////////////////////
            /// equip (item, hero)
            ///////////////////////// 
            case "EquipItem":
                EquipItem(requestType, payload);
                break;
            case "UnEquipItem":
                UnEquipItem(requestType, payload);
                break;
            case "EquipHero":
                EquipHero(requestType, payload);
                break;
            case "UnEquipHero":
                UnEquipHero(requestType, payload);
                break;
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
            // [DB갱신]
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

    private static void UpgradeStat(string responseType, string payload)
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

        // [DB갱신]

        var successResponse = JsonConvert.SerializeObject(new
        {
            success = true,
            newLevel = currentLevel + 1,
            remainingCoin = UserData.coin,
            stat = payload,
        });

        OnReceiveResponse?.Invoke(responseType, successResponse);
    }

    private static void KillEnemy(string responseType, string payload)
    {
        var jObj = JObject.Parse(payload);
        float enemyHP = jObj["enemyHP"]?.Value<float>() ?? -1f;
        float lastHit = jObj["lastHit"]?.Value<float>() ?? -1f;


        var serverEnemy = UserData.enemy;

        // 서버 기준 체력 계산
        float serverEnemyHp = serverEnemy.GetHP();

        // 1. 체력 검증: 클라가 0이라고 보낸 게 실제 서버에서 0일 수 있는지
        if (enemyHP > serverEnemyHp)
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "클라이언트가 보내온 적 체력이 서버 기준보다 높음 (조작 의심)"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 2. 마지막 데미지가 내 캐릭터 공격력보다 너무 크지 않은지 검증
        float maxServerAttack = GetFinalAttackPower_UserControlled(UserData, serverEnemy, withCritical: true); // 서버 입장에서 계산
        if (lastHit > maxServerAttack * 1.5f) // 1.5배 이상은 조작 의심
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "마지막 공격 데미지가 비정상적으로 큼 (조작 의심)"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 모든 검증 통과 → 보상 지급
        float reward = serverEnemy.GetReward();
        UserData.coin += (BigInteger)reward;

        selectEnemy();  // 새로운 적 선택 + 적 레벨 업

        // [DB갱신]

        var response = JsonConvert.SerializeObject(new
        {
            success = true,
            reward = reward,
            totalCoin = UserData.coin,
            EnemyId = UserData.enemy.EnemyId,
            Level = UserData.enemy.Level
        });

        OnReceiveResponse?.Invoke(responseType, response);
    }
    public static void selectEnemy()
    {
        var demonList = DataManager.Instance.heroDataByHeroType[GAME.HeroType.DEMON.ToString()];
        if (demonList == null || demonList.Count == 0)
        {
            Debug.LogWarning("[FakeServer] DEMON 타입 적이 존재하지 않습니다.");
            return;
        }

        // 랜덤하게 하나 선택
        int index = UnityEngine.Random.Range(0, demonList.Count);
        var selected = demonList[index];

        int beforeLevel = UserData.enemy.Level;
        int afterLevel = beforeLevel + 1;

        // UserData에 설정
        UserData.enemy = new FakeUserData.FakeEnemyData
        {
            EnemyId = int.Parse(selected.Id),
            Level = afterLevel
        };

        Debug.Log($"[FakeServer] DEMON 적 선택 완료 → ID: {selected.Id}, Name: {selected.Name}");
    }
    public static float GetFinalAttackPower_UserControlled(FakeUserData user, FakeUserData.FakeEnemyData enemy, bool withCritical = true)
    {
        // 서버에서 검증용, 크리티컬 항상 적용, 적 방어력 계산X 
        var table = DataManager.Instance.myCharacterTable;
        var statLevels = new int[]
        {
                user.statLevels.Level,
                user.statLevels.AttackPower,
                user.statLevels.AttackSpeed,
                user.statLevels.CriticalChance,
                user.statLevels.CriticalDamage
        };

        float baseAtk = table.DefaultAttackPower * Mathf.Log(statLevels[(int)STATUS_UI.Stat.AttackPower] + 1) * table.ConstantAttack;

        if (!withCritical)
            return baseAtk;

        float critMultiplier = table.DefaultCriticalDamage + statLevels[(int)STATUS_UI.Stat.CriticalDamage] * 0.5f;
        return baseAtk * critMultiplier;
    }


    private static void EquipItem(string responseType, string payload)
    {
        var jObj = JObject.Parse(payload);
        int itemId = jObj["itemId"]?.Value<int>() ?? -1;
        if (itemId == -1)
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "(-1) 잘못된 아이템"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 유효한 아이템인지 체크
        // 보유한 아이템인지 체크
        // 이미 착용중인 아이템인지 체크(이미 착용중인데 또 착용예외처리)
        int successType = -1;
        string _message = "";
        string parts = "";
        int equipId = -1;
        int prevId = -1;

        if (DataManager.Instance.itemData.TryGetValue(itemId, out DATA.ItemData data))
        {
            if (!UserData.ownedItems.Contains(itemId))
            {
                var error = JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "보유하지 않은 아이템."
                });
                OnReceiveResponse?.Invoke(responseType, error);
                return;
            }

            if (data.Part == "Ring")
            {
                string[] ringSlots = { "RingL", "RingR" };
                string targetSlot = null;
                int? existingItemId = null;

                foreach (string ringPart in ringSlots)
                {
                    if (UserData.equippedItems.TryGetValue(ringPart, out int equippedId))
                    {
                        if (equippedId == -1)
                        {
                            // 비어 있는 슬롯 발견 → 여기에 장착
                            successType = 1;
                            parts = ringPart;
                            prevId = -1;
                            equipId = itemId;
                            _message = $"{ringPart} 슬롯에 장착되었습니다.";

                            targetSlot = ringPart;
                            existingItemId = equippedId;
                            break;
                        }
                        else if (equippedId == itemId)
                        {
                            successType = -2;
                            _message = "이 아이템은 현재 장착 중입니다.";
                            break;
                        }
                        else if (targetSlot == null)
                        {
                            // 첫 번째 장착 슬롯만 저장     반지가 두쪽 다 껴있다면 RingL만 교체
                            targetSlot = ringPart;
                            existingItemId = equippedId;
                        }
                    }
                }

                // 여기까지 왔다는 건 둘 다 차 있음 → 교체
                if (targetSlot != null)
                {
                    successType = 2;
                    parts = targetSlot;
                    prevId = existingItemId ?? -1;
                    equipId = itemId;
                    _message = $"{targetSlot} 슬롯에 장착된 아이템({prevId})이 교체되었습니다.";
                }
            }
            else
            {
                if (UserData.equippedItems.TryGetValue(data.Part, out int equippedId))
                {
                    if (equippedId == itemId)
                    {
                        //Debug.Log("이 아이템은 현재 장착 중입니다.");
                        successType = -2;
                        _message = "이 아이템은 현재 장착 중입니다.";
                    }
                    else
                    {
                        //Debug.Log($"이 파츠에는 다른 아이템({equippedId})이 장착되어 있습니다.");
                        successType = 2;
                        parts = data.Part;
                        prevId = equippedId;
                        equipId = itemId;
                        _message = $"이 파츠에는 다른 아이템({equippedId})이 장착되어 있습니다 - 교체 되었습니다.";
                    }
                }
                else
                {
                    //Debug.Log("이 파츠에는 아직 아무 것도 장착되지 않았습니다.");
                    successType = 1;
                    parts = data.Part;
                    prevId = -1;
                    equipId = itemId;
                    _message = "장착 되었습니다.";
                }
            }
        }
        else
        {
            successType = -1;
            _message = "존재하지 않는 아이템입니다.";
        }

        if (successType > 0)
        {
            // [DB갱신]
            UserData.equippedItems[parts] = equipId;
        }

        var msg = JsonConvert.SerializeObject(new
        {
            success = successType > 0 ? true : false,
            parts = parts,
            prevId = prevId,
            equipId = equipId,
            message = _message
        });

        OnReceiveResponse?.Invoke(responseType, msg);
    }
    private static void UnEquipItem(string responseType, string payload)
    {
        var jObj = JObject.Parse(payload);
        int itemId = jObj["itemId"]?.Value<int>() ?? -1;
        if (itemId == -1)
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "(-1) 잘못된 아이템"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 유효한 아이템인지 체크
        // 보유한 아이템인지 체크
        // 이미 착용중인 아이템인지 체크(이미 착용중인데 또 착용예외처리)
        int successType = -1;
        string _message = "";
        string parts = "";
        int equipId = -1;
        int prevId = itemId;

        if (DataManager.Instance.itemData.TryGetValue(itemId, out DATA.ItemData data))
        {
            if (!UserData.ownedItems.Contains(itemId))
            {
                var error = JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "보유하지 않은 아이템."
                });
                OnReceiveResponse?.Invoke(responseType, error);
                return;
            }

            if (data.Part == "Ring")
            {
                // RingL, RingR 둘 다 확인
                string[] ringSlots = { "RingL", "RingR" };
                foreach (string slot in ringSlots)
                {
                    if (UserData.equippedItems.TryGetValue(slot, out int equippedId))
                    {
                        if (equippedId == -3)
                        {
                            successType = -1;
                            _message = "아이템이 장착되지 않은 슬롯입니다. 오류";
                        }
                        else if (equippedId == itemId)
                        {
                            successType = 1;
                            parts = slot;
                            equipId = -1;
                            prevId = itemId;
                            _message = $"{slot} 슬롯에서 장착 해제되었습니다.";

                            // [DB 갱신]
                            UserData.equippedItems[slot] = -1;
                            break;
                        }
                    }
                }

                if (successType != 1)
                {
                    successType = -2;
                    _message = "이 아이템은 현재 장착 중이 아닙니다.";
                }
            }
            else
            {
                if (UserData.equippedItems.TryGetValue(data.Part, out int equippedId))
                {
                    if (equippedId == itemId)
                    {
                        successType = 1;
                        parts = data.Part;
                        equipId = -1;
                        _message = "이 아이템은 현재 장착 중입니다 - 장착 해제되었습니다.";
                        prevId = equippedId;
                    }
                }
                else
                {
                    successType = -3;
                    _message = "이 파츠에는 아직 아무 것도 장착되지 않았습니다.";
                }
            }
        }
        else
        {
            successType = -1;
            _message = "존재하지 않는 아이템입니다.";
        }

        if (successType > 0)
        {
            // [DB갱신]
            UserData.equippedItems[parts] = -1;
        }

        var msg = JsonConvert.SerializeObject(new
        {
            success = successType > 0 ? true : false,
            parts = parts,
            prevId = prevId,
            equipId = equipId,
            message = _message
        });

        OnReceiveResponse?.Invoke(responseType, msg);
    }
    private static void EquipHero(string responseType, string payload)
    {
        var jObj = JObject.Parse(payload);
        int heroId = jObj["heroId"]?.Value<int>() ?? -1;
        int prevposition = jObj["prevposition"]?.Value<int>() ?? -1;
        int position = jObj["position"]?.Value<int>() ?? -1;
        if (heroId == -1)
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "(-1) 잘못된 히어로"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 유효한 히어로인지 체크
        // 보유한 히어로인지 체크
        // 이미 착용중인 히어로인지 체크(이미 착용중인데 또 착용예외처리)
        int successType = -1;
        string _message = "";
        int prevId = -1;
        int prevpos = -1;
        int pos = -1;
        int equipId = -1;

        if (DataManager.Instance.heroData.TryGetValue(heroId, out DATA.HeroData data))
        {
            if (!UserData.ownedHeroIds.Contains(heroId))
            {
                var error = JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "보유하지 않은 히어로."
                });
                OnReceiveResponse?.Invoke(responseType, error);
                return;
            }

            var id = UserData.equippedHeroIds[position.ToString()];
            if (id == -1)
            {
                //Debug.Log("이 파츠에는 아직 아무 것도 장착되지 않았습니다.");
                successType = 1;
                prevId = -1;
                prevpos = prevposition;
                pos = position;
                equipId = heroId;
                _message = "장착 되었습니다.";
            }
            else
            {
                if (id == heroId)
                {
                    successType = -2;
                    _message = "이 히어로는 현재 장착 중입니다.";
                }
                else
                {
                    successType = 2;
                    prevId = id;
                    prevpos = prevposition;
                    pos = position;
                    equipId = heroId;
                    _message = "기존 이 위치에 장착된 히어로는 해제되고, 선택한 히어로가 장착됩니다.";
                }
            }
        }
        else
        {
            successType = -1;
            _message = "존재하지 않는 히어로입니다.";
        }

        if (successType > 0)
        {
            // [DB갱신]
            if (prevpos > 0)
            {
                UserData.equippedHeroIds[prevpos.ToString()] = -1;
            }
            UserData.equippedHeroIds[pos.ToString()] = equipId;
        }

        var msg = JsonConvert.SerializeObject(new
        {
            success = successType > 0 ? true : false,
            prevId = prevId,
            prevPos = prevposition,
            equipPos = position,
            equipId = equipId,
            message = _message
        });

        OnReceiveResponse?.Invoke(responseType, msg);
    }
    private static void UnEquipHero(string responseType, string payload)
    {
        var jObj = JObject.Parse(payload);
        int heroId = jObj["heroId"]?.Value<int>() ?? -1;
        int position = jObj["position"]?.Value<int>() ?? -1;
        if (heroId == -1)
        {
            var error = JsonConvert.SerializeObject(new
            {
                success = false,
                message = "(-1) 잘못된 히어로"
            });
            OnReceiveResponse?.Invoke(responseType, error);
            return;
        }

        // 유효한 히어로인지 체크
        // 보유한 히어로인지 체크
        // 이미 착용중인 히어로인지 체크(이미 착용중인데 또 착용예외처리)
        int successType = -1;
        string _message = "";
        int equipId = -1;
        int unequipId = -1;

        if (DataManager.Instance.heroData.TryGetValue(heroId, out DATA.HeroData data))
        {
            if (!UserData.ownedHeroIds.Contains(heroId))
            {
                var error = JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "보유하지 않은 히어로."
                });
                OnReceiveResponse?.Invoke(responseType, error);
                return;
            }

            var id = UserData.equippedHeroIds[position.ToString()];
            if (id == -1)
            {
                //Debug.Log("이 파츠에는 아직 아무 것도 장착되지 않았습니다.");
                successType = -2;
                _message = "장착되어 있지 않은 히어로 입니다.";
            }
            else
            {
                if (id == heroId)
                {
                    successType = 1;
                    unequipId = heroId;
                    _message = "정상적으로 장착 해제되었습니다.";
                }
                else
                {
                    successType = -3;
                    unequipId = heroId;
                    equipId = id;
                    _message = "장착되어 있는 히어로와 다릅니다.";
                }
            }
        }
        else
        {
            successType = -1;
            _message = "존재하지 않는 히어로입니다.";
        }

        if (successType > 0)
        {
            // [DB갱신]
            if (position > 0)
            {
                UserData.equippedHeroIds[position.ToString()] = -1;
            }
        }

        var msg = JsonConvert.SerializeObject(new
        {
            success = successType > 0 ? true : false,
            unEquipPos = position,
            unEquipId = unequipId,
            equipId = equipId,
            message = _message
        });

        OnReceiveResponse?.Invoke(responseType, msg);
    }
}
