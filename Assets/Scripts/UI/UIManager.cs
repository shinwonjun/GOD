
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public NavigationTabController tabHandlers;
    // Stat Tab
    public Dictionary<STATUS_UI.Stat, StatSlotView> statHandlers = new Dictionary<STATUS_UI.Stat, StatSlotView>();

    // Inventory Tab
    public Dictionary<string, ItemSlotView> inventoryHandlers = new Dictionary<string, ItemSlotView>();

    // Character Tab
    public Dictionary<STATUS_UI.Character, CharacterSlotView> characterHandlers = new Dictionary<STATUS_UI.Character, CharacterSlotView>();

    // Popup
    public Dictionary<POPUP.POPUP, IPopupBase> popupHandlers = new Dictionary<POPUP.POPUP, IPopupBase>();

    public Transform PopupPanel { get; set; } = null;
    public GameObject currentPopup { get; set; } = null;

    protected override void Awake()
    {
        PopupPanel = transform.Find("Canvas/PopupPanel");
    }
    void Start()
    {
        DataManager.Instance.InitData();
    }

    public void ShowPopup(string description, POPUP.POPUP type)
    {
        currentPopup = popupHandlers[type].ShowPopup(description);
    }
}
