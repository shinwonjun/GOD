const form = document.getElementById('attackForm');
const resultDiv = document.getElementById('result');
const downloadBtn = document.getElementById('downloadBtn');

const KILL_FOR_LEVELUP = 10;

function calcEnemyDefense(baseDef, level, defC, defE) {
  return baseDef * Math.pow(defE, defC * (level - 1));
}

function calcEnemyHP(baseHP, level, hpGrowthRate) {
  return baseHP * Math.pow(hpGrowthRate, level - 1);
}

function calcPlayerAttackRaw(baseAttack, playerLevel, cAtk) {
  return baseAttack * Math.log(playerLevel + 1) * cAtk;
}

function calcPlayerAttackAfterDefense(rawAttack, enemyDefense) {
  return rawAttack * (1 / (1 + enemyDefense / 100));
}

function calcPlayerCritChance(baseCritChance, playerLevel) {
  let crit = baseCritChance + playerLevel * 1;
  return crit > 100 ? 100 : crit;
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

function getFormData() {
  const formElements = form.querySelectorAll('input');
  const formData = {};
  formElements.forEach(el => {
    formData[el.id] = el.value;
  });
  return formData;
}

form.addEventListener('submit', function (event) {
  const id = event.submitter?.id;
  if (!id) return;

  event.preventDefault();
  const formData = getFormData();

  if (id === 'simulationBtn') {
    simulateBattle(formData, resultDiv);
	showUpgradeCosts(formData, resultDiv);	
  } else if (id === 'downloadBtn') {
    saveSimulationSettings();
  } else if (id === 'loadBtn') {
	document.getElementById('loadFileInput').click();
  }
});

setupSimulationLoadUI();