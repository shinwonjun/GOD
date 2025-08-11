using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HeroBase : MonoBehaviour
{
    private const int OptionSlot1Count = 2;
    private const int OptionSlot2Count = 1;
    private const int OptionSlot3Count = 1;
    [SerializeField] protected Image image;
    [HideInInspector] protected int position = -1;
    [HideInInspector] protected DATA.HeroData info;
    [HideInInspector] protected Dictionary<int, Dictionary<int, DATA.HeroOptionData>> Options = new();
    public GAME.HeroType heroType { get; set; } = GAME.HeroType.None;

    public virtual void init(DATA.HeroData data, int pos)
    {
        info = data;
        if (info == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Options[0] = new Dictionary<int, DATA.HeroOptionData>();
        Options[1] = new Dictionary<int, DATA.HeroOptionData>();
        Options[2] = new Dictionary<int, DATA.HeroOptionData>();

        gameObject.SetActive(true);
        heroType = (GAME.HeroType)Enum.Parse(typeof(GAME.HeroType), info.Type);
        position = pos;
        image.sprite = UIManager.Instance.uiAtlas[STATUS_UI.TAB.Dex].GetSprite(info.Sprite);
    }
    public abstract void attack();  // 공격
    public abstract void hit(float beforeHP, float damage, bool isCrit);     // 피격
}
