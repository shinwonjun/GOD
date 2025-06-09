using UnityEngine;

public class CharacterSlotView : SlotViewBase<DATA.CharacterData>
{
    [SerializeField]
    public STATUS_UI.Character type { get; set; } = STATUS_UI.Character.None;

    private DATA.CharacterData data;
    public override DATA.CharacterData GetData() => data;
    public override void SetData(DATA.CharacterData newData) => data = newData;
    private void Awake()
    {
        popupType = POPUP.POPUP.character;
    }

    public override void ShowPopup()
    {
        UIManager.Instance.ShowPopup(data.Description, popupType);
    }
}
