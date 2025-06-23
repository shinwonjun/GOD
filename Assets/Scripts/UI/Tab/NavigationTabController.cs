using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using UnityEngine.U2D;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
public class NavigationTabController : MonoBehaviour
{
    [SerializeField] public Toggle[] toggles; // 5개의 Toggle들 연결

    [SerializeField] public GameObject[] tabContents; // 5개의 각 탭에 해당하는 패널 혹은 컨텐츠

    public STATUS_UI.TAB currentTabIndex { get; private set; } = STATUS_UI.TAB.None;

    private bool isFirstTabLoaded = false;
    // Addressables 주소 (Addressable Asset Settings에서 설정한 주소 이름)
    private string addressableKey_statslot = "Assets/Addressables/Prefabs/UI/Tab_stat/statslot.prefab";
    private string addressableKey_itemslot = "Assets/Addressables/Prefabs/UI/Tab_inventory/itemslot.prefab";
    private string addressableKey_characterslot = "Assets/Addressables/Prefabs/UI/Tab_Character/characterslot.prefab";
    private string addressableKey_dexsList = "Assets/Addressables/Prefabs/UI/Tab_Dex/dexList.prefab";
    private string addressableKey_dexslot = "Assets/Addressables/Prefabs/UI/Tab_Dex/dexSlot.prefab";
    private string addressableKey_popupItem = "Assets/Addressables/Prefabs/UI/Popup/popupItem.prefab";
    private string addressableKey_popupCharacter = "Assets/Addressables/Prefabs/UI/Popup/popupCharacter.prefab";



    private const string addressableKey_statAtlas = "statAtlas";
    private const string addressableKey_itemAtlas = "itemAtlas";
    private const string addressableKey_heroAtlas = "heroAtlas";


    void Start()
    {
    }

