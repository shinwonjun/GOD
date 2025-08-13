using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupItem : PopupBase, IPopupItem
{
    [SerializeField] public TextMeshProUGUI uiOption1_a;
    [SerializeField] public TextMeshProUGUI uiOption1_b;
    [SerializeField] public TextMeshProUGUI uiOption2;
    [SerializeField] public TextMeshProUGUI uiOption3;
    private ItemData itemData = null;
    public override void Equip()
    {
        if (itemData == null)
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
                itemId = itemData.Id
            };

            string payloadJson = JsonConvert.SerializeObject(payloadObj);
            NetworkManager.SendRequest_Test("EquipItem", payloadJson);
        }
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

        equiped = GameMyData.Instance.checkEquipedItem(int.Parse(itemData.Id));
        return this;
    }

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
    }
}