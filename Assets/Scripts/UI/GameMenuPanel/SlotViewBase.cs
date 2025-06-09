using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class SlotViewBase<TData> : MonoBehaviour, IPointerClickHandler
    where TData : class
{
    public POPUP.POPUP popupType { get; set; } = POPUP.POPUP.None;
    public abstract TData GetData();
    public abstract void SetData(TData newData);

    public void OnPointerClick(PointerEventData eventData)
    {
        TData data = GetData();
        ShowPopup();
    }

    public abstract void ShowPopup();
}
