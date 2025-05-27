// simulation-core.js

window.simulateBattle = function simulateBattle(formData, resultDiv) {

  let playerLevel = parseInt(formData.playerLevel);
  const playerMaxLevel = parseInt(formData.playerMaxLevel);
  const baseAttack = parseFloat(formData.baseAttack);
  const attackC = parseFloat(formData.attackC);
  const baseCritChance = parseFloat(formData.critChance);
  const baseCritDamage = parseFloat(formData.critDamage);
  const baseAttackSpeed = parseFloat(formData.attackSpeed);

  const baseResource = parseFloat(formData.baseResource);
  const resourceInterval = parseFloat(formData.resourceInterval);

  const initialEnemyLevel = parseInt(formData.enemyLevel);
  const enemyMaxLevel = parseInt(formData.enemyMaxLevel);
  const enemyBaseHP = parseFloat(formData.enemyHP);
  const enemyBaseDef = parseFloat(formData.enemyBaseDef);
  const hpGrowthRate = parseFloat(formData.hpGrowthRate);
  const defC = parseFloat(formData.defC);
  const defE = parseFloat(formData.defE);
  const rewardBase = parseFloat(formData.rewardBase);
  const rewardCostC = parseFloat(formData.rewardCostC);
  const maxTime = parseFloat(formData.maxTime);
  const maxKill = parseInt(formData.maxKill);

  const upgradeCostLevel = parseFloat(formData.upgradeCostLevel);
  const upgradeCostAttack = parseFloat(formData.upgradeCostAttack);
  const upgradeCostSpeed = parseFloat(formData.upgradeCostSpeed);
  const upgradeCostCritChance = parseFloat(formData.upgradeCostCritChance);
  const upgradeCostCritDamage = parseFloat(formData.upgradeCostCritDamage);

  const levelCostC = parseFloat(formData.levelCostC);
  const attackCostC = parseFloat(formData.attackCostC);
  const speedCostC = parseFloat(formData.speedCostC);
  const critChanceCostC = parseFloat(formData.critChanceCostC);
  const critDamageCostC = parseFloat(formData.critDamageCostC);

  const maxAttackLevel = parseInt(formData.maxAttackLevel);
  const maxSpeedLevel = parseInt(formData.maxSpeedLevel);
  const maxCritChanceLevel = parseInt(formData.maxCritChanceLevel);
  const maxCritDamageLevel = parseInt(formData.maxCritDamageLevel);

  let attackLevel = 0, speedLevel = 0, critChanceLevel = 0, critDamageLevel = 0;

  function calcUpgradeCost(baseCost, costC, level) {
    return baseCost * Math.pow(costC, level);
  }

  const strategy = formData.strategyType || 'balanced';
  const weights = {
	  level: parseFloat(document.getElementById(`${strategy}LevelUp`).value),
	  attack: parseFloat(document.getElementById(`${strategy}Attack`).value),
	  critChance: parseFloat(document.getElementById(`${strategy}CritChance`).value),
	  critDamage: parseFloat(document.getElementById(`${strategy}CritDamage`).value),
	  speed: parseFloat(document.getElementById(`${strategy}Speed`).value)
  };

  let totalTime = 0, killCount = 0, attackCount = 0, resource = 0;
  let totalResource = 0;
  let nextResourceTime = resourceInterval, totalDamageDealt = 0;
  let currentEnemyLevel = initialEnemyLevel;
  let enemyHP = enemyBaseHP * Math.pow(hpGrowthRate, currentEnemyLevel - 1);
  let enemyDefense = enemyBaseDef * Math.pow(defE, defC * (currentEnemyLevel - 1));
  const battleLogsHtml = [];
  let currentEnemyLog = [];
  const powerTimeline = [];
  const upgradeTimeline = [];

  while (totalTime < maxTime && killCount < maxKill) {
    while (totalTime >= nextResourceTime) {
      const gain = baseResource * playerLevel;
      resource += gain;
	  totalResource += gain;
      currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">💰 시간 ${nextResourceTime.toFixed(2)}초: 재화 ${gain.toFixed(2)} 획득  (누적 재화: ${totalResource.toFixed(2)})</span>`);
      nextResourceTime += resourceInterval;
    }

	function getRandomWeightedStat(weights) {
	  const entries = [
		{ key: 'level', weight: weights.level },
		{ key: 'attack', weight: weights.attack },
		{ key: 'speed', weight: weights.speed },
		{ key: 'critChance', weight: weights.critChance },
		{ key: 'critDamage', weight: weights.critDamage }
	  ];

	  const total = entries.reduce((sum, e) => sum + e.weight, 0);
	  const rand = Math.random() * total;
	  let acc = 0;

	  for (const entry of entries) {
		acc += entry.weight;
		if (rand <= acc) return entry.key;
	  }
	  return entries[0].key; // fallback
	}
	
	 let currentUpgrade = {
      time: totalTime.toFixed(2),
      level: playerLevel,
      attack: attackLevel,
      speed: speedLevel,
      critChance: critChanceLevel,
      critDamage: critDamageLevel
    };
	
	let upgradeAttempted = true;
	while (resource > 0 && upgradeAttempted) {
	  upgradeAttempted = false;  // 먼저 false로 설정
	  const statType = getRandomWeightedStat(weights);

	  switch (statType) {
		case 'level':
		  if (playerLevel < playerMaxLevel) {
			const cost = calcUpgradeCost(upgradeCostLevel, levelCostC, playerLevel);
			if (resource >= cost) {
			  resource -= cost;
			  currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">✨ 레벨 업그레이드! → (${playerLevel} -> ${playerLevel+1}) 레벨 (비용: ${cost.toFixed(2)})</span>`);		
			  playerLevel++;	
              currentUpgrade.level = playerLevel;  
			}
		  }
		  break;
		case 'attack':
		  if (attackLevel < maxAttackLevel) {
			const cost = calcUpgradeCost(upgradeCostAttack, attackCostC, attackLevel);
			if (resource >= cost) {
			  resource -= cost;
			  currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">✨ 공격력 업그레이드! → (${attackLevel} -> ${attackLevel+1})}레벨 (비용: ${cost.toFixed(2)})</span>`);	
			  attackLevel++;	
              currentUpgrade.attack = attackLevel;
			}
		  }
		  break;

		case 'speed':
		  if (speedLevel < maxSpeedLevel) {
			const cost = calcUpgradeCost(upgradeCostSpeed, speedCostC, speedLevel);
			if (resource >= cost) {
			  resource -= cost;
			  currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">✨ 공격속도 업그레이드! → (${speedLevel} -> ${speedLevel+1})레벨 (비용: ${cost.toFixed(2)})</span>`);
			  speedLevel++;
              currentUpgrade.speed = speedLevel;
			}
		  }
		  break;

		case 'critChance':
		  if (critChanceLevel < maxCritChanceLevel) {
			const cost = calcUpgradeCost(upgradeCostCritChance, critChanceCostC, critChanceLevel);
			if (resource >= cost) {
			  resource -= cost;
			  currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">✨ 크리 확률 업그레이드! → (${critChanceLevel} -> ${critChanceLevel+1})레벨 (비용: ${cost.toFixed(2)})</span>`);
			  critChanceLevel++;
              currentUpgrade.critChance = critChanceLevel;
			}
		  }
		  break;

		case 'critDamage':
		  if (critDamageLevel < maxCritDamageLevel) {
			const cost = calcUpgradeCost(upgradeCostCritDamage, critDamageCostC, critDamageLevel);
			if (resource >= cost) {
			  resource -= cost;
			  currentEnemyLog.push(`<span class="resource-log" style="color: green; font-weight: bold;">✨ 크리 데미지 업그레이드! → (${critDamageLevel} -> ${critDamageLevel+1})레벨 (비용: ${cost.toFixed(2)})</span>`);
			  critDamageLevel++;
              currentUpgrade.critDamage = critDamageLevel;
			}
		  }
		  break;
	  }
	}

    attackCount++;
    const rawAttack = (baseAttack + (attackLevel * 0.2)) * Math.log(playerLevel + 1) * attackC;
    const playerCritChance = Math.min(baseCritChance + playerLevel + critChanceLevel, 100);
    const playerCritDamage = baseCritDamage + playerLevel * 0.5 + critDamageLevel * 0.5;
    const playerAttackSpeed = baseAttackSpeed + (playerLevel + speedLevel) / 10;
    const attackInterval = 1 / playerAttackSpeed;

    const attackAfterDefense = rawAttack * (1 / (1 + enemyDefense / 100));
    const isCrit = Math.random() * 100 < playerCritChance;
    const finalDamage = isCrit ? attackAfterDefense * playerCritDamage : attackAfterDefense;

    enemyHP -= finalDamage;
    totalTime += attackInterval;
    totalDamageDealt += finalDamage;
	
	const maxEnemyHP = enemyBaseHP * Math.pow(hpGrowthRate, currentEnemyLevel - 1);
	const upgradedBaseAttack = baseAttack + (attackLevel * 0.2);
	const formattedLog = `
	[공격 ${attackCount}회차] ⏱️ 시간: ${totalTime.toFixed(2)}초
	┌──────────── 캐릭터 상태 ────────────┐
	│ 레벨:           ${playerLevel}
	│ 공격력:         기본 ${upgradedBaseAttack.toFixed(2)} → 계산 ${rawAttack.toFixed(2)} → 방어 후 ${attackAfterDefense.toFixed(2)}
	│ 공격속도:       ${playerAttackSpeed.toFixed(2)} 회/초
	│ 크리티컬 확률:   ${playerCritChance.toFixed(2)}%
	│ 크리티컬 데미지: ${playerCritDamage.toFixed(2)}배
	│ 보유 재화:       ${resource.toFixed(2)}
	└──────────────────────────────┘
	┌────── 적 상태 ──────┐
	│ 레벨:    ${currentEnemyLevel}
	│ 체력:    ${maxEnemyHP.toFixed(2)} → ${enemyHP.toFixed(2)}
	│ 방어력:  ${enemyDefense.toFixed(2)}
	└─────────────────┘

	💥 입힌 데미지: ${finalDamage.toFixed(2)} ${isCrit ? '(크리티컬!)' : '(일반 공격)'}
	`;

	currentEnemyLog.push(`<pre>${formattedLog}</pre>`);



    if (enemyHP <= 0) {
      killCount++;
      const rewardAmount = rewardBase * Math.pow(rewardCostC, killCount - 1);
      resource += rewardAmount;
	  totalResource += rewardAmount;
      currentEnemyLog.push(`<span class="kill" style="color: green; font-weight: bold;">✅ 적 처치 완료 ${rewardAmount.toFixed(2)}  (누적 재화: ${totalResource.toFixed(2)}) 획득 (${killCount}마리)</span>`);

      battleLogsHtml.push(`
<details>
  <summary>🐲 ${killCount}번째 적 처치 (레벨 ${currentEnemyLevel}, 시간 ${totalTime.toFixed(2)}초)</summary>
  <pre>${currentEnemyLog.join('\n')}</pre>
</details>
      `);
      currentEnemyLog = [];

    const combatPower1 = 
	  (rawAttack ** 1.1) * 0.8  // 공격력 지수 증가 반영
	  * (1 + playerCritChance / 150)  // 크리 확률 영향 가중(과도하지 않게)
	  * (1 + playerCritDamage / 50)   // 크리 데미지 가중
	  * Math.log(playerLevel + 2)     // 레벨 로그 성장
	  + playerAttackSpeed * 30        // 공격속도는 합산으로 보조 가중치
	  + playerLevel * 15;             // 레벨 상수 가중치 추가

	
    const expectedDmg = rawAttack * (1 + Math.min(playerCritChance, 100) / 100 * (playerCritDamage - 1));
	// 공격속도와 기대데미지 곱에 지수 성장 반영
	const combatPower2 = 
	  expectedDmg * (playerAttackSpeed ** 1.2) * 12  // 공격속도 지수적 영향 반영
	  * Math.log(playerLevel + 2)                   // 레벨 로그 성장 반영
	  + Math.pow(playerLevel, 1.3) * 10;             // 레벨 지수 가중치 추가

    const combatPower3 = totalDamageDealt;

      powerTimeline.push({
        time: totalTime.toFixed(2),
        combat1: combatPower1,
        combat2: combatPower2,
        combat3: combatPower3,
		level: playerLevel,
		attack: attackLevel,
		speed: speedLevel,
		critChance: critChanceLevel,
		critDamage: critDamageLevel
      });
	  
	  upgradeTimeline.push(currentUpgrade);

      if (currentEnemyLevel < enemyMaxLevel) currentEnemyLevel++;
      enemyHP = enemyBaseHP * Math.pow(hpGrowthRate, currentEnemyLevel - 1);
      enemyDefense = enemyBaseDef * Math.pow(defE, defC * (currentEnemyLevel - 1));
    }
  }

	const summaryData1 = {
	 '총 사냥 시간(초)': totalTime,
	 '최종 재화': totalResource
	};

	const summaryData2 = {
	  '총 처치 수': killCount,
	  '최종 내 레벨': playerLevel,
	  '최종 적 레벨': currentEnemyLevel
	};

	resultDiv.innerHTML = `
	<details style="margin-top: 20px;">
	  <summary style="cursor: pointer; font-weight: bold;">📊 전투 결과 요약</summary>
	  <div style="margin-top: 10px;">
		<strong>시뮬레이션 결과 요약</strong>
		<canvas id="summaryChart1" style="max-width: 100%; height: 400px; margin-bottom: 20px;"></canvas>
		<canvas id="summaryChart2" style="max-width: 100%; height: 400px; margin-bottom: 20px;"></canvas>

		<strong>전투력 변화 추이</strong>
		<div style="position: relative;">
		  <canvas id="powerChart" style="max-width: 100%; height: 400px; margin-bottom: 20px;"></canvas>
		  <div id="yLevelLegend"
			 style="position: absolute; right: -180px; top: 60px;
					font-weight: bold; background: white;
					padding: 8px 12px; border-radius: 6px;
					box-shadow: 0 0 4px rgba(0,0,0,0.15);">
		  </div>
		</div>
		<label><input type="checkbox" id="toggleLevelGraphs" checked /> 레벨업 그래프 보기</label>

		<details style="margin-top: 15px;">
		  <summary style="cursor: pointer; font-weight: bold;">📝 전체 전투 로그</summary>
		  <div id="battleLog" style="max-height: 500px; overflow-y:auto; border:1px solid #ccc; padding:10px; margin-top:10px;">
			${battleLogsHtml.join('\n')}
		  </div>
		</details>
	  </div>
	</details>
	<hr style="margin: 30px 0; border: none; border-top: 2px dashed #ccc;" />
	`;

	// 레벨업 그래프 ON / OFF
	document.getElementById('toggleLevelGraphs').addEventListener('change', (e) => {
	  const showLevels = e.target.checked;
	  if (!window.powerChartInstance) return;
	  window.powerChartInstance.data.datasets.forEach(dataset => {
		if (['캐릭터 레벨', '공격력 레벨', '공격속도 레벨', '크리 확률 레벨', '크리 데미지 레벨'].includes(dataset.label)) {
		  dataset.hidden = !showLevels;  // 체크박스 체크 시 보이고, 해제 시 숨기기
		}
	  });
	  window.powerChartInstance.update();
	});

  // summaryChart1 추가 렌더링
  const ctx1 = document.getElementById('summaryChart1').getContext('2d');
  if (window.summaryChartInstance) window.summaryChartInstance.destroy();
  window.summaryChartInstance = new Chart(ctx1, {
    type: 'bar',
    data: {
      labels: Object.keys(summaryData1),
      datasets: [
        {
          label: '시뮬레이션 결과',
          data: Object.values(summaryData1).map(v => +v.toFixed(2)),
          backgroundColor: 'rgba(54, 162, 235, 0.6)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }
      ]
    },
    options: {
      responsive: true,
      scales: {
        x: { title: { display: true, text: '요약 지표' } },
        y: {
          title: { display: true, text: '값' },
          beginAtZero: true
        }
      }
    }
  });  
  
  // 새 캔버스(예: summaryChart2) 만들어야 함
	const ctx2 = document.getElementById('summaryChart2').getContext('2d');
	if (window.summaryChartInstance2) window.summaryChartInstance2.destroy();
	window.summaryChartInstance2 = new Chart(ctx2, {
	  type: 'bar',
	  data: {
		labels: Object.keys(summaryData2),
		datasets: [{
		  label: '시뮬레이션 결과',
		  data: Object.values(summaryData2).map(v => +v.toFixed(2)),
		  backgroundColor: 'rgba(255, 159, 64, 0.6)',
		  borderColor: 'rgba(255, 159, 64, 1)',
		  borderWidth: 1
		}]
	  },
	  options: {
		responsive: true,
		scales: {
		  x: { title: { display: true, text: '요약 지표' } },
		  y: { beginAtZero: true, title: { display: true, text: '값' } }
		}
	  }
	});

  // powerChart 렌더링 유지
  const powerCtx = document.getElementById('powerChart').getContext('2d');
  if (window.powerChartInstance) window.powerChartInstance.destroy();
  const annotations = {};
  
  
  const showLevelGraphs = document.getElementById('toggleLevelGraphs')?.checked ?? true;

  // datasets 배열 준비
  const datasets = [
    {
      label: '전투력1 (합산)',
      data: powerTimeline.map(p => p.combat1),
      borderColor: 'blue',
      yAxisID: 'yPower',
      fill: false,
      tension: 0.3
    },
    {
      label: '전투력2 (DPS기반)',
      data: powerTimeline.map(p => p.combat2),
      borderColor: 'green',
      yAxisID: 'yPower',
      fill: false,
      tension: 0.3
    },
    {
      label: '전투력3 (누적피해)',
      data: powerTimeline.map(p => p.combat3),
      borderColor: 'red',
      yAxisID: 'yPower',
      fill: false,
      tension: 0.3
    }
  ];

  if (showLevelGraphs) {
    datasets.push(
      {
        label: '캐릭터 레벨',
        data: upgradeTimeline.map(p => p.level),
        borderColor: 'blue',
		borderWidth: 1,  
        yAxisID: 'yLevel',
        fill: false,
        tension: 0.3
      },
      {
        label: '공격력 레벨',
        data: upgradeTimeline.map(p => p.attack),
        borderColor: 'red',
		borderWidth: 1,  
        yAxisID: 'yLevel',
        fill: false,
        tension: 0.3
      },
      {
        label: '공격속도 레벨',
        data: upgradeTimeline.map(p => p.speed),
        borderColor: 'orange',
		borderWidth: 1,  
        yAxisID: 'yLevel',
        fill: false,
        tension: 0.3
      },
      {
        label: '크리 확률 레벨',
        data: upgradeTimeline.map(p => p.critChance),
        borderColor: 'green',
		borderWidth: 1,  
        yAxisID: 'yLevel',
        fill: false,
        tension: 0.3
      },
      {
        label: '크리 데미지 레벨',
        data: upgradeTimeline.map(p => p.critDamage),
        borderColor: 'purple',
		borderWidth: 1,  
        yAxisID: 'yLevel',
        fill: false,
        tension: 0.3
      }
    );
  }
  
  window.powerChartInstance = new Chart(powerCtx, {
    type: 'line',
    data: {
      labels: powerTimeline.map(p => p.time),
      datasets: datasets
    },
    options: {
      responsive: true,
      scales: {
        x: {
          title: { display: true, text: '시간 (초)' }
        },
        yPower: {
          type: 'linear',
          position: 'left',
          title: { display: true, text: '전투력 수치' },
          beginAtZero: true
        },
        yLevel: {
          type: 'linear',
          position: 'right',
          title: { display: true, text: '레벨 수치' },
          beginAtZero: true,
          min: 0,
          max: 100,
          grid: { drawOnChartArea: false }
        }
      },
      plugins: {
		legend: {
			labels: {
			  filter: function(legendItem, chartData) {
				// legendItem.text가 레전드 텍스트
				// 전투력1,2,3은 보여주고 나머지는 숨김
				return ['전투력1 (합산)', '전투력2 (DPS기반)', '전투력3 (누적피해)'].includes(legendItem.text);
			  }
			}
		},
        annotation: { annotations }
      }
    }
  });
  
  const yLevelLegend = document.getElementById('yLevelLegend');
	yLevelLegend.innerHTML = `
	  <div style="color: rgba(0,0,255,0.8); margin-bottom: 4px;">■ 캐릭터 레벨</div>
	  <div style="color: rgba(255,0,0,0.8); margin-bottom: 4px;">■ 공격력 레벨</div>
	  <div style="color: rgba(255,165,0,0.8); margin-bottom: 4px;">■ 속도 레벨</div>
	  <div style="color: rgba(0,128,0,0.8); margin-bottom: 4px;">■ 크리 확률 레벨</div>
	  <div style="color: rgba(128,0,128,0.8); margin-bottom: 4px;">■ 크리 데미지 레벨</div>
	`;
}
