using System;
using System.Threading;

public abstract class Singleton<T> where T : class, new()
{
    // volatile 키워드로 멀티스레드 접근 시 최신 값을 보장
    private static volatile T _instance;
    private static readonly object _lock = new object();

    protected Singleton()
    {
        // 외부에서 직접 생성 방지
        if (_instance != null)
        {
            throw new InvalidOperationException("Instance already created.");
        }
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // double-check locking 패턴
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}
