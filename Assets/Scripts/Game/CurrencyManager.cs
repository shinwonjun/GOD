using System;
using System.Collections;
using UnityEngine;

public class CurrencyManager : MonoSingleton<CurrencyManager>
{
    public float gainInterval = 10f;  // 60초마다 획득
    public int resourcePerInterval = 100;
    private Coroutine gainCoroutine;

    private DateTime? serverTime = null;
    private DateTime? lastClaimTime = null;


    public void SetServerTime(DateTime time)
    {
        serverTime = time;
        TrySyncReward();
    }

    public void SetLastClaimTime(DateTime time)
    {
        lastClaimTime = time;
        TrySyncReward();
    }
    private void TrySyncReward()
    {
        if (serverTime != null && lastClaimTime != null)
        {
            var delta = serverTime.Value - lastClaimTime.Value;
            int intervals = Mathf.FloorToInt((float)delta.TotalSeconds / gainInterval);
            int reward = intervals * resourcePerInterval;

            NetworkManager.SendRequest_Test("AddCoin", reward.ToString());

            Debug.Log($"[CurrencyManager] 오프라인 보상: {reward} 획득 → request AddCoin");
            StartAutoGain();
        }
    }

    void Start()
    {
    }

    public void StartAutoGain()
    {
        if (gainCoroutine != null)
        {
            StopCoroutine(gainCoroutine);
            gainCoroutine = null;
            Debug.Log("[CurrencyManager] 기존 코루틴 중지");
        }

        gainCoroutine = StartCoroutine(GainResourceLoop());
        Debug.Log("[CurrencyManager] 재화 획득 코루틴 시작");
    }

    public void StopAutoGain()
    {
        if (gainCoroutine != null)
        {
            StopCoroutine(gainCoroutine);
            gainCoroutine = null;
            Debug.Log("[CurrencyManager] 재화 획득 타이머 중지");
        }
    }

    IEnumerator GainResourceLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(gainInterval);
            NetworkManager.SendRequest_Test("AddCoin", resourcePerInterval.ToString());
            Debug.Log($"[로컬] 재화 획득! request AddCoin");
        }
    }

    // private void OnApplicationQuit()
    // {
    //     // 종료 시 서버에 마지막 수령 시간 저장 로직 (추후 확장)
    // }
}
