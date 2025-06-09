using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : SlotViewBase<DATA.ItemData>
{
    private DATA.ItemData data;
    public override DATA.ItemData GetData() => data;
    public override void SetData(DATA.ItemData newData) => data = newData;
    private void Awake()
    {
        popupType = POPUP.POPUP.item;
    }

    public override void ShowPopup()
    {
        UIManager.Instance.ShowPopup(data.Description, popupType);
    }
}
