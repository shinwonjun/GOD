using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class SlotViewBase<TData> : MonoBehaviour, IPointerClickHandler
    where TData : class
{
    public STATUS_UI.TAB tabType { get; set; } = STATUS_UI.TAB.None;
    public POPUP.POPUP popupType { get; set; } = POPUP.POPUP.None;
    public abstract TData GetData();
    public abstract void SetData(TData newData);

    public abstract void ShowPopup();

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        TData data = GetData();
        ShowPopup();
    }

    public virtual void Refresh(int id)
    {
    }
}
