
using System.Threading.Tasks;

public class GameManager : MonoSingleton<GameManager>
{
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
        await Task.Delay(0);
    }
}
