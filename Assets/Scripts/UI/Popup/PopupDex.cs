using System;
using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDex : PopupBase, IPopupDex
{
    private HeroData heroData = null;
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
            var payloadObj = new
            {
                heroId = heroData.Id
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
