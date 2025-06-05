using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
public class NavigationTabController : MonoBehaviour
{
    [SerializeField]
    public Toggle[] toggles; // 4개의 Toggle들 연결

    [SerializeField]
    public GameObject[] tabContents; // 4개의 각 탭에 해당하는 패널 혹은 컨텐츠

    public STATUS_UI.TAB currentTabIndex { get; private set; } = STATUS_UI.TAB.None;

    private bool isFirstTabLoaded = false;
    // Addressables 주소 (Addressable Asset Settings에서 설정한 주소 이름)
    private string addressableKey_stat = "Assets/Addressables/Prefabs/UI/Tab_stat/stat.prefab";
    private string addressableKey_itemslot = "Assets/Addressables/Prefabs/UI/Tab_inventory/itemslot.prefab";
    private string addressableKey_popupItem = "Assets/Addressables/Prefabs/UI/Popup/popupItem.prefab";


    void Start()
    {
        // 각 Toggle에 onValueChanged 이벤트 연결
        for (int i = 0; i < toggles.Length; i++)
        {
            int index = i;  // 클로저 문제 방지
            toggles[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                    OnTabSelected(index);
            });
        }

        // 시작 시 첫 탭 활성화
        OnTabSelected(0);
    }

    void OnTabSelected(int index)
    {
        if ((int)currentTabIndex == index)
        {
            // 같은 탭 눌렀으면 아무 동작 안 함
            return;
        }

        if (Enum.IsDefined(typeof(STATUS_UI.TAB), index))
        {
            currentTabIndex = (STATUS_UI.TAB)index;
        }
        else
        {
            Debug.LogWarning("해당 값은 GameState에 없습니다.");
            return;
        }


        // 선택된 탭에 맞게 내용 활성화/비활성화 처리
        for (int i = 0; i < tabContents.Length; i++)
        {
            tabContents[i].SetActive(i == index);
        }
        

        if (index == 0 && !isFirstTabLoaded)
        {
            LoadStatPrefabToFirstTab();
            LoadItemSlotPrefabToFirstTab();
            isFirstTabLoaded = true;
        }
    }

    void LoadStatPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_stat).Completed += OnLoadPrefabsStat;
    }

    private void OnLoadPrefabsStat(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var kvp in DataManager.Instance.statData)
            {
                string statName = kvp.Key;
                string statValue = kvp.Value;

                if (Enum.TryParse(statName, true, out STATUS_UI.Stat statEnum))
                {
                    var parent = tabContents[(int)currentTabIndex];
                    var parentContent = parent.GetComponent<UIGameMenuPanelTab>().content;

                    GameObject statInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parentContent);
                    statInstance.name = "stat_" + statName;
                    statInstance.transform.localScale = Vector3.one;

                    var statView = statInstance.GetComponent<StatView>();

                    statView.setTitle(statName);
                    statView.setDiscription(statValue);
                    UIManager.Instance.statHandlers[statEnum] = statView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_stat}");
        }
    }


    void LoadItemSlotPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_itemslot).Completed += OnLoadPrefabsItemSlot;
        
        Addressables.LoadAssetAsync<GameObject>(addressableKey_popupItem).Completed += OnLoadPrefabsPopupItem;
        
    }

    private void OnLoadPrefabsItemSlot(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = tabContents[(int)STATUS_UI.TAB.Inventory];
            var parentContent = parent.GetComponent<UIGameMenuPanelTab>().content;
            int idx = 0;

            foreach (KeyValuePair<string, List<ItemData>> kvp in DataManager.Instance.itemData)
            {
                string materialKey = kvp.Key;               // 예: "천", "가죽", "철" 등
                List<ItemData> itemList = kvp.Value;        // 해당 Material에 속한 ItemData 리스트

                // Material 이름부터 출력
                Debug.Log($"=== Material: {materialKey} (총 {itemList.Count}개 아이템) ===");

                // 루프 2: 해당 Material 리스트 안의 각 ItemData를 순회
                foreach (ItemData item in itemList)
                {
                    // ItemData의 Part와 Discription을 출력
                    Debug.Log($"Part: {item.Part}, Discription: {item.Discription}");

                    GameObject itemInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parentContent);
                    itemInstance.name = "itemslot_" + idx;
                    itemInstance.transform.localScale = Vector3.one;

                    var itemSlotView = itemInstance.GetComponent<ItemSlotView>();
                    itemSlotView.descriptionText = item.Discription;

                    UIManager.Instance.inventoryHandlers["itemslot_" + idx] = itemSlotView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_itemslot}");
        }
    }

    private void OnLoadPrefabsPopupItem(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = UIManager.Instance.PopupPanel;

            GameObject instance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parent);
            instance.name = "popupItem";
            instance.transform.localScale = Vector3.one;

            var popup = instance.GetComponent<PopupBase>();
            UIManager.Instance.popupHandlers[POPUP.POPUP.item] = popup;
            instance.SetActive(false);
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_popupItem}");
        }
    }

    private void OnDestroy()
    {
        // 필요하면 Addressables.Release(handle) 처리 추가 가능
    }
}
