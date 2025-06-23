using UnityEngine;
using UnityEngine.UI;

public class DexSlotView : SlotViewBase<DATA.HeroData>
{
    [SerializeField] private Image image;
    [SerializeField] private Image imageHas;
    [SerializeField] private GameObject objEquiped;
    private DATA.HeroData data;
    public override DATA.HeroData GetData() => data;
    public override void SetData(DATA.HeroData newData)
    {
        tabType = STATUS_UI.TAB.Dex;
        popupType = POPUP.POPUP.item;
        data = newData;

        var has = GameMyData.Instance.hasDex(int.Parse(data.Id));
        imageHas.gameObject.SetActive(!has);

        var checkEquiped = GameMyData.Instance.checkEquipedDex(int.Parse(data.Id));
        objEquiped.SetActive(checkEquiped);

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