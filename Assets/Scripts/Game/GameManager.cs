
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
        GameMyData.Instance.LoadFromJson(json);

        await UIManager.Instance.LoadDataUI();
        await LoadGame();
    }

    public async Task LoadGame()
    {
        await heroHandlers.LoadHeroEnemy();
        await Task.Delay(0);
    }
}
