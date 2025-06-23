using System;
using Newtonsoft.Json;
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
            case "StartGame":
                SimulateStartGame(requestType, payload);
                break;
            default:
                Debug.LogWarning("[FakeServer] Unknown request type.");
                break;
        }
    }

    private static void SimulateStartGame(string responseType, string playerId)
    {
         string response = @"{
            ""userId"": {
                ""userId"": 99999
            },
            ""statLevels"": {
                ""Level"": 12,
                ""AttackPower"": 25,
                ""AttackSpeed"": 7,
                ""CriticalChance"": 15,
                ""CriticalDamage"": 30
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
            ]
        }";

        OnReceiveResponse?.Invoke(responseType, response);
    }
}
