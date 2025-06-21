using UnityEngine;

public static class NetworkManager
{
    public static void Start()
    {
        // 서버 응답 등록
        FakeServer.OnReceiveResponse = (type, data) =>
        {
            HandlePacket(type, data);
        };
    }

    public static void SendRequest(string requestType, string payload)
    {
        Debug.Log($"[NetworkManager] Sending request: {requestType} / {payload}");
        FakeServer.Request(requestType, payload);
    }

    private static void HandlePacket(string responseType, string json)
    {
        Debug.Log($"[NetworkManager] HandlePlayerData Response: {json}");
        // JSON 파싱 및 UI 반영 가능

        switch (responseType)
        {
            case "PlayerData":
                GameManager.Instance.StartGame(json);
                break;
            default:
                break;
        }
    }
}
