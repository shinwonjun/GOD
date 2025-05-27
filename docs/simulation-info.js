// 전략별 상태 저장용 객체
const strategyValues = {
  balanced: {
	level: 0,
    attack: 40,
    critChance: 20,
    critDamage: 20,
    speed: 20,
    backup: {}
  },
  dps: {
	level: 0,
    attack: 60,
    critChance: 10,
    critDamage: 0,
    speed: 30,
    backup: {}
  },
  crit: {
	level: 0,
    attack: 20,
    critChance: 50,
    critDamage: 30,
    speed: 0,
    backup: {}
  }
};

// 현재 선택된 전략
let currentStrategy = "balanced";

// 전략 선택 시 실행
function showStrategyInputs(selected) {
  // 먼저 현재 전략의 입력값을 저장
  saveCurrentStrategyInputs();

  // 새 전략 선택
  currentStrategy = selected;

  // 모든 전략 입력창 숨김
  ['balanced', 'dps', 'crit'].forEach(type => {
    document.getElementById(type + 'Inputs').style.display = (type === selected) ? 'block' : 'none';
  });

  // 새 전략 입력값 불러오기
  loadStrategyInputs(selected);

  // 유효성 검사
  validateStrategySum(selected);
}

// 현재 입력값을 해당 전략 객체에 저장
function saveCurrentStrategyInputs() {
  const form = strategyValues[currentStrategy];
  form.level = getNumber(`${currentStrategy}LevelUp`);
  form.attack = getNumber(`${currentStrategy}Attack`);
  form.critChance = getNumber(`${currentStrategy}CritChance`);
  form.critDamage = getNumber(`${currentStrategy}CritDamage`);
  form.speed = getNumber(`${currentStrategy}Speed`);
}

// 선택된 전략의 값을 입력창에 반영
function loadStrategyInputs(strategy) {
  const form = strategyValues[strategy];
  document.getElementById(`${strategy}LevelUp`).value = form.level;
  document.getElementById(`${strategy}Attack`).value = form.attack;
  document.getElementById(`${strategy}CritChance`).value = form.critChance;
  document.getElementById(`${strategy}CritDamage`).value = form.critDamage;
  document.getElementById(`${strategy}Speed`).value = form.speed;
}

// 총합 검증
function validateStrategySum(type) {
  const sum =
    getNumber(`${type}LevelUp`) +
    getNumber(`${type}Attack`) +
    getNumber(`${type}CritChance`) +
    getNumber(`${type}CritDamage`) +
    getNumber(`${type}Speed`);
  const warning = document.getElementById('strategyWarning');
  warning.style.display = sum > 100 ? 'block' : 'none';
}

// 숫자 안전 추출 함수
function getNumber(id) {
  return parseFloat(document.getElementById(id).value) || 0;
}

// 초기화 시 리스너 등록
function initStrategyInputs() {
   ['balanced', 'dps', 'crit'].forEach(type => {
    ['LevelUp', 'Attack', 'CritChance', 'CritDamage', 'Speed'].forEach(attr => {
      const id = `${type}${attr}`;
      const el = document.getElementById(id);

      // 입력 이전 값 저장
      el.addEventListener('focus', () => {
        strategyValues[type].backup[attr] = el.value;
      });

      // 입력 후 검증
      el.addEventListener('input', () => {
        if (currentStrategy !== type) return;

        const total =
          getNumber(`${type}LevelUp`) +
          getNumber(`${type}Attack`) +
          getNumber(`${type}CritChance`) +
          getNumber(`${type}CritDamage`) +
          getNumber(`${type}Speed`);

        const warning = document.getElementById('strategyWarning');

        if (total > 100) {
          // 롤백
          el.value = strategyValues[type].backup[attr] || 0;
          warning.style.display = 'block';
        } else {
          warning.style.display = 'none';
          strategyValues[type][attr.toLowerCase()] = parseFloat(el.value) || 0;
        }
      });
    });
  });
}

// 실행
window.addEventListener('DOMContentLoaded', initStrategyInputs);
