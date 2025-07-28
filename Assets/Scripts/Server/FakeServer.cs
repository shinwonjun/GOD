using System;
using System.Collections.Generic;
using System.Data;
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
    public List<int> equippedHeroIds;
    public Dictionary<string, int> equippedItems;
    public List<int> ownedItems;
    public BigInteger coin;
    public BigInteger diamond;
    public FakeEnemyData enemy;

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
            ""diamond"": 100,

            ""enemy"": {
                ""EnemyId"": 39,        // 39 ~ 61번이 데몬 진영 캐릭터임 악당으로 쓰자 - 나중에 데몬 진영에서 랜덤하게 뽑도록 적용
                ""Level"": 1,           // 테스트
            }
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

        var response = JsonConvert.SerializeObject(new
        {
            success = true,
            reward = reward,
            totalCoin = UserData.coin
        });

        OnReceiveResponse?.Invoke(responseType, response);
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
}
