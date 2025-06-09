using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatSlotView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TextMeshProUGUI textDiscription;
    [SerializeField] private Button upgradeButton;

    [HideInInspector] public int level { get; private set; } = 1;



    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

        setLevel(level.ToString());
    }

    private void OnUpgradeButtonClicked()
    {
        Debug.Log("업그레이드 버튼 클릭됨!");
        // 여기서 원하는 로직 처리
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지 위해 리스너 해제 권장
        upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
    }

    public void setTitle(string _title)
    {
        textTitle.text = _title;
    }

    public void setLevel(string _level)
    {
        textLevel.text = _level;
    }

    public void setDiscription(string _discription)
    {
        textDiscription.text = _discription;
    }
}
