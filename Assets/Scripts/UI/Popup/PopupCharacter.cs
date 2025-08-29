using System;
using System.Linq;
using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharacter : PopupBase, IPopupItem
{
    [SerializeField] public TextMeshProUGUI uiOption1_a;
    [SerializeField] public TextMeshProUGUI uiOption1_b;
    [SerializeField] public TextMeshProUGUI uiOption2;
    [SerializeField] public TextMeshProUGUI uiOption3;
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

        var options = GameMyData.Instance.UserData.itemOptions.FirstOrDefault(h => h.Id == int.Parse(itemData.Id));
        if (options != null)
        {
            foreach (var outer in options.options) // 바깥 Dictionary<int, Dictionary<int, List<string>>>
            {
                int type = outer.Key;
                string strOptions = "";
                foreach (var inner in outer.Value) // 안쪽 Dictionary<int, List<string>>
                {
                    var option = DataManager.Instance.itemOptionData[type].FirstOrDefault(opt => opt.Id == inner.Key.ToString());

                    if (option != null)
                    {
                        float value1 = float.Parse(inner.Value[0]);
                        float value2 = float.Parse(inner.Value[1]);
                        float value3 = float.Parse(inner.Value[2]);
                        strOptions = $"{option.Description}({value1}~{value2}){value3}";
                    }
                    else
                    {
                        strOptions = "";
                    }
                }

                if (type == (int)GAME.HeroSkilSlot.Slot1_a)
                {
                    uiOption1_a.text = strOptions;
                }
                else if (type == (int)GAME.HeroSkilSlot.Slot1_b)
                {

                    uiOption1_b.text = strOptions;
                }
                else if (type == (int)GAME.HeroSkilSlot.Slot2)
                {
                    uiOption2.text = strOptions;
                }
                else if (type == (int)GAME.ItemSkilSlot.Slot3)
                {
                    uiOption3.text = strOptions;
                }
            }
        }
        return this;
    }

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public void ResetOptions()
    {

    }
}
