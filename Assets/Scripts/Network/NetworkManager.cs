using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkManager
{
    private const string apiUrl = "https://example.com/api/";  // 실제 REST API 서버 URL로 변경

    public static void Start()
    {
        // 서버 응답 등록
        FakeServer.OnReceiveResponse = async (type, data) =>
        {
            await HandlePacket(type, data);
        };
    }

    public static void SendRequest(string requestType, string payload)
    {
        Debug.Log($"[NetworkManager] Sending request: {requestType} / {payload}");

        // 서버에 요청 보내기 (비동기적으로 실행)
        // API 엔드포인트 URL을 지정하고 요청을 `
        string endpoint = apiUrl + requestType;  // 예시: "StartGame"
        SendHttpRequest(endpoint, payload);
    }

    public static void SendRequest_Test(string requestType, string payload)
    {
        Debug.Log($"[NetworkManager] Sending request: {requestType} / payload: {payload}");

        // FakeServer에 요청을 보내서 응답 받기
        FakeServer.Request(requestType, payload);
    }
    // HTTP 요청 보내기
    private static void SendHttpRequest(string endpoint, string payload)
    {
        // UnityWebRequest를 사용하여 POST 요청을 보냄
        UnityWebRequest request = new UnityWebRequest(endpoint, "POST");

        // 요청의 Body에 JSON 형태로 데이터를 추가
        byte[] jsonToSend = new UTF8Encoding().GetBytes(payload);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 비동기 요청
        request.SendWebRequest().completed += (asyncOperation) =>
        {
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"[NetworkManager] Error: {request.error}");
            }
            else
            {
                // 응답 받은 데이터 처리
                string response = request.downloadHandler.text;
                Debug.Log($"[NetworkManager] Response: {response}");

                // 응답 데이터에서 responseType을 추출하고, HandlePacket 호출
                string responseType = ExtractResponseType(response);
                HandlePacket(responseType, response);
            }
        };
    }
    private static string ExtractResponseType(string json)
    {
        // JSON 파싱하여 responseType 추출 (간단한 예시: 첫 번째 필드로 가정)
        // 실제 JSON 형식에 맞춰서 필요한 파싱 방식으로 변경해야 함
        var jsonData = JsonUtility.FromJson<ResponseData>(json);
        return jsonData.responseType; // 응답에서 responseType 추출
    }

    // 응답 패킷 처리
    private static async Task HandlePacket(string responseType, string json)
    {
        Debug.Log($"[NetworkManager] HandlePacket ResponseType: {responseType}, JSON: {json}");

        // 응답에 따라 게임 상태 업데이트
        switch (responseType)
        {
            case "StartGame":
                await GameManager.Instance.StartGame(json);
                break;
            case "GetServerTime":
                var serverTimeObj = GameMyData.Instance.LoadFromJson(json, typeof(ServerTimeResponse));
                var serverTime = (ServerTimeResponse)serverTimeObj;
                CurrencyManager.Instance.SetServerTime(DateTime.Parse(serverTime.serverTime));
                Debug.Log("[NetworkManager] GetServerTime - " + serverTime.serverTime);
                break;
            case "GetLastClaimTime":
                var lastClaimTimeObj = GameMyData.Instance.LoadFromJson(json, typeof(LastClaimTimeResponse));
                var lastClaimTime = (LastClaimTimeResponse)lastClaimTimeObj;
                CurrencyManager.Instance.SetLastClaimTime(DateTime.Parse(lastClaimTime.lastClaimTime));
                Debug.Log("[NetworkManager] SetLastClaimTime - " + lastClaimTime.lastClaimTime);
                break;
            case "AddCoin":
                var coinObj = GameMyData.Instance.LoadFromJson(json, typeof(AddCoinResponse));
                var coin = (AddCoinResponse)coinObj;
                if (coin.success)
                    GameMyData.Instance.Coin = coin.newCoin;
                Debug.Log($"[NetworkManager] AddCoin:{coin.success} - 현재 코인: {GameMyData.Instance.Coin}");
                break;
            case "UpgradeStat":
                var statUpgradeObj = GameMyData.Instance.LoadFromJson(json, typeof(StatUpgradeResponse));
                var statUpgrade = (StatUpgradeResponse)statUpgradeObj;
                if (statUpgrade.success)
                {
                    GameMyData.Instance.Coin = statUpgrade.remainingCoin;
                    STATUS_UI.Stat statType;
                    if (statUpgrade.stat == "LevelUpgrade")
                        statType = STATUS_UI.Stat.Level;
                    else if (statUpgrade.stat == "AttackPower")
                        statType = STATUS_UI.Stat.AttackPower;
                    else if (statUpgrade.stat == "AttackSpeed")
                        statType = STATUS_UI.Stat.AttackSpeed;
                    else if (statUpgrade.stat == "CriticalChance")
                        statType = STATUS_UI.Stat.CriticalChance;
                    else if (statUpgrade.stat == "CriticalDamage")
                        statType = STATUS_UI.Stat.CriticalDamage;
                    else
                        return;

                    GameMyData.Instance.UserData.statLevelsByIndex[(int)statType] = statUpgrade.newLevel;
                    UIManager.Instance.statHandlers[statType].IncreaseLevel(statUpgrade.newLevel);
                    Debug.Log($"[NetworkManager] UpgradeStat:{statUpgrade.success} - 레벨: {statUpgrade.newLevel} , 현재 코인: {statUpgrade.message}");
                }
                else
                {
                    Debug.Log($"[NetworkManager] UpgradeStat:{statUpgrade.success} - 현재 코인: {statUpgrade.message}");
                }
                break;
            case "KillEnemy":
                var killEnemyObj = GameMyData.Instance.LoadFromJson(json, typeof(KillEnemyResponse));
                var killEnemy = (KillEnemyResponse)killEnemyObj;

                if (killEnemy.success)
                {
                    GameMyData.Instance.Coin = killEnemy.totalCoin;
                    Debug.Log($"[NetworkManager] KillEnemy 성공 - 보상: {killEnemy.reward}, 총 코인: {killEnemy.totalCoin}");

                    GameMyData.Instance.UserData.enemy.EnemyId = killEnemy.EnemyId;
                    GameMyData.Instance.UserData.enemy.Level = killEnemy.Level;
                }
                else
                {
                    Debug.LogWarning($"[NetworkManager] KillEnemy 실패 - 사유: {killEnemy.message}");
                }

                await GameManager.Instance.SetNextEnemy(killEnemy.success);
                break;
            case "EquipItem":
                var EquipItemObj = GameMyData.Instance.LoadFromJson(json, typeof(EquipItem));
                var equipItem = (EquipItem)EquipItemObj;
                if (equipItem.success)
                {
                    GameMyData.Instance.UserData.equippedItems[equipItem.parts] = equipItem.equipId;
                    UIManager.Instance.refreshInventory(equipItem.prevId, equipItem.equipId);
                    UIManager.Instance.currentPopup.HidePopup();
                    UIManager.Instance.OnRefreshEquipItem();
                }

                Debug.Log($"[NetworkManager] EquipItem:{equipItem.success} - message : {equipItem.message}");
                break;
            case "UnEquipItem":
                var UnEquipItemObj = GameMyData.Instance.LoadFromJson(json, typeof(UnEquipItem));
                var unEquipItem = (UnEquipItem)UnEquipItemObj;
                if (unEquipItem.success)
                {
                    GameMyData.Instance.UserData.equippedItems[unEquipItem.parts] = unEquipItem.equipId;
                    UIManager.Instance.refreshInventory(unEquipItem.prevId, unEquipItem.equipId);
                    UIManager.Instance.currentPopup.HidePopup();
                    UIManager.Instance.OnRefreshEquipItem();
                }

                Debug.Log($"[NetworkManager] UnEquipItem:{unEquipItem.success} - message : {unEquipItem.message}");
                break;
            case "EquipHero":
                var EquipHeroObj = GameMyData.Instance.LoadFromJson(json, typeof(EquipHero));
                var equipHero = (EquipHero)EquipHeroObj;
                if (equipHero.success)
                {
                    if (equipHero.prevPos > 0)
                    {
                        GameMyData.Instance.UserData.equippedHeroIds[equipHero.prevPos.ToString()] = -1;
                    }
                    GameMyData.Instance.UserData.equippedHeroIds[equipHero.equipPos.ToString()] = equipHero.equipId;

                    GameManager.Instance.heroHandlers.OnRefreshHero();
                    UIManager.Instance.refreshDex(equipHero.prevId);
                    UIManager.Instance.currentPopup.HidePopup();
                }

                Debug.Log($"[NetworkManager] EquipHero:{equipHero.success} - message : {equipHero.message}");
                break;
            case "UnEquipHero":
                var UnquipHeroObj = GameMyData.Instance.LoadFromJson(json, typeof(UnEquipHero));
                var unequipHero = (UnEquipHero)UnquipHeroObj;
                if (unequipHero.success)
                {
                    if (unequipHero.unEquipPos > 0)
                    {
                        GameMyData.Instance.UserData.equippedHeroIds[unequipHero.unEquipPos.ToString()] = -1;
                    }

                    GameManager.Instance.heroHandlers.OnRefreshHero();
                    UIManager.Instance.refreshDex(unequipHero.unEquipId);
                    UIManager.Instance.currentPopup.HidePopup();
                }

                Debug.Log($"[NetworkManager] UnEquipHero:{unequipHero.success} - message : {unequipHero.message}");
                break;
            case "GetHeroOptions":
                var HeroOptionObj = GameMyData.Instance.LoadFromJson(json, typeof(HeroOption));
                var option = (HeroOption)HeroOptionObj;
                if (option.success)
                {
                    var target = GameMyData.Instance.UserData.heroOptions.Find(h => h.Id == option.heroId);
                    target.options.Clear();

                    foreach (var kv in option.options) // option.options = Dictionary<int, string>
                    {
                        int slot = kv.Key;              // key 값 (10, 11, 20, 31 ...)
                        string value = kv.Value;        // "1006,6.3"

                        // 쉼표 기준 분리
                        var parts = value.Split(',');
                        if (parts.Length >= 2)
                        {
                            string optIdStr = parts[0].Trim();   // "1006"
                            string rolledStr = parts[1].Trim();  // "6.3"

                            int optId = int.Parse(optIdStr);
                            float rolled = float.Parse(rolledStr, CultureInfo.InvariantCulture);

                            foreach (var key in DataManager.Instance.heroOptionData)
                            {
                                var found = key.Value.FirstOrDefault(o => o.Id == optId.ToString());
                                if (found != null)
                                {
                                    //return (found.Min, found.Max);

                                    if (!target.options.ContainsKey(slot))
                                        target.options[slot] = new Dictionary<int, List<string>>();

                                    target.options[slot][optId] = new List<string>
                                    {
                                        found.Min.ToString(CultureInfo.InvariantCulture),
                                        found.Max.ToString(CultureInfo.InvariantCulture),
                                        rolled.ToString(CultureInfo.InvariantCulture)
                                    };

                                    int a = 0;
                                    break;
                                }
                            }

                            Console.WriteLine($"Slot={slot}, OptionId={optId}, Value={rolled}");
                        }
                    }

                    GameMyData.Instance.Diamond = option.diamond;
                    UIManager.Instance.currentPopup.Refresh();
                }
                Debug.Log($"[NetworkManager] GetHeroOptions:{option.success} - message : {option.message}");
                break;
            case "GetItemOptions":
                var ItemOptionObj = GameMyData.Instance.LoadFromJson(json, typeof(ItemOption));
                var itemOption = (ItemOption)ItemOptionObj;
                if (itemOption.success)
                {
                    var target = GameMyData.Instance.UserData.itemOptions.Find(h => h.Id == itemOption.itemId);
                    target.options.Clear();

                    foreach (var kv in itemOption.options) // option.options = Dictionary<int, string>
                    {
                        int slot = kv.Key;              // key 값 (10, 11, 20, 31 ...)
                        string value = kv.Value;        // "1006,6.3"

                        // 쉼표 기준 분리
                        var parts = value.Split(',');
                        if (parts.Length >= 2)
                        {
                            string optIdStr = parts[0].Trim();   // "1006"
                            string rolledStr = parts[1].Trim();  // "6.3"

                            int optId = int.Parse(optIdStr);
                            float rolled = float.Parse(rolledStr, CultureInfo.InvariantCulture);

                            foreach (var key in DataManager.Instance.itemOptionData)
                            {
                                var found = key.Value.FirstOrDefault(o => o.Id == optId.ToString());
                                if (found != null)
                                {
                                    //return (found.Min, found.Max);

                                    if (!target.options.ContainsKey(slot))
                                        target.options[slot] = new Dictionary<int, List<string>>();

                                    target.options[slot][optId] = new List<string>
                                    {
                                        found.Min.ToString(CultureInfo.InvariantCulture),
                                        found.Max.ToString(CultureInfo.InvariantCulture),
                                        rolled.ToString(CultureInfo.InvariantCulture)
                                    };

                                    int a = 0;
                                    break;
                                }
                            }

                            Console.WriteLine($"Slot={slot}, OptionId={optId}, Value={rolled}");
                        }
                    }

                    GameMyData.Instance.Diamond = itemOption.diamond;
                    UIManager.Instance.currentPopup.Refresh();
                }
                Debug.Log($"[NetworkManager] GetItemOptions:{itemOption.success} - message : {itemOption.message}");
                break;
            default:
                Debug.LogWarning($"[NetworkManager] Unhandled responseType: {responseType}");
                break;
        }
    }

    // 응답을 파싱할 때 사용할 구조체 (예시)
    [System.Serializable]
    private class ResponseData
    {
        public string responseType;
        public string message;
    }
}
