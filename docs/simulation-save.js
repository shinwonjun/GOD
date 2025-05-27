// simulation-save.js

function collectTabData(tabId) {
  const container = document.getElementById(tabId);
  const inputs = container.querySelectorAll('input');
  const tabData = {};
  inputs.forEach(input => {
    tabData[input.id] = input.value;
  });
  return tabData;
}

window.saveSimulationSettings = function saveSimulationSettings() {
  const data = {
    player: collectTabData('player'),
    resource: collectTabData('resource'),
    upgrade: collectTabData('upgrade'),
    enemy: collectTabData('enemy')
  };

  const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(data, null, 2));
  const dlAnchorElem = document.createElement('a');
  dlAnchorElem.setAttribute("href", dataStr);
  dlAnchorElem.setAttribute("download", "simulation_data.json");
  dlAnchorElem.click();
};
