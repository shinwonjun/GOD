
using System.Threading.Tasks;

public class GameManager : MonoSingleton<GameManager>
{
    void Start()
    {
        NetworkManager.Start();
        NetworkManager.SendRequest("PlayerData", "");
    }
    public void StartGame(string playerdata)
    {
        UIManager.Instance.LoadDataUI();
    }
}
