using System;
using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDex : PopupBase, IPopupDex
{
    private HeroData heroData = null;
    public Toggle selectPos1;
    public Toggle selectPos2;
    public Toggle selectPos3;
    void Awake()
    {
    }
    
    public override void Start()
    {
        base.Start();

        selectPos1.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(1); });
        selectPos2.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(2); });
        selectPos3.onValueChanged.AddListener((isOn) => { if (isOn) OnToggleChanged(3); });
    }

    public override void init()
    {
        base.init();
    }

    void OnToggleChanged(int index)
    {
        Debug.Log($"선택된 포지션: {index}");
    }

    public int GetSelectedIndex()
    {
        if (selectPos1.isOn) return 1;
        if (selectPos2.isOn) return 2;
        if (selectPos3.isOn) return 3;
        return 0; // 아무것도 선택되지 않음 (AllowSwitchOff=true인 경우 가능)
    }

    public override void Equip()
    {
        if (heroData == null)
            return;

        if (equiped)
        {
            Unequip();
            return;
        }
        else
        {
            Debug.Log("Equip Clicked");
            var prevPos = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
            var pos = GetSelectedIndex();
            if (pos == 0)
            {
                return;
            }

            var payloadObj = new
            {
                heroId = heroData.Id,
                prevposition = prevPos,
                position = pos
            };

            string payloadJson = JsonConvert.SerializeObject(payloadObj);
            NetworkManager.SendRequest_Test("EquipHero", payloadJson);
        }
    }
    public override void Unequip()
    {
        Debug.Log("Unequip Clicked");

        var position = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
        var payloadObj = new
        {
            heroId = heroData.Id,
            position = position,
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("UnEquipHero", payloadJson);
    }

    public void SetItem(HeroData heroData)
    {
        this.heroData = heroData;
        var position = GameMyData.Instance.getEquippedDexIndex(int.Parse(heroData.Id));
        switch (position)
        {
            case "1": selectPos1.isOn = true; break;
            case "2": selectPos2.isOn = true; break;
            case "3": selectPos3.isOn = true; break;
            default: break;
        }
    }
}
