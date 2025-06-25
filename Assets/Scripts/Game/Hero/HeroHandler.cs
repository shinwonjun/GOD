using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HeroHandler : MonoBehaviour
{
    const int MAX_HERO = 3;
    const int MAX_ENEMY = 1;

    [SerializeField] GameObject[] heroPos;
    [SerializeField] GameObject[] enemyPos;
    [HideInInspector] public HeroBase[] heros;
    [HideInInspector] public HeroBase[] enemys = new HeroBase[MAX_ENEMY];


    private string addressableKey_hero = "Assets/Addressables/Prefabs/Game/Hero/hero.prefab";
    private string addressableKey_enemy = "Assets/Addressables/Prefabs/Game/Hero/enemy.prefab";

    void Start()
    {

    }
    public async Task LoadHeroEnemy()
    {
        var handle_hero = Addressables.LoadAssetAsync<GameObject>(addressableKey_hero);
        await handle_hero.Task;
        OnLoadPrefabsHero(handle_hero);

        var handle_enemy = Addressables.LoadAssetAsync<GameObject>(addressableKey_enemy);
        await handle_enemy.Task;
        OnLoadPrefabsEnemy(handle_enemy);
    }
    private void OnLoadPrefabsHero(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            heros = new HeroBase[MAX_HERO];
            for (int i = 0; i < MAX_HERO; ++i)
            {
                var parent = heroPos[i];
                GameObject goHero = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parent.transform);
                RectTransform rt = goHero.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                goHero.name = "hero_" + i;
                goHero.transform.localScale = Vector3.one;

                var hero = goHero.GetComponent<Hero>();

                int id = GameMyData.Instance.listEquipHeros[i];
                bool checkEquip = false;
                if (DataManager.Instance.heroData.ContainsKey(id))
                {
                    var data = DataManager.Instance.GetHeroData(id);
                    if (data != null)
                    {
                        var type = (GAME.HeroType)Enum.Parse(typeof(GAME.HeroType), data.Type);
                        hero.init(type, i, data.Sprite);
                        checkEquip = true;
                    }
                }
                if (checkEquip == false)
                {
                    hero.init(GAME.HeroType.None, -1, "");
                }

                heros[i] = hero;
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_hero}");
        }
    }

    private void OnLoadPrefabsEnemy(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            enemys = new HeroBase[MAX_ENEMY];
            for (int i = 0; i < MAX_ENEMY; ++i)
            {
                var parent = enemyPos[i];
                GameObject goEnemy = Instantiate(handle.Result, Vector3.zero, Quaternion.identity, parent.transform);
                RectTransform rt = goEnemy.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                goEnemy.name = "enemy_" + i;
                goEnemy.transform.localScale = Vector3.one;

                var enemy = goEnemy.GetComponent<Enemy>();

                int id = GameMyData.Instance.listEquipHeros[i];
                bool checkEquip = false;
                if (DataManager.Instance.heroData.ContainsKey(id))
                {
                    var data = DataManager.Instance.GetHeroData(id);
                    if (data != null)
                    {
                        var type = (GAME.HeroType)Enum.Parse(typeof(GAME.HeroType), data.Type);
                        enemy.init(type, i, data.Sprite);
                        checkEquip = true;
                    }
                }
                if (checkEquip == false)
                {
                    enemy.init(GAME.HeroType.None, -1, "");
                }

                enemys[i] = enemy;
            }
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab at {addressableKey_hero}");
        }
    }
}
