
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    // Stat Tab
    public Dictionary<STATUS_UI.Stat, StatView> statHandlers = new Dictionary<STATUS_UI.Stat, StatView>();

    // Inventory Tab
    public Dictionary<string, ItemSlotView> inventoryHandlers = new Dictionary<string, ItemSlotView>();

    // Character Tab
    public Dictionary<STATUS_UI.Character, CharacterSlotView> characterHandlers = new Dictionary<STATUS_UI.Character, CharacterSlotView>();

    // Popup
    public Dictionary<POPUP.POPUP, PopupBase> popupHandlers = new Dictionary<POPUP.POPUP, PopupBase>();

    public Transform PopupPanel { get; set; } = null;
    public GameObject currentPopup { get; set; } = null;

    protected override void Awake()
    {
        PopupPanel = transform.Find("Canvas/PopupPanel");
    }
    void Start()
    {
        initHandler();
        DataManager.Instance.InitData();
    }

    void initHandler()
    {

    }

    public void ShowPopup(string description)
    {
        currentPopup = popupHandlers[POPUP.POPUP.item].ShowPopup(description);
    }

    public void HidePopup()
    {
        if (currentPopup)
        {
            currentPopup.GetComponent<PopupBase>()?.HidePopup();
        }
    }
}
