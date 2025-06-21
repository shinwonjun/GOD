using System.Collections.Generic;

public class GameMyData : Singleton<GameMyData>
{
    public string id { get; set; } = "";
    public Dictionary<STATUS_UI.Stat, int> dicStatLevel = new Dictionary<STATUS_UI.Stat, int>();
}
