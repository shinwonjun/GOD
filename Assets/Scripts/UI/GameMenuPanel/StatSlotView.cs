using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;

public class StatSlotView : SlotViewBase<DATA.StatData>
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textTitleComponent;
    [SerializeField] private TextMeshProUGUI textLevelComponent;
    [SerializeField] private TextMeshProUGUI textDiscriptionComponent;
    [SerializeField] private TextMeshProUGUI textExpenseComponent;
    [SerializeField] private TextMeshProUGUI textCostComponent;
    [SerializeField] private Button upgradeButton;


    [HideInInspector] public TextMeshProUGUI textTitle;
    [HideInInspector] public TextMeshProUGUI textLevel;
    [HideInInspector] public TextMeshProUGUI textDiscription;
    [HideInInspector] public TextMeshProUGUI textExpense;
    [HideInInspector] public TextMeshProUGUI textCost;

    [HideInInspector] public int level { get; private set; } = 1;

    private STATUS_UI.Stat type;
    private DATA.StatData data;
    public override DATA.StatData GetData() => data;
    public override void SetData(DATA.StatData newData)
    {
        tabType = STATUS_UI.TAB.Stats;
        popupType = POPUP.POPUP.stat;
        data = newData;

        level = GameMyData.Instance.dicStatLevel[type];
        setLevel(level.ToString());

        setTitle(data.Name);
        setDiscription(data.Description);
        setButtonName("UPGRADE");
        if (DataManager.Instance.statUpgradeTable.TryGetValue(type, out var upgradeData))
        {
            int cost = Mathf.FloorToInt(upgradeData.GetCostAtLevel(level + 1));
            setCost(cost);
        }

        setImage(data.Sprite);
    }

    public void SetType(STATUS_UI.Stat _type)
    {
        type = _type;
    }

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
    }
    public override void ShowPopup()
    {
        UIManager.Instance.ShowPopup(data.Description, popupType);
    }

    private void OnUpgradeButtonClicked()
    {
        Debug.Log($"{type} 업그레이드 버튼 클릭됨!");
        string packetpayload = "";
        if (type == STATUS_UI.Stat.Level)
            packetpayload = "LevelUpgrade";
        else if (type == STATUS_UI.Stat.AttackPower)
            packetpayload = "AttackPower";
        else if (type == STATUS_UI.Stat.AttackSpeed)
            packetpayload = "AttackSpeed";
        else if (type == STATUS_UI.Stat.CriticalChance)
            packetpayload = "CriticalChance";
        else if (type == STATUS_UI.Stat.CriticalDamage)
            packetpayload = "CriticalDamage";
        //NetworkManager.SendRequest_Test(packetpayload, "");

        NetworkManager.SendRequest_Test("StatUpgrade", packetpayload);
    }

    private void OnDestroy()
    {
        upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
    }

    private void setTitle(string _title)
    {
        GameObject textObj = new GameObject("textName");
        textObj.transform.SetParent(transform);
        textTitle = textObj.AddComponent<TextMeshProUGUI>();
        textTitle.text = _title;
        textTitle.fontSize = 36;
        textTitle.color = Color.white;
        textTitle.alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Center;

        RectTransform rect = textTitleComponent.GetComponent<RectTransform>();
        RectTransform newRect = textObj.GetComponent<RectTransform>();

        Utils.CopyRectTransform(rect, newRect);

        Destroy(textTitleComponent.gameObject);
    }

    private void setLevel(string _level)
    {
        GameObject textObj = new GameObject("textLv");
        textObj.transform.SetParent(transform);
        textLevel = textObj.AddComponent<TextMeshProUGUI>();
        textLevel.text = _level;
        textLevel.fontSize = 36;
        textLevel.color = Color.white;
        textLevel.alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Center;

        RectTransform rect = textLevelComponent.GetComponent<RectTransform>();
        RectTransform newRect = textObj.GetComponent<RectTransform>();

        Utils.CopyRectTransform(rect, newRect);

        Destroy(textLevelComponent.gameObject);
    }

    private void setDiscription(string _discription)
    {
        GameObject textObj = new GameObject("textLv");
        textObj.transform.SetParent(transform);
        textDiscription = textObj.AddComponent<TextMeshProUGUI>();
        textDiscription.text = _discription;
        textDiscription.fontSize = 32;
        textDiscription.color = Color.white;
        textDiscription.alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Center;

        RectTransform rect = textDiscriptionComponent.GetComponent<RectTransform>();
        RectTransform newRect = textObj.GetComponent<RectTransform>();

        Utils.CopyRectTransform(rect, newRect);

        Destroy(textDiscriptionComponent.gameObject);
    }

    private void setButtonName(string _name)
    {
        GameObject textObj = new GameObject("textExpense");
        textObj.transform.SetParent(upgradeButton.transform);
        textExpense = textObj.AddComponent<TextMeshProUGUI>();
        textExpense.text = _name;
        textExpense.fontSize = 24;
        textExpense.color = Color.black;
        textExpense.alignment = TextAlignmentOptions.Center;

        RectTransform rect = textExpenseComponent.GetComponent<RectTransform>();
        RectTransform newRect = textObj.GetComponent<RectTransform>();

        Utils.CopyRectTransform(rect, newRect);

        Destroy(textExpenseComponent.gameObject);
    }

    private void setCost(BigInteger cost)
    {
        GameObject textObj = new GameObject("textCost");
        textObj.transform.SetParent(upgradeButton.transform);
        textCost = textObj.AddComponent<TextMeshProUGUI>();
        textCost.text = cost.ToString();
        textCost.fontSize = 24;
        textCost.color = Color.black;
        textCost.alignment = TextAlignmentOptions.Center;

        RectTransform rect = textCostComponent.GetComponent<RectTransform>();
        RectTransform newRect = textObj.GetComponent<RectTransform>();

        Utils.CopyRectTransform(rect, newRect);

        Destroy(textCostComponent.gameObject);
    }

    private void setImage(string _sprite)
    {
        image.sprite = UIManager.Instance.uiAtlas[tabType].GetSprite(_sprite);
    }

    private void UpdateCost()
    {
        if (DataManager.Instance.statUpgradeTable.TryGetValue(type, out var upgradeData))
        {
            int cost = Mathf.FloorToInt(upgradeData.GetCostAtLevel(level + 1));
            textCost.text = cost.ToString();
        }
        else
        {
            Debug.LogWarning($"StatUpgradeData not found for stat: {type}");
            textCost.text = "0";
        }
    }
    public void IncreaseLevel(int _level)
    {
        level = _level;
        textLevel.text = _level.ToString();
        UpdateCost();
    }
}
