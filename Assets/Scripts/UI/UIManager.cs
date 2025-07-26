
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[System.Serializable]
public class CurrencyUI
{
    public Image coin;
    public TextMeshProUGUI coinText;
    public Image diamond;
    public TextMeshProUGUI diamondText;
}

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] public NavigationTabController tabHandlers;

    [Header("💰 Currency GameObject")] public CurrencyUI currencyUI;


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

    // Font
    //public TMP_FontAsset defaultFontAssets;

    [HideInInspector] public SpriteAtlas systemuiAtlas;
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

    public async Task LoadDataUI()
    {
        await DataManager.Instance.InitData();
        await tabHandlers.InitTab();            // 여기서 아틀라스 로드
        await InitUI();
        await Task.Delay(0);
    }

    public async Task InitUI()
    {
        {
            // currency ui load(재화)
            currencyUI.coin.sprite = systemuiAtlas.GetSprite("coin");
            currencyUI.diamond.sprite = systemuiAtlas.GetSprite("diamond");

            currencyUI.coinText.text = GameMyData.Instance.coin.ToString();
            currencyUI.diamondText.text = GameMyData.Instance.diamond.ToString();
        }
        await Task.Delay(0);
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
