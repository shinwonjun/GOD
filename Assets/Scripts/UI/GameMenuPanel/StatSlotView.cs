using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatSlotView : SlotViewBase<DATA.StatData>
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TextMeshProUGUI textDiscription;
    [SerializeField] private Button upgradeButton;

    [HideInInspector] public int level { get; private set; } = 1;

    private DATA.StatData data;
    public override DATA.StatData GetData() => data;
    public override void SetData(DATA.StatData newData)
    {
        tabType = STATUS_UI.TAB.Stats;
        popupType = POPUP.POPUP.stat;
        data = newData;

        setTitle(data.Name);
        setLevel(level.ToString());
        setDescription(data.Description);
        setImage(data.Sprite);
    }


    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        setLevel(level.ToString());
    }
    public override void ShowPopup()
    {
        UIManager.Instance.ShowPopup(data.Description, popupType);
    }

    private void OnUpgradeButtonClicked()
    {
        Debug.Log("업그레이드 버튼 클릭됨!");
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

    private void setDescription(string _discription)
    {
        textDiscription.text = _discription;
    }

    private void setImage(string _sprite)
    {
        image.sprite = UIManager.Instance.uiAtlas[tabType].GetSprite(_sprite);
    }
}
