using UnityEngine;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour
{
    public string descriptionText { get; set; } = "";
    private Button _button;
    private void Awake()
    {
        // Button 컴포넌트 가져오기
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("[ItemUI] Button 컴포넌트가 없습니다. 이 스크립트가 붙은 오브젝트에는 반드시 Button이 필요합니다.");
            return;
        }

        // 클릭 이벤트 리스너 등록
        _button.onClick.AddListener(OnClickItem);
    }

    private void OnDestroy()
    {
        // 혹시 제거될 때 이벤트 클린업
        if (_button != null)
            _button.onClick.RemoveListener(OnClickItem);
    }

    /// <summary>
    /// 아이템이 클릭되었을 때 호출됩니다.
    /// UIManager descriptionText를 넘겨 팝업을 띄워 달라고 요청합니다.
    /// </summary>
    private void OnClickItem()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPopup(descriptionText);
        }
    }
}
