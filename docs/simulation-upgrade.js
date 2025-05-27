// simulation-upgrade.js (비모듈 방식)

window.showUpgradeCosts = function showUpgradeCosts(formData) {
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

  const maxLevelUp = parseInt(formData.maxLevelUp);
  const maxAttackLevel = parseInt(formData.maxAttackLevel);
  const maxSpeedLevel = parseInt(formData.maxSpeedLevel);
  const maxCritChanceLevel = parseInt(formData.maxCritChanceLevel);
  const maxCritDamageLevel = parseInt(formData.maxCritDamageLevel);

  const maxLevel = Math.max(maxLevelUp, maxAttackLevel, maxSpeedLevel, maxCritChanceLevel, maxCritDamageLevel);

  const levels = [];
  const costLevels = [], costAttacks = [], costSpeeds = [], costCritChances = [], costCritDamages = [];

  function calcUpgradeCost(baseCost, costConstant, level) {
    return baseCost * Math.pow(costConstant, level - 1);
  }

  const upgradeDetails = document.createElement('details');
  upgradeDetails.style.marginTop = '20px';
  upgradeDetails.open = false; // 기본은 닫힌 상태

  const summary = document.createElement('summary');
  summary.textContent = '📈 업그레이드 비용 보기';
  summary.style.cursor = 'pointer';
  summary.style.fontWeight = 'bold';
  summary.style.marginBottom = '10px';

  const upgradeDiv = document.createElement('div');
  upgradeDiv.id = "upgrade-costs";

  upgradeDetails.appendChild(summary);
  upgradeDetails.appendChild(upgradeDiv);
  const resultDiv = document.getElementById('result');
  resultDiv.appendChild(upgradeDetails);

  let html = `
    <h3>업그레이드 비용 계산</h3>
    <table border="1" cellpadding="6" cellspacing="0" style="border-collapse: collapse; width: 100%; margin-bottom: 15px;">
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

  for (let lvl = 1; lvl <= maxLevel; lvl++) {
    const levelCost = lvl <= maxLevelUp ? calcUpgradeCost(upgradeCostLevel, levelCostC, lvl) : null;
    const attackCost = lvl <= maxAttackLevel ? calcUpgradeCost(upgradeCostAttack, attackCostC, lvl) : null;
    const speedCost = lvl <= maxSpeedLevel ? calcUpgradeCost(upgradeCostSpeed, speedCostC, lvl) : null;
    const critChanceCost = lvl <= maxCritChanceLevel ? calcUpgradeCost(upgradeCostCritChance, critChanceCostC, lvl) : null;
    const critDamageCost = lvl <= maxCritDamageLevel ? calcUpgradeCost(upgradeCostCritDamage, critDamageCostC, lvl) : null;

    html += `
      <tr>
        <td style="text-align:center;">${lvl}</td>
        <td style="text-align:right;">${levelCost !== null ? levelCost.toFixed(2) : ''}</td>
        <td style="text-align:right;">${attackCost !== null ? attackCost.toFixed(2) : ''}</td>
        <td style="text-align:right;">${speedCost !== null ? speedCost.toFixed(2) : ''}</td>
        <td style="text-align:right;">${critChanceCost !== null ? critChanceCost.toFixed(2) : ''}</td>
        <td style="text-align:right;">${critDamageCost !== null ? critDamageCost.toFixed(2) : ''}</td>
      </tr>
    `;

    levels.push(lvl);
    costLevels.push(levelCost);
    costAttacks.push(attackCost);
    costSpeeds.push(speedCost);
    costCritChances.push(critChanceCost);
    costCritDamages.push(critDamageCost);
  }

  html += `
      </tbody>
    </table>
    <canvas id="upgradeChart" style="max-width: 100%; height: 400px;"></canvas>
  `;

  upgradeDiv.innerHTML = html;

  const ctx = document.getElementById('upgradeChart').getContext('2d');
  if (window.upgradeChartInstance) window.upgradeChartInstance.destroy();

  window.upgradeChartInstance = new Chart(ctx, {
    type: 'line',
    data: {
      labels: levels,
      datasets: [
        { label: '레벨업 비용', data: costLevels, borderColor: 'red', fill: false },
        { label: '공격력 비용', data: costAttacks, borderColor: 'blue', fill: false },
        { label: '공격속도 비용', data: costSpeeds, borderColor: 'orange', fill: false },
        { label: '크리티컬 확률 비용', data: costCritChances, borderColor: 'green', fill: false },
        { label: '크리티컬 데미지 비용', data: costCritDamages, borderColor: 'purple', fill: false }
      ]
    },
    options: {
      responsive: true,
      scales: {
        x: { title: { display: true, text: '레벨' } },
        y: { beginAtZero: true, title: { display: true, text: '비용' } }
      }
    }
  });
};
