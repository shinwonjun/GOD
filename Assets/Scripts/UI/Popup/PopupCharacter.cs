using System;
using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharacter : PopupBase, IPopupItem
{
    private ItemData itemData = null;
    public override void Equip()
    {
        // 파츠는 이미 착용되어 있기 때문에 착용 버튼은 없다
        base.Equip();
    }
    public override void Unequip()
    {
        Debug.Log("Unequip Clicked");
        var payloadObj = new
        {
            itemId = itemData.Id
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("UnEquipItem", payloadJson);
    }
    
    public override PopupBase ShowPopup()
    {
        gameObject.SetActive(true);
        uiTitle.text = itemData.Name;
        uiDescription.text = itemData.Description;

        equiped = true; // 내 캐릭터가 장착한 아이템을 클릭한거라 무조건 true
        uiEquipBtnText.text = equiped ? "UNEQUIP" : "EQUIP";
        return this;
    }

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
    }
}
