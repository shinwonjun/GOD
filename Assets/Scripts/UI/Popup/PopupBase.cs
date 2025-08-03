using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour, IPopupBase
{
    [SerializeField] public TextMeshProUGUI uiInfoText;
    [SerializeField] public TextMeshProUGUI uiEquipBtnText;
    [SerializeField] public Button equipButton;
    [SerializeField] public Button closeButton;

    protected bool equiped = false;

    public void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
        if (equipButton != null)
            equipButton.onClick.AddListener(Equip);
    }

    public virtual PopupBase ShowPopup(string description, bool equiped)
    {
        gameObject.SetActive(true);
        uiInfoText.text = description;
        this.equiped = equiped;
        uiEquipBtnText.text = equiped ? "UNEQUIP" : "EQUIP";
        return this;
    }

    public virtual void Equip()
    {
        if (equiped)
        {
            Unequip();
            return;
        }
        else
        {
            Debug.Log("Equip Clicked");
        }
    }

    public virtual void Unequip()
    {
        Debug.Log("Unequip Clicked");
        // 구현 내용
    }

    public virtual void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
