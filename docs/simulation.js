const form = document.getElementById('attackForm');
const resultDiv = document.getElementById('result');
const downloadBtn = document.getElementById('downloadBtn');

const HP_GROWTH_RATE = 1.15;
const KILL_FOR_LEVELUP = 10;

function calcEnemyDefense(baseDef, level, defC, defE) {
  return baseDef * Math.pow(defE, defC * (level - 1));
}

function calcEnemyHP(baseHP, level) {
  return baseHP * Math.pow(HP_GROWTH_RATE, level - 1);
}

function calcPlayerAttackRaw(baseAttack, playerLevel, cAtk) {
  return baseAttack * Math.log(playerLevel + 1) * cAtk;
}

function calcPlayerAttackAfterDefense(rawAttack, enemyDefense) {
  return rawAttack * (1 / (1 + enemyDefense / 100));
}

function calcPlayerCritChance(baseCritChance, playerLevel) {
  let crit = baseCritChance + playerLevel * 1;
  if (crit > 100) crit = 100;
  return crit;
}

function calcPlayerCritDamage(baseCritDamage, playerLevel) {
  return baseCritDamage + playerLevel * 0.5;
}

function calcPlayerAttackSpeed(baseAttackSpeed, playerLevel) {
  return baseAttackSpeed + playerLevel / 10;
}

function calcDamage(rawAttack, critChance, critDamage, enemyDefense) {
  const attackAfterDefense = calcPlayerAttackAfterDefense(rawAttack, enemyDefense);
  const isCrit = Math.random() * 100 < critChance;
  const finalDamage = isCrit ? attackAfterDefense * critDamage : attackAfterDefense;
  return { finalDamage, isCrit, rawAttack, attackAfterDefense };
}

form.addEventListener('submit', function(event) {
  event.preventDefault();
  runSimulation();
});

downloadBtn.addEventListener('click', function() {
  const formData = getFormData();
  const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(formData, null, 2));
  const dlAnchorElem = document.createElement('a');
  dlAnchorElem.setAttribute("href", dataStr);
  dlAnchorElem.setAttribute("download", "simulation_data.json");
  dlAnchorElem.click();
});

function calcUpgradeCost(baseCost, costConstant, level) {
  return baseCost * Math.pow(costConstant, level - 1);
}

let upgradeChartInstance = null;

