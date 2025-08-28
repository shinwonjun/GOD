
using System;
using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

            float beforeHP = currentEnemyHP;

            // 개별 계산
            float baseAtk = SimulationCalc.GetBaseAttack(); // 기본 공격력 (defaultAttack)
            float rawAtk = SimulationCalc.GetAttackPower(); // 스탯 및 레벨 적용된 계산 공격력
            float reducedAtk = SimulationCalc.ApplyDefense(rawAtk, currentEnemy.GetDefense());

            bool isCrit;
            float finalDamage = SimulationCalc.GetFinalAttackPower(out isCrit);
            currentEnemyHP -= finalDamage;

            HandleAttack(beforeHP, finalDamage, isCrit);

            var statLevels = GameMyData.Instance.UserData.statLevelsByIndex;

            Debug.Log(
    $@"[공격 {attackCount}회차] ⏱️ 시간: {elapsedTime:F2}초
┌──────────── 캐릭터 상태 ────────────┐
│ 레벨:           {statLevels[(int)STATUS_UI.Stat.Level]}
│ 공격력:         기본 {baseAtk:F2} → 계산 {rawAtk:F2} → 방어 후 {reducedAtk:F2} → 최종 {finalDamage:F2}
│ 공격속도:       {SimulationCalc.GetAttackSpeed():F2} 회/초
│ 크리티컬 확률:   {SimulationCalc.GetCriticalChance():F2}% (Lv {statLevels[(int)STATUS_UI.Stat.CriticalChance]})
│ 크리티컬 데미지: {SimulationCalc.GetCriticalDamage():F2}배 (Lv {statLevels[(int)STATUS_UI.Stat.CriticalDamage]})
│ 공격력 레벨:     {statLevels[(int)STATUS_UI.Stat.AttackPower]}
│ 속도 레벨:       {statLevels[(int)STATUS_UI.Stat.AttackSpeed]}
│ 보유 재화:       {GameMyData.Instance.Coin:F2}
└──────────────────────────────┘
┌────── 적 상태 ──────┐
│ 레벨:    {currentEnemy.Level}
│ 체력:    {beforeHP:F2} → {Mathf.Max(0, currentEnemyHP):F2}
│ 방어력:  {currentEnemy.GetDefense():F2}
└─────────────────┘

💥 입힌 데미지: {finalDamage:F2} {(isCrit ? "(크리티컬 공격!)" : "(일반 공격)")}\n"
            );

            if (currentEnemyHP <= 0)
            {
                Debug.Log("[Simulation] 적 처치 완료!");
                HandleEnemyKilled(finalDamage);
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

    private void HandleAttack(float beforeHP, float damage, bool isCrit)
    {
        float afterHP = beforeHP - damage;

        // hero attack 
        heroHandlers.heros[0].attack(isCrit);

        // enemy 체력 차감
        heroHandlers.enemys[0].hit(beforeHP, damage, isCrit);
    }

    private void HandleEnemyKilled(float lastHitDamage)
    {
        var enemyHP = GameMyData.Instance.UserData.enemy.GetHP();

        var payloadObj = new
        {
            enemyHP = enemyHP,
            lastHit = lastHitDamage
        };

        string payloadJson = JsonConvert.SerializeObject(payloadObj);


        NetworkManager.SendRequest_Test("KillEnemy", payloadJson);
        Debug.Log("[Simulation] kill!");
    }

    ////////////////////////////////////////////////////////////////////////////////////

    public async Task SetNextEnemy(bool successed)
    {
        int id = GameMyData.Instance.UserData.enemy.EnemyId;
        if (successed == true)
        {
            var data = DataManager.Instance.GetHeroData(id);
            if (data != null)
            {
                heroHandlers.enemys[0].die();
                await Task.Delay(2000);

                heroHandlers.enemys[0].init(data, 0);
                Enemy enemy = heroHandlers.enemys[0] as Enemy;
                if (enemy != null)
                {
                    enemy.setLevel(GameMyData.Instance.UserData.enemy.Level);
                }
            }

            Debug.Log($"[GameManager] KillEnemy 성공 - 새로운 적 등장: {id}, 총 코인: {GameMyData.Instance.UserData.enemy.Level}");
            await Task.Delay(1000);
        }
        else
        {
            Debug.Log($"[GameManager] KillEnemy 실패 - 다시 시작: {id}, 총 코인: {GameMyData.Instance.UserData.enemy.Level}");
            await Task.Delay(1000);
        }

        // 1초 후 시뮬 다시 시작
        StartSimulation();
    }
}
