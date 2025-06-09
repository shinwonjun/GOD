using UnityEngine;

public interface IPopupBase
{
    /// <summary>
    /// 팝업을 화면에 띄운다.
    /// </summary>
    GameObject ShowPopup(string description);

    /// <summary>
    /// 팝업을 닫는다.
    /// </summary>
    void HidePopup();
}
