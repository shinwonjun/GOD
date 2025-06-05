using UnityEngine;

/// <summary>
/// MonoBehaviour 기반 싱글톤 베이스 클래스
/// 씬에 중복 인스턴스가 있으면 자동 파괴 처리하며,
/// DontDestroyOnLoad로 씬 전환에도 살아남음.
/// </summary>
/// <typeparam name="T">상속할 타입</typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// 싱글톤 인스턴스 접근용 프로퍼티
    /// </summary>
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[MonoSingleton] {typeof(T)} Instance already destroyed on application quit. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 씬에서 기존 인스턴스 검색
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        // 없으면 새 GameObject 생성 후 붙임
                        var singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                        _instance = singletonObject.AddComponent<T>();

                        DontDestroyOnLoad(singletonObject);
                        Debug.Log($"[MonoSingleton] An instance of {typeof(T)} was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log($"[MonoSingleton] Using existing instance of {typeof(T)}.");
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    /// <summary>
    /// 인스턴스가 이미 존재하면 중복 생성 방지
    /// </summary>
    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[MonoSingleton] Duplicate instance of {typeof(T)} detected. Destroying new one.");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 애플리케이션 종료 시점 플래그 세팅
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}
