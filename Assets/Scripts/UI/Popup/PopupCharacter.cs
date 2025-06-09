using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharacter : MonoBehaviour, IPopupBase
{
    [SerializeField] public Button closeButton;
    [SerializeField] public Button infoButton;
    [SerializeField] public Button equipButton;     // 장착, 장착해제 두가지 기능

    public void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
    }


    public GameObject ShowPopup(string description)
    {
        bool ac = description == "" ? false : true;
        infoButton.gameObject.SetActive(ac);
        equipButton.gameObject.SetActive(true);

        gameObject.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);

        return gameObject;
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
