using UnityEngine;

public interface IPopupBase
{
    /// <summary>
    /// 팝업을 화면에 띄운다.
    /// </summary>
    PopupBase ShowPopup();

    /// <summary>
    /// 착용 버튼
    /// </summary>
    void Equip();

    /// <summary>
    /// 해제 버튼
    /// </summary>
    void Unequip();

    /// <summary>
    /// 팝업을 닫는다.
    /// </summary>
    void HidePopup();
}

public interface IPopupItem
{
    void SetItem(DATA.ItemData itemData);
    void ResetOptions();
}
public interface IPopupDex
{
    void SetItem(DATA.HeroData heroData);
    void ResetOptions();
}