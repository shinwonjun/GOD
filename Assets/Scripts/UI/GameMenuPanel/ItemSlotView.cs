using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : SlotViewBase<DATA.ItemData>
{
    [SerializeField] private Image image;
    [SerializeField] private Image imageHas;
    [SerializeField] private GameObject objEquiped;


    private DATA.ItemData data;
    public override DATA.ItemData GetData() => data;
    public override void SetData(DATA.ItemData newData)
    {
        tabType = STATUS_UI.TAB.Inventory;
        popupType = POPUP.POPUP.inventory;
        data = newData;

        var has = GameMyData.Instance.hasItem(int.Parse(data.Id));
        imageHas.gameObject.SetActive(!has);

        var checkEquiped = GameMyData.Instance.checkEquipedItem(int.Parse(data.Id));
        objEquiped.SetActive(checkEquiped);

        setImage(data.Sprite);
    }
    private void Awake()
    {
    }

    public override void ShowPopup()
    {
        //var checkEquiped = GameMyData.Instance.checkEquipedItem(int.Parse(data.Id));
        UIManager.Instance.ShowPopup(popupType, data);
    }
    private void setImage(string _sprite)
    {
        image.sprite = UIManager.Instance.uiAtlas[tabType].GetSprite(_sprite);
    }
    public override void Refresh(int id)
    {
        var itemData = DataManager.Instance.itemData[id];
        SetData(itemData);
    }
}