
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] public HeroHandler heroHandlers;

    void Start()
    {
        NetworkManager.Start();
        NetworkManager.SendRequest_Test("StartGame", "");
    }
    public async Task StartGame(string json)
    {
        GameMyData.Instance.LoadGameInfoJson(json);

        await UIManager.Instance.LoadDataUI();
        await LoadGame();
    }

    public async Task LoadGame()
    {
        await heroHandlers.LoadHeroEnemy();
        await Task.Delay(0);

        NetworkManager.SendRequest_Test("GetServerTime", "");       // 현재 서버 시간 가져오고
        NetworkManager.SendRequest_Test("GetLastClaimTime", "");    // 재접속 전 마지막 시간 가져오고 + 보상
    }
}