function showUpgradeCosts() {
  const upgradeCostLevel = parseFloat(document.getElementById('upgradeCostLevel').value);
  const upgradeCostAttack = parseFloat(document.getElementById('upgradeCostAttack').value);
  const upgradeCostSpeed = parseFloat(document.getElementById('upgradeCostSpeed').value);
  const upgradeCostCritChance = parseFloat(document.getElementById('upgradeCostCritChance').value);
  const upgradeCostCritDamage = parseFloat(document.getElementById('upgradeCostCritDamage').value);

  const levelCostC = parseFloat(document.getElementById('levelCostC').value);
  const attackCostC = parseFloat(document.getElementById('attackCostC').value);
  const speedCostC = parseFloat(document.getElementById('speedCostC').value);
  const critChanceCostC = parseFloat(document.getElementById('critChanceCostC').value);
  const critDamageCostC = parseFloat(document.getElementById('critDamageCostC').value);

  const maxLevelUp = parseInt(document.getElementById('maxLevelUp').value);
  const maxAttackLevel = parseInt(document.getElementById('maxAttackLevel').value);
  const maxSpeedLevel = parseInt(document.getElementById('maxSpeedLevel').value);
  const maxCritChanceLevel = parseInt(document.getElementById('maxCritChanceLevel').value);
  const maxCritDamageLevel = parseInt(document.getElementById('maxCritDamageLevel').value);

  const maxLevel = Math.max(maxLevelUp, maxAttackLevel, maxSpeedLevel, maxCritChanceLevel, maxCritDamageLevel);

  let html = `
    <h3>업그레이드 비용 계산 (각 항목별 최대 레벨까지)</h3>
    <table border="1" cellpadding="6" cellspacing="0" style="border-collapse: collapse; width: 100%;">
      <thead style="background-color: #007bff; color: white;">
        <tr>
          <th>레벨</th>
          <th>레벨업 비용</th>
          <th>공격력 비용</th>
          <th>공격속도 비용</th>
          <th>크리티컬 확률 비용</th>
          <th>크리티컬 데미지 비용</th>
        </tr>
      </thead>
      <tbody>
  `;

  const levels = [];
  const costLevels = [];
  const costAttacks = [];
  const costSpeeds = [];
  const costCritChances = [];
  const costCritDamages = [];

  for(let lvl = 1; lvl <= maxLevel; lvl++) {
    const costLevel = (lvl <= maxLevelUp) ? calcUpgradeCost(upgradeCostLevel, levelCostC, lvl) : null;
    const costAttack = (lvl <= maxAttackLevel) ? calcUpgradeCost(upgradeCostAttack, attackCostC, lvl) : null;
    const costSpeed = (lvl <= maxSpeedLevel) ? calcUpgradeCost(upgradeCostSpeed, speedCostC, lvl) : null;
    const costCritChance = (lvl <= maxCritChanceLevel) ? calcUpgradeCost(upgradeCostCritChance, critChanceCostC, lvl) : null;
    const costCritDamage = (lvl <= maxCritDamageLevel) ? calcUpgradeCost(upgradeCostCritDamage, critDamageCostC, lvl) : null;

    html += `
      <tr>
        <td style="text-align:center;">${lvl}</td>
        <td style="text-align:right;">${costLevel !== null ? costLevel.toFixed(2) : ''}</td>
        <td style="text-align:right;">${costAttack !== null ? costAttack.toFixed(2) : ''}</td>
        <td style="text-align:right;">${costSpeed !== null ? costSpeed.toFixed(2) : ''}</td>
        <td style="text-align:right;">${costCritChance !== null ? costCritChance.toFixed(2) : ''}</td>
        <td style="text-align:right;">${costCritDamage !== null ? costCritDamage.toFixed(2) : ''}</td>
      </tr>
    `;

    levels.push(lvl);
    costLevels.push(costLevel);
    costAttacks.push(costAttack);
    costSpeeds.push(costSpeed);
    costCritChances.push(costCritChance);
    costCritDamages.push(costCritDamage);
  }

  html += `
      </tbody>
    </table>
    <canvas id="upgradeChart" style="max-width: 100%; height: 400px; margin-top: 30px;"></canvas>
  `;

  resultDiv.innerHTML = html;

  if (upgradeChartInstance) {
    upgradeChartInstance.destroy();
  }

  const ctx = document.getElementById('upgradeChart').getContext('2d');

  upgradeChartInstance = new Chart(ctx, {
    type: 'line',
    data: {
      labels: levels,
      datasets: [
        {
          label: '레벨업 비용',
          data: costLevels,
          borderColor: 'rgba(255, 99, 132, 1)',
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          spanGaps: true,
          fill: false,
        },
        {
          label: '공격력 비용',
          data: costAttacks,
          borderColor: 'rgba(54, 162, 235, 1)',
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          spanGaps: true,
          fill: false,
        },
        {
          label: '공격속도 비용',
          data: costSpeeds,
          borderColor: 'rgba(255, 206, 86, 1)',
          backgroundColor: 'rgba(255, 206, 86, 0.2)',
          spanGaps: true,
          fill: false,
        },
        {
          label: '크리티컬 확률 비용',
          data: costCritChances,
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          spanGaps: true,
          fill: false,
        },
        {
          label: '크리티컬 데미지 비용',
          data: costCritDamages,
          borderColor: 'rgba(153, 102, 255, 1)',
          backgroundColor: 'rgba(153, 102, 255, 0.2)',
          spanGaps: true,
          fill: false,
        },
      ]
    },
    options: {
      responsive: true,
      interaction: {
        mode: 'nearest',
        axis: 'x',
        intersect: false
      },
      plugins: {
        legend: { position: 'top' },
        tooltip: { enabled: true, mode: 'nearest', intersect: false }
      },
      scales: {
        x: { title: { display: true, text: '레벨' } },
        y: { title: { display: true, text: '업그레이드 비용' }, beginAtZero: true }
      }
    }
  });
}

