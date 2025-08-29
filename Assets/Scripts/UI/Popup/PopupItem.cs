using System.Linq;
using DATA;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupItem : PopupBase, IPopupItem
{
    [SerializeField] public Button resetButton;
    [SerializeField] public TextMeshProUGUI uiOption1_a;
    [SerializeField] public TextMeshProUGUI uiOption1_b;
    [SerializeField] public TextMeshProUGUI uiOption2;
    [SerializeField] public TextMeshProUGUI uiOption3;
    private ItemData itemData = null;

    public override void Start()
    {
        base.Start();        
        
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetOptions);
    }
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
    public override void Refresh()
    {
        base.Refresh();
        ShowPopup();
    }
    public override PopupBase ShowPopup()
    {
        gameObject.SetActive(true);
        uiTitle.text = itemData.Name;
        uiDescription.text = itemData.Description;

        equiped = GameMyData.Instance.checkEquipedItem(int.Parse(itemData.Id));
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
                    //var option = DataManager.Instance.itemOptionData[type].FirstOrDefault(opt => opt.Id == inner.Key.ToString());
                    var option = DataManager.Instance.itemOptionData
                        .SelectMany(kv => kv.Value)               // 모든 List<HeroOptionData> 펼치기
                        .FirstOrDefault(o => o.Id == inner.Key.ToString());

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
        Debug.Log("Reset Clicked");

        var payloadObj = new
        {
            itemId = itemData.Id,
            diamond = GameMyData.Instance.UserData.diamond,
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);
        NetworkManager.SendRequest_Test("GetItemOptions", payloadJson);
    }
}