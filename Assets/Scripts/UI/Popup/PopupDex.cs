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

    public override void Start()
    {
        base.Start();

        // 리스너 등록
        selectPos1.onValueChanged.AddListener((isOn) => OnToggleChanged(1, isOn));
        selectPos2.onValueChanged.AddListener((isOn) => OnToggleChanged(2, isOn));
        selectPos3.onValueChanged.AddListener((isOn) => OnToggleChanged(3, isOn));
    }

    void OnToggleChanged(int index, bool isOn)
    {
        if (isOn)
        {
            Debug.Log($"선택된 포지션: {index}");
            // 예: 다른 토글 끄기
            DeselectOthers(index);
        }
    }

    void DeselectOthers(int selectedIndex)
    {
        if (selectedIndex != 1) selectPos1.isOn = false;
        if (selectedIndex != 2) selectPos2.isOn = false;
        if (selectedIndex != 3) selectPos3.isOn = false;
    }

    public int GetSelectedIndex()
    {
        if (selectPos1.isOn) return 1;
        if (selectPos2.isOn) return 2;
        if (selectPos3.isOn) return 3;
        return 0; // 아무 것도 선택되지 않은 경우
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
        var payloadObj = new
        {
            itemId = heroData.Id
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("UnEquipHero", payloadJson);
    }

    public void SetItem(HeroData heroData)
    {
        this.heroData = heroData;
    }
}
