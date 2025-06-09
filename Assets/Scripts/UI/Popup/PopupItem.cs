using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupItem : MonoBehaviour, IPopupBase
{
    [SerializeField] public TextMeshProUGUI uiText;              // UnityEngine.UI.Text를 쓸 때
    [SerializeField] public Button closeButton;

    public void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
    }

    public GameObject ShowPopup(string description)
    {
        // 텍스트 설정
        uiText.text = description;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        return gameObject;
    }
    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}