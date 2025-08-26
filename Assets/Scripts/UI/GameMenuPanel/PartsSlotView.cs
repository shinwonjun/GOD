using UnityEngine;
using UnityEngine.UI;

public class PartsSlotView : SlotViewBase<DATA.EquipslotData>
{
    [SerializeField] private Image image;
    [SerializeField] public STATUS_UI.Character type { get; set; } = STATUS_UI.Character.None;

    private DATA.EquipslotData data;

    private ITEM.AttachPart parts = ITEM.AttachPart.None;
    public override DATA.EquipslotData GetData() => data;
    public override void SetData(DATA.EquipslotData newData)
    {
        tabType = STATUS_UI.TAB.Character;
        popupType = POPUP.POPUP.character;
        data = newData;

        switch (data.Part)
        {
            case "Pitching":
                parts = ITEM.AttachPart.Pitching;
                break;
            case "Armor":
                parts = ITEM.AttachPart.Armor;
                break;
            case "Gloves":
                parts = ITEM.AttachPart.Gloves;
                break;
            case "Necklace":
                parts = ITEM.AttachPart.Necklace;
                break;
            case "RingL":
                parts = ITEM.AttachPart.RingL;
                break;
            case "RingR":
                parts = ITEM.AttachPart.RingR;
                break;
            case "Shoes":
                parts = ITEM.AttachPart.Shoes;
                break;
        }

        var item = GameMyData.Instance.getEquipedPartsID(parts);

        if (item != null)
        {
            setImage(item.Sprite);
        }
        else
        {
            image.sprite = null;
        }
    }
    private void Awake()
    {
    }

    public override void ShowPopup()
    {
        var itemData = GameMyData.Instance.getEquipedPartsID(parts);
        //var checkEquiped = true;       // 이미 착용하고 있는 슬롯이기 때문에
        UIManager.Instance.ShowPopup(popupType, itemData);
    }
    private void setImage(string _sprite)
    {
        image.sprite = UIManager.Instance.uiAtlas[STATUS_UI.TAB.Inventory].GetSprite(_sprite);
    }
}
