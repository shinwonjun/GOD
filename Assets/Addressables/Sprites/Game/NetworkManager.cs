using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkManager
{
    private const string apiUrl = "https://example.com/api/";  // 실제 REST API 서버 URL로 변경

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

        // 서버에 요청 보내기 (비동기적으로 실행)
        // API 엔드포인트 URL을 지정하고 요청을 보냄
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
    private static void HandlePacket(string responseType, string json)
    {
        Debug.Log($"[NetworkManager] HandlePacket ResponseType: {responseType}, JSON: {json}");

        // 응답에 따라 게임 상태 업데이트
        switch (responseType)
        {
            case "StartGame":
                GameManager.Instance.StartGame();
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
