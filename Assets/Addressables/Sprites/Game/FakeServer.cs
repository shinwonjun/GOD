using System;
using UnityEngine;

public static class FakeServer
{
    // (requestType, responseData)
    public static Action<string, string> OnReceiveResponse;

    public static void Request(string requestType, string payload)
    {
        Debug.Log($"[FakeServer] Received request: {requestType} / payload: {payload}");

        switch (requestType)
        {
            case "PlayerData":
                SimulatePlayerData(requestType, payload);
                break;
            default:
                Debug.LogWarning("[FakeServer] Unknown request type.");
                break;
        }
    }

    private static void SimulatePlayerData(string responseType, string playerId)
    {
        string response = $"{{\"playerId\":\"{playerId}\",\"score\":123,\"level\":5}}";
        OnReceiveResponse?.Invoke(responseType, response);
    }
}
