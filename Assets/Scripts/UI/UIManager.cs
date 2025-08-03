
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
    public Dictionary<STATUS_UI.Character, PartsSlotView> characterHandlers = new Dictionary<STATUS_UI.Character, PartsSlotView>();

    // Dex Tab
    public Dictionary<string, DexSlotView> dexHandlers = new Dictionary<string, DexSlotView>();

    // Popup
    public Dictionary<POPUP.POPUP, PopupBase> popupHandlers = new Dictionary<POPUP.POPUP, PopupBase>();

    // Atlas
    public Dictionary<STATUS_UI.TAB, SpriteAtlas> uiAtlas = new Dictionary<STATUS_UI.TAB, SpriteAtlas>();

    [HideInInspector] public SpriteAtlas systemuiAtlas;
    public SpriteAtlas statAtlas { get; set; } = null;
    public SpriteAtlas itemAtlas { get; set; } = null;

    public Transform PopupPanel { get; set; } = null;
    public PopupBase currentPopup { get; set; } = null;

    protected override void Awake()
    {
        PopupPanel = transform.Find("Canvas/PopupPanel");
    }
    void Start()
    {
        // 앱 시작 시 초기값
        currencyUI.coinText.text = GameMyData.Instance.Coin.ToString("N0");
        currencyUI.diamondText.text = GameMyData.Instance.Diamond.ToString("N0");

        GameMyData.Instance.OnCoinChanged += coin =>
        {
            currencyUI.coinText.text = coin.ToString("N0"); // 천단위 쉼표 포함
        };
        GameMyData.Instance.OnDiamondChanged += diamond =>
        {
            currencyUI.diamondText.text = diamond.ToString("N0"); // 천단위 쉼표 포함
        };
    }

    public async Task LoadDataUI()
    {
        await DataManager.Instance.InitData();
        await tabHandlers.InitTab();            // 여기서 아틀라스 로드
        await InitCurrencyUI();
        await Task.Delay(0);
    }

    public async Task InitCurrencyUI()
    {
        {
            // currency ui load(재화)
            currencyUI.coin.sprite = systemuiAtlas.GetSprite("coin");
            currencyUI.diamond.sprite = systemuiAtlas.GetSprite("diamond");
        }
        await Task.Delay(0);
    }

    public void ShowPopup(POPUP.POPUP type, DATA.ItemData itemData, bool equiped)
    {
        if (popupHandlers.ContainsKey(type))
        {
            if (type == POPUP.POPUP.inventory)
                (popupHandlers[type] as PopupItem).SetItem(itemData);
            else if (type == POPUP.POPUP.character)
                (popupHandlers[type] as PopupCharacter).SetItem(itemData);

            currentPopup = popupHandlers[type].ShowPopup(itemData.Description, equiped);
        }
        else
        {
            Debug.Log(type + " 타입의 팝업은 아직 없습니다.");
        }
    }

    public void ShowPopup(POPUP.POPUP type, DATA.StatData statData)
    {
        if (popupHandlers.ContainsKey(type))
        {
            currentPopup = popupHandlers[type].ShowPopup(statData.Description, true);
        }
        else
        {
            Debug.Log(type + " 타입의 팝업은 아직 없습니다.");
        }
    }

    public void ShowPopup(POPUP.POPUP type, DATA.HeroData heroData, bool equiped)
    {
        if (popupHandlers.ContainsKey(type))
        {
            (popupHandlers[type] as PopupDex).SetItem(heroData);
            currentPopup = popupHandlers[type].ShowPopup(heroData.Description, equiped);
        }
        else
        {
            Debug.Log(type + " 타입의 팝업은 아직 없습니다.");
        }
    }

    public void refreshInventory(int prevId, int equipId)
    {
        if (prevId != -1)
        {
            foreach (var pair in inventoryHandlers)
            {
                ItemSlotView slotView = pair.Value;

                if (slotView.GetData() != null && slotView.GetData().Id == prevId.ToString())
                {
                    slotView.Refresh(prevId);
                    break; // 해당 슬롯만 갱신하면 되므로 루프 종료
                }
            }
        }

        if (equipId != -1)
        {
            foreach (var pair in inventoryHandlers)
            {
                ItemSlotView slotView = pair.Value;

                if (slotView.GetData() != null && slotView.GetData().Id == equipId.ToString())
                {
                    slotView.Refresh(equipId);
                    break; // 해당 슬롯만 갱신하면 되므로 루프 종료
                }
            }
        }
    }
}