document.getElementById('calculateUpgradeBtn').addEventListener('click', showUpgradeCosts);

function getFormData() {
  return {
    playerLevel: document.getElementById('playerLevel').value,
    playerMaxLevel: document.getElementById('playerMaxLevel').value,
    baseAttack: document.getElementById('baseAttack').value,
    attackSpeed: document.getElementById('attackSpeed').value,
    attackC: document.getElementById('attackC').value,
    critChance: document.getElementById('critChance').value,
    critDamage: document.getElementById('critDamage').value,
    baseResource: document.getElementById('baseResource').value,
    resourceInterval: document.getElementById('resourceInterval').value,
    upgradeCostLevel: document.getElementById('upgradeCostLevel').value,
    upgradeCostAttack: document.getElementById('upgradeCostAttack').value,
    upgradeCostSpeed: document.getElementById('upgradeCostSpeed').value,
    upgradeCostCritChance: document.getElementById('upgradeCostCritChance').value,
    upgradeCostCritDamage: document.getElementById('upgradeCostCritDamage').value,
    levelCostC: document.getElementById('levelCostC').value,
    attackCostC: document.getElementById('attackCostC').value,
    speedCostC: document.getElementById('speedCostC').value,
    critChanceCostC: document.getElementById('critChanceCostC').value,
    critDamageCostC: document.getElementById('critDamageCostC').value,
    maxLevelUp: document.getElementById('maxLevelUp').value,
    maxAttackLevel: document.getElementById('maxAttackLevel').value,
    maxSpeedLevel: document.getElementById('maxSpeedLevel').value,
    maxCritChanceLevel: document.getElementById('maxCritChanceLevel').value,
    maxCritDamageLevel: document.getElementById('maxCritDamageLevel').value,
    enemyLevel: document.getElementById('enemyLevel').value,
    enemyMaxLevel: document.getElementById('enemyMaxLevel').value,
    enemyHP: document.getElementById('enemyHP').value,
    enemyBaseDef: document.getElementById('enemyBaseDef').value,
    defC: document.getElementById('defC').value,
    defE: document.getElementById('defE').value,
    rewardBase: document.getElementById('rewardBase').value,
    rewardCostC: document.getElementById('rewardCostC').value,
    maxTime: document.getElementById('maxTime').value,
    maxKill: document.getElementById('maxKill').value,
  };
}

