// simulation-load.js

// JSON 데이터를 입력 요소에 채워 넣기
window.loadSimulationSettings = function loadSimulationSettings(jsonData) {
  Object.entries(jsonData).forEach(([tab, tabValues]) => {
    Object.entries(tabValues).forEach(([id, value]) => {
      const input = document.getElementById(id);
      if (input) {
        input.value = value;
      }
    });
  });
};

// 파일 선택 후 로딩 처리만 담당
window.setupSimulationLoadUI = function setupSimulationLoadUI(fileInputId = 'loadFileInput') {
  const fileInput = document.getElementById(fileInputId);
  if (!fileInput) return;

  fileInput.addEventListener('change', event => {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
      try {
        const jsonData = JSON.parse(e.target.result);
        loadSimulationSettings(jsonData);
        alert('설정이 성공적으로 불러와졌습니다!');
      } catch (err) {
        alert('파일을 불러오는 데 실패했습니다. JSON 형식을 확인해주세요.');
      }
    };
    reader.readAsText(file);
  });
};
