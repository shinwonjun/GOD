using UnityEngine;
using UnityEngine.UI;

public class UIGameMenuPanelTab : MonoBehaviour
{
    [SerializeField]
    public STATUS_UI.TAB tab = STATUS_UI.TAB.None;

    [HideInInspector]
    public Transform content { get; private set; } = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        content = transform.Find("ScrollView/Content");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
