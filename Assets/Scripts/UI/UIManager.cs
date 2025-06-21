
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public NavigationTabController tabHandlers;
    // Stat Tab
    public Dictionary<STATUS_UI.Stat, StatSlotView> statHandlers = new Dictionary<STATUS_UI.Stat, StatSlotView>();

    // Inventory Tab
    public Dictionary<string, ItemSlotView> inventoryHandlers = new Dictionary<string, ItemSlotView>();

    // Character Tab
    public Dictionary<STATUS_UI.Character, CharacterSlotView> characterHandlers = new Dictionary<STATUS_UI.Character, CharacterSlotView>();

    // Dex Tab
    public Dictionary<string, DexSlotView> dexHandlers = new Dictionary<string, DexSlotView>();

    // Popup
    public Dictionary<POPUP.POPUP, IPopupBase> popupHandlers = new Dictionary<POPUP.POPUP, IPopupBase>();

    // Atlas

    public Dictionary<STATUS_UI.TAB, SpriteAtlas> uiAtlas = new Dictionary<STATUS_UI.TAB, SpriteAtlas>();
    public SpriteAtlas statAtlas { get; set; } = null;
    public SpriteAtlas itemAtlas { get; set; } = null;

    public Transform PopupPanel { get; set; } = null;
    public GameObject currentPopup { get; set; } = null;

    protected override void Awake()
    {
        PopupPanel = transform.Find("Canvas/PopupPanel");
    }
    void Start()
    {
    }

    public async void LoadDataUI()
    {
        await DataManager.Instance.InitData();
        await tabHandlers.InitTab();
    }

    public void ShowPopup(string description, POPUP.POPUP type)
    {
        if (popupHandlers.ContainsKey(type))
        {
            currentPopup = popupHandlers[type].ShowPopup(description);
        }
        else
        {
            Debug.Log(type + " 타입의 팝업은 아직 없습니다.");
        }
    }
}
