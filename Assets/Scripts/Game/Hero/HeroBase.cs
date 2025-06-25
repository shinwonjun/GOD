using UnityEngine;
using UnityEngine.UI;

public abstract class HeroBase : MonoBehaviour
{
    [SerializeField] protected Image image;
    [HideInInspector] protected int position = -1;
    public GAME.HeroType heroType { get; set; } = GAME.HeroType.None;

    public virtual void init(GAME.HeroType type, int pos, string sprite)
    {        
        heroType = type;
        position = pos;
        image.sprite = UIManager.Instance.uiAtlas[STATUS_UI.TAB.Dex].GetSprite(sprite);
    }
    public abstract void attack();  // 공격
    public abstract void hit();     // 피격
}
