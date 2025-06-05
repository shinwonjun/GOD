using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{

    public TextMeshProUGUI uiText;              // UnityEngine.UI.Text를 쓸 때
    public Button closeButton;

    public void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
    }

    /// <summary>
    /// 팝업을 화면에 띄우고, 내용(description)을 설정한다.
    /// </summary>
    public GameObject ShowPopup(string description)
    {
        // 텍스트 설정
        uiText.text = description;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        return gameObject;
    }
    /// <summary>
    /// 팝업을 닫는다.
    /// </summary>
    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
