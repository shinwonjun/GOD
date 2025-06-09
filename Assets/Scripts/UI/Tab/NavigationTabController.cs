using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
public class NavigationTabController : MonoBehaviour
{
    [SerializeField] public Toggle[] toggles; // 4개의 Toggle들 연결

    [SerializeField] public GameObject[] tabContents; // 4개의 각 탭에 해당하는 패널 혹은 컨텐츠

    public STATUS_UI.TAB currentTabIndex { get; private set; } = STATUS_UI.TAB.None;

    private bool isFirstTabLoaded = false;
    // Addressables 주소 (Addressable Asset Settings에서 설정한 주소 이름)
    private string addressableKey_statslot = "Assets/Addressables/Prefabs/UI/Tab_stat/statslot.prefab";
    private string addressableKey_itemslot = "Assets/Addressables/Prefabs/UI/Tab_inventory/itemslot.prefab";
    private string addressableKey_characterslot = "Assets/Addressables/Prefabs/UI/Tab_Character/characterslot.prefab";
    private string addressableKey_popupItem = "Assets/Addressables/Prefabs/UI/Popup/popupItem.prefab";
    private string addressableKey_popupCharacter = "Assets/Addressables/Prefabs/UI/Popup/popupCharacter.prefab";


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
            LoadStatSlotPrefabToFirstTab();
            LoadItemSlotPrefabToFirstTab();
            LoadCharacterSlotPrefabToFirstTab();

            LoadPopupPrefabToFirstTab();
            isFirstTabLoaded = true;
        }
    }

    void LoadStatSlotPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_statslot).Completed += OnLoadPrefabsStatSlot;
    }

    private void OnLoadPrefabsStatSlot(AsyncOperationHandle<GameObject> handle)
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

                    GameObject statSlotInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parentContent);
                    statSlotInstance.name = "statslot_" + statName;
                    statSlotInstance.transform.localScale = Vector3.one;

                    var statSlotView = statSlotInstance.GetComponent<StatSlotView>();

                    statSlotView.setTitle(statName);
                    statSlotView.setDiscription(statValue);
                    UIManager.Instance.statHandlers[statEnum] = statSlotView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_statslot}");
        }
    }


    void LoadItemSlotPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_itemslot).Completed += OnLoadPrefabsItemSlot;
    }

    private void OnLoadPrefabsItemSlot(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = tabContents[(int)STATUS_UI.TAB.Inventory];
            var parentContent = parent.GetComponent<UIGameMenuPanelTab>().content;
            int idx = 0;

            foreach (KeyValuePair<string, List<DATA.ItemData>> kvp in DataManager.Instance.itemData)
            {
                string materialKey = kvp.Key;               // 예: "천", "가죽", "철" 등
                List<DATA.ItemData> itemList = kvp.Value;        // 해당 Material에 속한 ItemData 리스트

                // Material 이름부터 출력
                Debug.Log($"=== Material: {materialKey} (총 {itemList.Count}개 아이템) ===");

                // 루프 2: 해당 Material 리스트 안의 각 ItemData를 순회
                foreach (DATA.ItemData item in itemList)
                {
                    Debug.Log($"Part: {item.Part}, Discription: {item.Description}");

                    GameObject itemslotInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parentContent);
                    itemslotInstance.name = "itemslot_" + idx;
                    itemslotInstance.transform.localScale = Vector3.one;

                    var itemSlotView = itemslotInstance.GetComponent<ItemSlotView>();
                    itemSlotView.SetData(item);
                    UIManager.Instance.inventoryHandlers["itemslot_" + idx] = itemSlotView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_itemslot}");
        }
    }

    private void LoadCharacterSlotPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_characterslot).Completed += OnLoadPrefabsCharacterSlot;
    }

    private void OnLoadPrefabsCharacterSlot(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (STATUS_UI.Character characterEnum in Enum.GetValues(typeof(STATUS_UI.Character)))
            {
                string name = characterEnum.ToString();           // enum 값을 문자열로 변환
                var parent = tabContents[(int)STATUS_UI.TAB.Character];

                Transform slot = parent.transform.Find("Slot/" + name);
                if (slot != null)
                {
                    GameObject characterslotInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, slot);
                    characterslotInstance.name = "characterslot_" + name;
                    characterslotInstance.transform.localScale = Vector3.one;
                    characterslotInstance.transform.localPosition = Vector3.zero;

                    var characterSlotView = characterslotInstance.GetComponent<CharacterSlotView>();
                    characterSlotView.type = characterEnum;

                    var item = DataManager.Instance.characterData[name];
                    characterSlotView.SetData(item);

                    UIManager.Instance.characterHandlers[characterEnum] = characterSlotView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_characterslot}");
        }
    }

    private void LoadPopupPrefabToFirstTab()
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey_popupItem).Completed += OnLoadPrefabsPopupItem;
        Addressables.LoadAssetAsync<GameObject>(addressableKey_popupCharacter).Completed += OnLoadPrefabsPopupCharacter;
    }

    private void OnLoadPrefabsPopupItem(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = UIManager.Instance.PopupPanel;

            GameObject instance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parent);
            instance.name = "popupItem";
            instance.transform.localScale = Vector3.one;

            var popup = instance.GetComponent<IPopupBase>();
            UIManager.Instance.popupHandlers[POPUP.POPUP.item] = popup;
            instance.SetActive(false);
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_popupItem}");
        }
    }

    private void OnLoadPrefabsPopupCharacter(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = UIManager.Instance.PopupPanel;

            GameObject instance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parent);
            instance.name = "popupCharacter";
            instance.transform.localScale = Vector3.one;

            var popup = instance.GetComponent<IPopupBase>();
            UIManager.Instance.popupHandlers[POPUP.POPUP.character] = popup;
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
