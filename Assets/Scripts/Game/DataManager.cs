using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : Singleton<DataManager>
{

    private string addressableKey_stat_data = "Assets/Addressables/Data/stat_data.json";
    private string addressableKey_item_data = "Assets/Addressables/Data/item_data.json";
    private string addressableKey_equipslot_data = "Assets/Addressables/Data/equipslot_data.json";
    private string addressableKey_herolist_data = "Assets/Addressables/Data/herolist_data.json";
    private string addressableKey_hero_data = "Assets/Addressables/Data/hero_data.json";    
    private string addressableKey_hero_option_data = "Assets/Addressables/Data/hero_option_data.json";    
    private string addressableKey_stat_upgrade_table = "Assets/Addressables/Data/stat_upgrade_table.json";
    private string addressableKey_currency_table = "Assets/Addressables/Data/currency_table.json";
    private string addressableKey_enemy_table = "Assets/Addressables/Data/enemy_table.json";
    private string addressableKey_mycharacter_table = "Assets/Addressables/Data/mycharacter_table.json";

    public List<DATA.StatData> statData = new List<DATA.StatData>();
    public Dictionary<string, List<DATA.ItemData>> itemDataByMaterial = new Dictionary<string, List<DATA.ItemData>>();
    public Dictionary<int, DATA.ItemData> itemData = new Dictionary<int, DATA.ItemData>();
    public Dictionary<string, DATA.EquipslotData> characterData = new Dictionary<string, DATA.EquipslotData>();

    public List<DATA.HeroList> heroList = new List<DATA.HeroList>();
    public Dictionary<string, List<DATA.HeroData>> heroDataByHeroType = new Dictionary<string, List<DATA.HeroData>>();
    public Dictionary<int, DATA.HeroData> heroData = new Dictionary<int, DATA.HeroData>();
    public Dictionary<int, List<DATA.HeroOptionData>> heroOptionData = new Dictionary<int, List<DATA.HeroOptionData>>();
    public Dictionary<STATUS_UI.Stat, DATA.StatUpgradeData> statUpgradeTable = new Dictionary<STATUS_UI.Stat, DATA.StatUpgradeData>();

    public DATA.CurrencyTable currencyTable;
    public DATA.EnemyTable enemyTable;
    public DATA.MyCharacterTable myCharacterTable;

    public async Task InitData()
    {
        await LoadStatData();
        await LoadItemData();
        await LoadCharacterData();
        await LoadHeroList();
        await LoadHeroData();
        await LoadHeroOptionData();

        await LoadStatUpgradeTable();
        await LoadCurrencyTable();
        await LoadEnemyTable();
        await LoadMyCharacterTable();
    }

    private async Task LoadStatData()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_stat_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            statData = JsonConvert.DeserializeObject<List<DATA.StatData>>(asset.text);
        }
        else
        {
            Debug.LogError("DataManager: stat_data.json 로드 실패!");
        }
    }

    private async Task LoadItemData()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_item_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            List<DATA.ItemData> allItems = JsonConvert.DeserializeObject<List<DATA.ItemData>>(asset.text);
            itemData = new Dictionary<int, DATA.ItemData>();
            foreach (DATA.ItemData item in allItems)
            {
                int key = int.Parse(item.Id);
                itemData.Add(key, item);

                string material = item.Material;
                if (itemDataByMaterial.ContainsKey(material))
                {
                    itemDataByMaterial[material].Add(item);
                }
                else
                {
                    itemDataByMaterial[material] = new List<DATA.ItemData> { item };
                }
            }
        }
        else
        {
            Debug.LogError("DataManager: item_data.json 로드 실패!");
        }
    }

    private async Task LoadCharacterData()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_equipslot_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            List<DATA.EquipslotData> datas = JsonConvert.DeserializeObject<List<DATA.EquipslotData>>(asset.text);
            if (datas == null)
            {
                Debug.LogError("DataManager: scharacter_data.json 로드 실패!");
                return;
            }

            foreach (DATA.EquipslotData item in datas)
            {
                string key = item.Part;
                characterData[key] = item;
            }
        }
        else
        {
            Debug.LogError("DataManager: scharacter_data.json 로드 실패!");
        }
    }

    private async Task LoadHeroList()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_herolist_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            heroList = JsonConvert.DeserializeObject<List<DATA.HeroList>>(asset.text);
            if (heroList == null)
            {
                Debug.LogError("DataManager: hero_list.json 로드 실패1!");
                return;
            }
            // 디버그용 로그: 각 재질(Material)별 아이템 개수 확인
            foreach (var kvp in heroList)
            {
                Debug.LogFormat("HeroName = {0}, Type = {1}", kvp.Name, kvp.Type);
            }
        }
        else
        {
            Debug.LogError("DataManager: hero_list.json 로드 실패2!");
        }
    }

    private async Task LoadHeroData()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_hero_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            List<DATA.HeroData> allHeros = JsonConvert.DeserializeObject<List<DATA.HeroData>>(asset.text);
            if (allHeros == null)
            {
                Debug.LogError("DataManager: hero_data.json 로드 실패1!");
                return;
            }

            // 2) heroData 딕셔너리 초기화 및 그룹화
            heroDataByHeroType = new Dictionary<string, List<DATA.HeroData>>();
            foreach (DATA.HeroData item in allHeros)
            {
                heroData.Add(int.Parse(item.Id), item);

                string key = item.Type;
                if (heroDataByHeroType.ContainsKey(key))
                {
                    heroDataByHeroType[key].Add(item);
                }
                else
                {
                    heroDataByHeroType[key] = new List<DATA.HeroData> { item };
                }
            }

            foreach (var kvp in heroDataByHeroType)
            {
                Debug.LogFormat("DataManager: Material = {0}, Count = {1}", kvp.Key, kvp.Value.Count);
            }
        }
        else
        {
            Debug.LogError("DataManager: hero_data.json 로드 실패2!");
        }
    }

    private async Task LoadHeroOptionData()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_hero_option_data);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            List<DATA.HeroOptionData> allHeroOptionData = JsonConvert.DeserializeObject<List<DATA.HeroOptionData>>(asset.text);
            if (allHeroOptionData == null)
            {
                Debug.LogError("DataManager: hero_data_option.json 로드 실패1!");
                return;
            }

            // 2) heroData 딕셔너리 초기화 및 그룹화
            heroOptionData = new Dictionary<int, List<DATA.HeroOptionData>>();
            foreach (DATA.HeroOptionData item in allHeroOptionData)
            {
                int key = int.Parse(item.Type);
                if (heroOptionData.ContainsKey(key))
                {
                    heroOptionData[key].Add(item);
                }
                else
                {
                    heroOptionData[key] = new List<DATA.HeroOptionData> { item };
                }
            }
        }
        else
        {
            Debug.LogError("DataManager: hero_data_option.json 로드 실패2!");
        }
    }

    private async Task LoadStatUpgradeTable()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_stat_upgrade_table);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            DATA.StatUpgradeTable table = JsonConvert.DeserializeObject<DATA.StatUpgradeTable>(asset.text);

            if (table == null)
            {
                Debug.LogError("DataManager: stat_upgrade_table.json 로드 실패1!");
                return;
            }

            statUpgradeTable = new Dictionary<STATUS_UI.Stat, DATA.StatUpgradeData>
            {
                [STATUS_UI.Stat.Level] = table.LevelUpgrade,
                [STATUS_UI.Stat.AttackPower] = table.AttackUpgrade,
                [STATUS_UI.Stat.AttackSpeed] = table.SpeedUpgrade,
                [STATUS_UI.Stat.CriticalChance] = table.CritChanceUpgrade,
                [STATUS_UI.Stat.CriticalDamage] = table.CritDamageUpgrade
            };

            Debug.Log("DataManager: stat_upgrade_table.json 로딩 및 매핑 완료!");
        }
        else
        {
            Debug.LogError("DataManager: stat_upgrade_table.json 로드 실패2!");
        }
    }
    private async Task LoadCurrencyTable()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_currency_table);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            DATA.CurrencyTable table = JsonConvert.DeserializeObject<DATA.CurrencyTable>(asset.text);

            if (table == null)
            {
                Debug.LogError("DataManager: currency_table.json 로드 실패1!");
                return;
            }

            currencyTable = new DATA.CurrencyTable();
            currencyTable.ResourcePerInterval = table.ResourcePerInterval;
            currencyTable.GainInterval = table.GainInterval;

            Debug.Log("DataManager: currency_table.json 로딩 및 매핑 완료!");
        }
        else
        {
            Debug.LogError("DataManager: currency_table.json 로드 실패2!");
        }
    }
    private async Task LoadEnemyTable()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_enemy_table);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            DATA.EnemyTable table = JsonConvert.DeserializeObject<DATA.EnemyTable>(asset.text);

            if (table == null)
            {
                Debug.LogError("DataManager: enemy_table.json 로드 실패1!");
                return;
            }

            enemyTable = new DATA.EnemyTable();
            enemyTable.DefaultLevel = table.DefaultLevel;
            enemyTable.MaxLevel = table.MaxLevel;
            enemyTable.DefaultHP = table.DefaultHP;
            enemyTable.DefaultDef = table.DefaultDef;
            enemyTable.ConstantDef = table.ConstantDef;
            enemyTable.NaturalConstant = table.NaturalConstant;
            enemyTable.RateHP = table.RateHP;
            enemyTable.DefaultReward = table.DefaultReward;
            enemyTable.ConstantReward = table.ConstantReward;
            enemyTable.MaxKill = table.MaxKill;

            Debug.Log("DataManager: enemy_table.json 로딩 및 매핑 완료!");
        }
        else
        {
            Debug.LogError("DataManager: enemy_table.json 로드 실패2!");
        }
    }
    private async Task LoadMyCharacterTable()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey_mycharacter_table);
        await handle.Task; // 비동기적으로 완료를 기다림

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //Addressables에서 로드된 데이터 파싱
            TextAsset asset = handle.Result;
            DATA.MyCharacterTable table = JsonConvert.DeserializeObject<DATA.MyCharacterTable>(asset.text);

            if (table == null)
            {
                Debug.LogError("DataManager: mycharacter_table.json 로드 실패1!");
                return;
            }

            myCharacterTable = new DATA.MyCharacterTable();
            myCharacterTable.DefaultLevel = table.DefaultLevel;
            myCharacterTable.MaxLevel = table.MaxLevel;
            myCharacterTable.DefaultAttackPower = table.DefaultAttackPower;
            myCharacterTable.DefaultAttackSpeed = table.DefaultAttackSpeed;
            myCharacterTable.ConstantAttack = table.ConstantAttack;
            myCharacterTable.DefaultCriticalChance = table.DefaultCriticalChance;
            myCharacterTable.DefaultCriticalDamage = table.DefaultCriticalDamage;

            Debug.Log("DataManager: mycharacter_table.json 로딩 및 매핑 완료!");
        }
        else
        {
            Debug.LogError("DataManager: mycharacter_table.json 로드 실패2!");
        }
    }

    /////////////////////////////////////////////////////////

    public DATA.HeroData GetHeroData(int id)
    {
        if (heroData.ContainsKey(id))
        {
            return heroData[id];
        }

        return null;
    }
}
