using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public abstract class HeroBase : MonoBehaviour
{
    [SerializeField] protected Image image;
    [HideInInspector] protected int position = -1;
    [HideInInspector] protected DATA.HeroData info;
    public GAME.HeroType heroType { get; set; } = GAME.HeroType.None;

    public virtual void init(DATA.HeroData data, int pos)
    {
        info = data;
        if (info == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        heroType = (GAME.HeroType)Enum.Parse(typeof(GAME.HeroType), info.Type);
        position = pos;
        image.sprite = UIManager.Instance.uiAtlas[STATUS_UI.TAB.Dex].GetSprite(info.Sprite);
    }
    public abstract void attack();  // 공격
    public abstract void hit(float beforeHP, float damage, bool isCrit);     // 피격
}
