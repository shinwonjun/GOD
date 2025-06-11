using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotView : SlotViewBase<DATA.EquipslotData>
{
    [SerializeField] private Image image;
    [SerializeField] public STATUS_UI.Character type { get; set; } = STATUS_UI.Character.None;

    private DATA.EquipslotData data;
    public override DATA.EquipslotData GetData() => data;
    public override void SetData(DATA.EquipslotData newData)
    {
        tabType = STATUS_UI.TAB.Character;
        popupType = POPUP.POPUP.character;
        data = newData;
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
    }
}
