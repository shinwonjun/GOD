using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;

public class StatSlotView : SlotViewBase<DATA.StatData>
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TextMeshProUGUI textDescription;
    [SerializeField] private TextMeshProUGUI textExpense;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private Button upgradeButton;

    [HideInInspector] public int level { get; private set; } = 1;

    private STATUS_UI.Stat type;
    private DATA.StatData data;
    public override DATA.StatData GetData() => data;
    public override void SetData(DATA.StatData newData)
    {
        tabType = STATUS_UI.TAB.Stats;
        popupType = POPUP.POPUP.stat;
        data = newData;

        level = GameMyData.Instance.UserData.statLevelsByIndex[(int)type];
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
        //UIManager.Instance.ShowPopup(popupType, data);
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

        NetworkManager.SendRequest_Test("UpgradeStat", packetpayload);
    }

    private void OnDestroy()
    {
        upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
    }

    private void setTitle(string _title)
    {
        textTitle.text = _title;
    }

    private void setLevel(string _level)
    {
        textLevel.text = _level;
    }

    private void setDiscription(string _description)
    {
        textDescription.text = _description;
    }

    private void setButtonName(string _name)
    {
        textExpense.text = _name;
    }

    private void setCost(BigInteger cost)
    {
        textCost.text = cost.ToString();
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
