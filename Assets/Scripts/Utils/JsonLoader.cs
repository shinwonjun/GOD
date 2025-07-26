using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonLoader
{
    /// <summary>
    /// Resources 폴더에서 JSON 파일을 읽어 지정한 타입으로 디시리얼라이즈합니다.
    /// </summary>
    /// <typeparam name="T">파싱할 타입</typeparam>
    /// <param name="resourcePath">Resources 폴더 내 경로(확장자 제외)</param>
    /// <returns>파싱된 객체 또는 null</returns>
    public static T LoadFromResources<T>(string resourcePath) where T : class
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);
        if (jsonFile == null)
        {
            Debug.LogError($"[JsonLoader] Resources/{resourcePath}.json 파일을 찾을 수 없습니다.");
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<T>(jsonFile.text);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"[JsonLoader] JSON 파싱 실패: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 절대 경로에서 JSON 파일을 읽어 지정한 타입으로 디시리얼라이즈합니다.
    /// </summary>
    /// <typeparam name="T">파싱할 타입</typeparam>
    /// <param name="fullPath">절대 경로 포함 파일명</param>
    /// <returns>파싱된 객체 또는 null</returns>
    public static T LoadFromPath<T>(string fullPath) where T : class
    {
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"[JsonLoader] 파일이 존재하지 않습니다: {fullPath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (IOException ex)
        {
            Debug.LogError($"[JsonLoader] 파일 읽기 실패: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Debug.LogError($"[JsonLoader] JSON 파싱 실패: {ex.Message}");
            return null;
        }
    }

    // public static T Deserialize<T>(string json)
    // {
    //     return JsonConvert.DeserializeObject<T>(json);
    // }
}
