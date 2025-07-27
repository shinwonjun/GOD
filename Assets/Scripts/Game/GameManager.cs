
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private Coroutine battleRoutine;
    private float currentEnemyHP;
    [SerializeField] public HeroHandler heroHandlers;

    void Start()
    {
        NetworkManager.Start();
        NetworkManager.SendRequest_Test("StartGame", "");
    }
    public async Task StartGame(string json)
    {
        GameMyData.Instance.LoadGameInfoJson(json);

        await UIManager.Instance.LoadDataUI();

        await Task.Delay(1000);     // 1초 정도 후에 
        await LoadGame();
    }

    public async Task LoadGame()
    {
        await heroHandlers.LoadHeroEnemy();
        NetworkManager.SendRequest_Test("GetServerTime", "");       // 현재 서버 시간 가져오고
        NetworkManager.SendRequest_Test("GetLastClaimTime", "");    // 재접속 전 마지막 시간 가져오고 + 보상

        await Task.Delay(500);

        StartSimulation();
    }




    //////////// simulation

    public void StartSimulation()
    {
        if (battleRoutine != null)
            StopCoroutine(battleRoutine);

        currentEnemyHP = GameMyData.Instance.UserData.enemy.GetHP();
        battleRoutine = StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        var currentEnemy = GameMyData.Instance.UserData.enemy;
        int attackCount = 0;
        float elapsedTime = 0f;

        while (currentEnemyHP > 0)
        {
            float attackInterval = 1f / SimulationCalc.GetAttackSpeed();
            yield return new WaitForSeconds(attackInterval);

            elapsedTime += attackInterval;
            attackCount++;

            var stat = GameMyData.Instance.UserData.statLevelsByIndex;
            var table = DataManager.Instance.myCharacterTable;

            // 공격력 계산
            int levelAtk = stat[(int)STATUS_UI.Stat.AttackPower];
            float baseAtk = table.DefaultAttackPower;
            float calcAtk = baseAtk * Mathf.Log(levelAtk + 1) * table.ConstantAttack;

            // 크리티컬
            int levelCritChance = stat[(int)STATUS_UI.Stat.CriticalChance];
            int levelCritDamage = stat[(int)STATUS_UI.Stat.CriticalDamage];

            float critChance = Mathf.Min(100f, table.DefaultCriticalChance + levelCritChance);
            float critMultiplier = table.DefaultCriticalDamage + levelCritDamage * 0.5f;

            // 방어력
            float defense = currentEnemy.GetDefense();

            // 최종 데미지
            bool isCrit;
            float damageBeforeDef = calcAtk;
            float damageAfterDef = damageBeforeDef * (1f / (1f + defense / 100f));
            isCrit = UnityEngine.Random.value < (critChance / 100f);
            float finalDamage = isCrit ? damageAfterDef * critMultiplier : damageAfterDef;

            float beforeHP = currentEnemyHP;
            currentEnemyHP -= finalDamage;

            // 출력
            Debug.Log(
    $@"[공격 {attackCount}회차] ⏱️ 시간: {elapsedTime:F2}초
	┌──────────── 캐릭터 상태 ────────────┐
	│ 레벨:           {GameMyData.Instance.UserData.statLevelsByIndex[(int)STATUS_UI.Stat.Level]}
	│ 공격력:         기본 {baseAtk:F2} → 계산 {calcAtk:F2} → 방어 후 {damageAfterDef:F2}
	│ 공격속도:       {SimulationCalc.GetAttackSpeed():F2} 회/초
	│ 크리티컬 확률:   {critChance:F2}%
	│ 크리티컬 데미지: {critMultiplier:F2}배
	│ 보유 재화:       {GameMyData.Instance.Coin:F2}
	└──────────────────────────────┘
	┌────── 적 상태 ──────┐
	│ 레벨:    {currentEnemy.Level}
	│ 체력:    {beforeHP:F2} → {Mathf.Max(0, currentEnemyHP):F2}
	│ 방어력:  {defense:F2}
	└─────────────────┘

	💥 입힌 데미지: {finalDamage:F2} {(isCrit ? "(크리티컬 공격!)" : "(일반 공격)")}\n"
            );

            if (currentEnemyHP <= 0)
            {
                Debug.Log("[Simulation] 적 처치 완료!");
                StopSimulation();
                yield break;
            }
        }
    }

    public void StopSimulation()
    {
        if (battleRoutine != null)
        {
            StopCoroutine(battleRoutine);
            battleRoutine = null;
            Debug.Log("[Simulation] 시뮬레이션 중단됨.");
        }
    }

}
