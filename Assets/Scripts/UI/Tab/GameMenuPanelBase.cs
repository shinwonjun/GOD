using UnityEngine;

public class GameMenuPanelTabBase : MonoBehaviour
{
    [SerializeField] public STATUS_UI.TAB tab = STATUS_UI.TAB.None;

    [SerializeField] public Transform[] content;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