    public async Task InitTab()
    {
        await LoadUIAtlases();
        await LoadStatSlotPrefabToFirstTab();
        await LoadItemSlotPrefabToFirstTab();
        await LoadCharacterSlotPrefabToFirstTab();
        await LoadDexSlotPrefabToFirstTab();
        await LoadPopupPrefabToFirstTab();


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
            // LoadStatSlotPrefabToFirstTab();
            // LoadItemSlotPrefabToFirstTab();
            // LoadCharacterSlotPrefabToFirstTab();
            // LoadDexSlotPrefabToFirstTab();
            // LoadPopupPrefabToFirstTab();
            isFirstTabLoaded = true;
        }
    }

    async Task LoadUIAtlases()
    {
        {
            var handle = Addressables.LoadAssetAsync<SpriteAtlas>(addressableKey_itemAtlas);

            if (!handle.IsValid())
            {
                Debug.LogError("❌ InvalidHandle: Address may be wrong or not built!");
                return;
            }

            SpriteAtlas atlas = await handle.Task;

            if (atlas == null)
            {
                Debug.LogError("❌ Atlas is null even after valid handle!");
            }
            else
            {
                UIManager.Instance.uiAtlas[STATUS_UI.TAB.Inventory] = atlas;
                Debug.Log("✅ SpriteAtlas loaded: " + atlas.name);
            }
        }
        {
            var handle = Addressables.LoadAssetAsync<SpriteAtlas>(addressableKey_statAtlas);

            if (!handle.IsValid())
            {
                Debug.LogError("❌ InvalidHandle: Address may be wrong or not built!");
                return;
            }

            SpriteAtlas atlas = await handle.Task;

            if (atlas == null)
            {
                Debug.LogError("❌ Atlas is null even after valid handle!");
            }
            else
            {
                UIManager.Instance.uiAtlas[STATUS_UI.TAB.Stats] = atlas;
                Debug.Log("✅ SpriteAtlas loaded: " + atlas.name);
            }
        }
        {
            var handle = Addressables.LoadAssetAsync<SpriteAtlas>(addressableKey_heroAtlas);

            if (!handle.IsValid())
            {
                Debug.LogError("❌ InvalidHandle: Address may be wrong or not built!");
                return;
            }

            SpriteAtlas atlas = await handle.Task;

            if (atlas == null)
            {
                Debug.LogError("❌ Atlas is null even after valid handle!");
            }
            else
            {
                UIManager.Instance.uiAtlas[STATUS_UI.TAB.Dex] = atlas;
                Debug.Log("✅ SpriteAtlas loaded: " + atlas.name);
            }
        }
    }

    public async Task LoadStatSlotPrefabToFirstTab()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey_statslot);
        await handle.Task;
        OnLoadPrefabsStatSlot(handle);
    }

    private void OnLoadPrefabsStatSlot(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = tabContents[(int)STATUS_UI.TAB.Stats];
            var parentContent = parent.GetComponent<GameMenuPanelTabBase>().content[0];
            foreach (DATA.StatData item in DataManager.Instance.statData)
            {
                Debug.Log($"Name: {item.Name}, Discription: {item.Description}");
                if (Enum.TryParse(item.Name, true, out STATUS_UI.Stat statEnum))
                {

                    GameObject statSlotInstance = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parentContent);
                    statSlotInstance.name = "statslot_" + item.Name;
                    statSlotInstance.transform.localScale = Vector3.one;

                    var statSlotView = statSlotInstance.GetComponent<StatSlotView>();
                    statSlotView.SetData(item);
                    UIManager.Instance.statHandlers[statEnum] = statSlotView;
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_statslot}");
        }
    }


    async Task LoadItemSlotPrefabToFirstTab()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey_itemslot);
        await handle.Task;
        OnLoadPrefabsItemSlot(handle);
    }

    private void OnLoadPrefabsItemSlot(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var parent = tabContents[(int)STATUS_UI.TAB.Inventory];
            var parentContent = parent.GetComponent<GameMenuPanelTabBase>().content[0];
            int idx = 0;


            foreach (KeyValuePair<string, List<DATA.ItemData>> kvp in DataManager.Instance.itemDataByMaterial)
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

    private async Task LoadCharacterSlotPrefabToFirstTab()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey_characterslot);
        await handle.Task;
        OnLoadPrefabsCharacterSlot(handle);
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

    private async Task LoadDexSlotPrefabToFirstTab()
    {
        try
        {
            var dexListPrefab = await Addressables.LoadAssetAsync<GameObject>(addressableKey_dexsList).Task;
            var dexSlotPrefab = await Addressables.LoadAssetAsync<GameObject>(addressableKey_dexslot).Task;

            var dexListInstances = new Dictionary<string, GameObject>();
            var heroList = DataManager.Instance.heroList;
            var parent = tabContents[(int)STATUS_UI.TAB.Dex];

            Transform[] parentContents = new Transform[]
            {
                parent.GetComponent<GameMenuPanelTabBase>().content[0], // GOD
                parent.GetComponent<GameMenuPanelTabBase>().content[1]  // DEMON
            };


            var heroDataMap = new List<DATA.HeroData>[]
            {
                DataManager.Instance.heroData[GAME.HeroType.GOD.ToString()],
                DataManager.Instance.heroData[GAME.HeroType.DEMON.ToString()]
            };

            // 1. DexList 생성
            foreach (GAME.HeroType heroType in Enum.GetValues(typeof(GAME.HeroType)))
            {
                if (heroType == GAME.HeroType.None)
                    continue;

                int index = (int)heroType;

                // 1. heroData 가져오기
                var heroDataList = DataManager.Instance.heroData[heroType.ToString()];

                // 2. parentContent 매핑
                var parentContent = parentContents[index];

                // 3. heroList에서 해당 type에 해당하는 Hero만 필터링
                var matchingHeroes = heroList.Where(h => h.Type == heroType.ToString());

                foreach (var hero in matchingHeroes)
                {
                    // DexList 생성
                    GameObject dexListInstance = Instantiate(dexListPrefab, parentContent);
                    dexListInstance.name = hero.Name;
                    dexListInstance.transform.localScale = Vector3.one;

                    // Content 경로 찾기
                    var content = dexListInstance.transform.Find("ScrollView/Content");
                    if (content == null)
                    {
                        Debug.LogError($"❌ '{hero.Name}' 하위에 'ScrollView/Content' 오브젝트 없음");
                        continue;
                    }

                    dexListInstances[hero.Name] = content.gameObject;

                    // DexSlot 생성
                    var filtered = heroDataList.Where(h => h.Name.Contains(hero.Name)).ToList();
                    foreach (var item in filtered)
                    {
                        GameObject dexSlotInstance = Instantiate(dexSlotPrefab, content);
                        dexSlotInstance.name = $"dexslot_{item.Id}";
                        dexSlotInstance.transform.localScale = Vector3.one;

                        var dexSlotView = dexSlotInstance.GetComponent<DexSlotView>();
                        dexSlotView?.SetData(item);
                        UIManager.Instance.dexHandlers[dexSlotInstance.name] = dexSlotView;
                    }
                }
            }

            Debug.Log("✅ Dex UI 생성 완료");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Dex UI 로딩 중 오류 발생: {ex.Message}");
        }
    }

    private async Task LoadPopupPrefabToFirstTab()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey_popupItem); ;
        await handle.Task;
        OnLoadPrefabsPopupItem(handle);

        var handle2 = Addressables.LoadAssetAsync<GameObject>(addressableKey_popupCharacter);
        await handle2.Task;
        OnLoadPrefabsPopupCharacter(handle);
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