function runSimulation() {
  let playerLevel = parseInt(document.getElementById('playerLevel').value);
  const playerMaxLevel = parseInt(document.getElementById('playerMaxLevel').value);
  const baseAttack = parseFloat(document.getElementById('baseAttack').value);
  const attackC = parseFloat(document.getElementById('attackC').value);
  const baseCritChance = parseFloat(document.getElementById('critChance').value);
  const baseCritDamage = parseFloat(document.getElementById('critDamage').value);
  const baseAttackSpeed = parseFloat(document.getElementById('attackSpeed').value);

  const baseResource = parseFloat(document.getElementById('baseResource').value);
  const resourceInterval = parseFloat(document.getElementById('resourceInterval').value);

  const upgradeCostLevel = parseFloat(document.getElementById('upgradeCostLevel').value);
  const upgradeCostAttack = parseFloat(document.getElementById('upgradeCostAttack').value);
  const upgradeCostSpeed = parseFloat(document.getElementById('upgradeCostSpeed').value);
  const upgradeCostCritChance = parseFloat(document.getElementById('upgradeCostCritChance').value);
  const upgradeCostCritDamage = parseFloat(document.getElementById('upgradeCostCritDamage').value);

  const enemyLevel = parseInt(document.getElementById('enemyLevel').value);
  const enemyMaxLevel = parseInt(document.getElementById('enemyMaxLevel').value);
  const enemyBaseHP = parseFloat(document.getElementById('enemyHP').value);
  const enemyBaseDef = parseFloat(document.getElementById('enemyBaseDef').value);
  const defC = parseFloat(document.getElementById('defC').value);
  const defE = parseFloat(document.getElementById('defE').value);
  const rewardBase = parseFloat(document.getElementById('rewardBase').value);
  const rewardCostC = parseFloat(document.getElementById('rewardCostC').value);
  const maxTime = parseFloat(document.getElementById('maxTime').value);
  const maxKill = parseInt(document.getElementById('maxKill').value);

  let totalTime = 0;
  let killCount = 0;
  let attackCount = 0;
  let resource = 0;

  let nextResourceTime = resourceInterval;

  const MAX_SIM_TIME = maxTime;
  const MAX_KILL = maxKill;

  let enemyHP = calcEnemyHP(enemyBaseHP, enemyLevel);
  let enemyDefense = calcEnemyDefense(enemyBaseDef, enemyLevel, defC, defE);

  const logEntries = [];

  while (totalTime < MAX_SIM_TIME && killCount < MAX_KILL) {
    while (totalTime >= nextResourceTime) {
      const gain = baseResource * playerLevel;
      resource += gain;
      logEntries.push(`<span class="resource-log">시간 ${nextResourceTime.toFixed(2)}초: 재화 ${gain.toFixed(2)} 획득! 총 재화: ${resource.toFixed(2)}</span>\n---------------------------`);
      nextResourceTime += resourceInterval;
    }

    attackCount++;

    const rawAttack = calcPlayerAttackRaw(baseAttack, playerLevel, attackC);
    const playerCritChance = calcPlayerCritChance(baseCritChance, playerLevel);
    const playerCritDamage = calcPlayerCritDamage(baseCritDamage, playerLevel);
    const playerAttackSpeed = calcPlayerAttackSpeed(baseAttackSpeed, playerLevel);

    const attackInterval = 1 / playerAttackSpeed;

    const { finalDamage, isCrit, attackAfterDefense } = calcDamage(rawAttack, playerCritChance, playerCritDamage, enemyDefense);

    enemyHP -= finalDamage;
    totalTime += attackInterval;

    const damageHTML = isCrit
      ? `<span class="crit">입힌 데미지: ${finalDamage.toFixed(2)} (크리티컬 공격!)</span>`
      : `입힌 데미지: ${finalDamage.toFixed(2)} (일반 공격)`;

    logEntries.push(`공격 ${attackCount}회차
시간: ${totalTime.toFixed(2)}초
공격력 (방어력 적용 전) : ${rawAttack.toFixed(2)} / 공격력 (방어력 적용 후) : ${attackAfterDefense.toFixed(2)}
크리티컬 확률: ${playerCritChance.toFixed(2)}%
크리티컬 데미지 배율 (최종 데미지 X 연산): ${playerCritDamage.toFixed(2)}
공격 속도: ${playerAttackSpeed.toFixed(2)}회/초
적 레벨: ${enemyLevel}
적 체력: ${enemyHP.toFixed(2)}
적 방어력: ${enemyDefense.toFixed(2)}
내 재화: ${resource.toFixed(2)}
${damageHTML}
---------------------------
`);

    if (enemyHP <= 0) {
      killCount++;

      // 보상 재화 계산
      const rewardAmount = rewardBase * Math.pow(rewardCostC, killCount - 1);
      resource += rewardAmount;
      logEntries.push(`<span class="resource-log">적 처치 보상: 재화 ${rewardAmount.toFixed(2)} 획득! 총 재화: ${resource.toFixed(2)}</span>\n---------------------------`);

      logEntries.push(`<span class="kill">적 처치! 총 처치 수: ${killCount}</span>\n---------------------------`);

      const newEnemyLevel = Math.min(enemyLevel + killCount, enemyMaxLevel);
      enemyHP = calcEnemyHP(enemyBaseHP, newEnemyLevel);
      enemyDefense = calcEnemyDefense(enemyBaseDef, newEnemyLevel, defC, defE);

      if (killCount % KILL_FOR_LEVELUP === 0 && playerLevel < playerMaxLevel) {
        playerLevel++;
        logEntries.push(`내 캐릭터 레벨업! 현재 레벨: ${playerLevel}\n---------------------------`);
      }
    }
  }

  // 표로 보기 좋은 시뮬레이션 요약
  resultDiv.innerHTML = `
    <strong>시뮬레이션 결과 요약</strong>
    <table border="1" cellpadding="6" cellspacing="0" style="border-collapse: collapse; margin-bottom: 15px;">
      <tbody>
        <tr><th style="text-align:left;">총 사냥 시간</th><td>${totalTime.toFixed(2)}초</td></tr>
        <tr><th style="text-align:left;">총 처치 수</th><td>${killCount}</td></tr>
        <tr><th style="text-align:left;">최종 내 레벨</th><td>${playerLevel}</td></tr>
        <tr><th style="text-align:left;">최종 적 레벨</th><td>${Math.min(enemyLevel + killCount, enemyMaxLevel)}</td></tr>
        <tr><th style="text-align:left;">최종 재화</th><td>${resource.toFixed(2)}</td></tr>
      </tbody>
    </table>

    <canvas id="summaryChart" style="max-width: 600px; height: 300px; margin-bottom: 20px;"></canvas>

    <button id="toggleBattleLogBtn" style="margin-bottom: 10px; cursor: pointer;">전투 로그 보기 ▼</button>
    <pre id="battleLog" style="display: none; white-space: pre-wrap; max-height: 400px; overflow-y: auto; border: 1px solid #ccc; padding: 10px;">
${logEntries.join('\n')}
    </pre>
  `;

  // 요약 차트 그리기
  const summaryData = {
    '총 사냥 시간(초)': totalTime,
    '총 처치 수': killCount,
    '최종 내 레벨': playerLevel,
    '최종 적 레벨': Math.min(enemyLevel + killCount, enemyMaxLevel),
    '최종 재화': resource
  };

  const ctx = document.getElementById('summaryChart').getContext('2d');

  if (window.summaryChartInstance) {
    window.summaryChartInstance.destroy();
  }

  window.summaryChartInstance = new Chart(ctx, {
    type: 'bar',
    data: {
      labels: Object.keys(summaryData),
      datasets: [{
        label: '시뮬레이션 결과',
        data: Object.values(summaryData).map(v => typeof v === 'number' ? +v.toFixed(2) : v),
        backgroundColor: [
          'rgba(255, 99, 132, 0.7)',
          'rgba(54, 162, 235, 0.7)',
          'rgba(255, 206, 86, 0.7)',
          'rgba(75, 192, 192, 0.7)',
          'rgba(153, 102, 255, 0.7)'
        ],
        borderColor: [
          'rgba(255, 99, 132, 1)',
          'rgba(54, 162, 235, 1)',
          'rgba(255, 206, 86, 1)',
          'rgba(75, 192, 192, 1)',
          'rgba(153, 102, 255, 1)'
        ],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      scales: {
        y: { beginAtZero: true }
      }
    }
  });

  // 전투 로그 토글 기능
  document.getElementById('toggleBattleLogBtn').addEventListener('click', () => {
    const battleLog = document.getElementById('battleLog');
    const btn = document.getElementById('toggleBattleLogBtn');
    if (battleLog.style.display === 'none') {
      battleLog.style.display = 'block';
      btn.textContent = '전투 로그 숨기기 ▲';
    } else {
      battleLog.style.display = 'none';
      btn.textContent = '전투 로그 보기 ▼';
    }
  });
}
