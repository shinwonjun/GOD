using UnityEngine;
using UnityEngine.UI;

public class DexSlotView : SlotViewBase<DATA.HeroData>
{
    [SerializeField] private Image image;
    private DATA.HeroData data;
    public override DATA.HeroData GetData() => data;
    public override void SetData(DATA.HeroData newData)
    {
        tabType = STATUS_UI.TAB.Dex;
        popupType = POPUP.POPUP.item;
        data = newData;
        setImage(data.Sprite);
    }
    private void Awake()
    {
    }

    public override void ShowPopup()
    {
        UIManager.Instance.ShowPopup(data.Description, popupType);
    }
    private void setImage(string _sprite)
    {
        image.sprite = UIManager.Instance.uiAtlas[tabType].GetSprite(_sprite);
    }
}