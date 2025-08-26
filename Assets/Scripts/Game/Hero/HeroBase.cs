using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HeroBase : MonoBehaviour
{
     protected Animator animator = null;
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

        if (!Options.ContainsKey(0))
            Options[0] = new Dictionary<int, DATA.HeroOptionData>();
        else
            Options[0].Clear(); // 기존 데이터 초기화

        if (!Options.ContainsKey(1))
            Options[1] = new Dictionary<int, DATA.HeroOptionData>();
        else
            Options[1].Clear(); // 기존 데이터 초기화

        if (!Options.ContainsKey(2))
            Options[2] = new Dictionary<int, DATA.HeroOptionData>();
        else
            Options[2].Clear(); // 기존 데이터 초기화

        gameObject.SetActive(true);
        heroType = (GAME.HeroType)Enum.Parse(typeof(GAME.HeroType), info.Type);
        position = pos;
        image.sprite = UIManager.Instance.uiAtlas[STATUS_UI.TAB.Dex].GetSprite(info.Sprite);
    }
    public abstract void attack(bool isCrit);  // 공격
    public abstract void hit(float beforeHP, float damage, bool isCrit);     // 피격
}
