using System;
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
                Debug.LogError($"Error: {request.error}");
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
                Debug.Log("GetServerTime - " + serverTime.serverTime);
                break;
            case "GetLastClaimTime":
                var lastClaimTimeObj = GameMyData.Instance.LoadFromJson(json, typeof(LastClaimTimeResponse));
                var lastClaimTime = (LastClaimTimeResponse)lastClaimTimeObj;
                CurrencyManager.Instance.SetLastClaimTime(DateTime.Parse(lastClaimTime.lastClaimTime));
                Debug.Log("SetLastClaimTime - " + lastClaimTime.lastClaimTime);
                break;
            case "AddCoin":
                var coinObj = GameMyData.Instance.LoadFromJson(json, typeof(AddCoinResponse));
                var coin = (AddCoinResponse)coinObj;
                if (coin.success)
                    GameMyData.Instance.Coin = coin.newCoin;
                Debug.Log($"AddCoin:{coin.success} - 현재 코인: {GameMyData.Instance.Coin}");
                break;
            case "StatUpgrade":
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

                    GameMyData.Instance.dicStatLevel[statType] = statUpgrade.newLevel;
                    UIManager.Instance.statHandlers[statType].IncreaseLevel(statUpgrade.newLevel);
                    Debug.Log($"StatUpgrade:{statUpgrade.success} - 레벨: {statUpgrade.newLevel} , 현재 코인: {statUpgrade.message}");
                }
                else
                {
                    Debug.Log($"StatUpgrade:{statUpgrade.success} - 현재 코인: {statUpgrade.message}");
                }
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
