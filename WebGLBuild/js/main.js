// Весь JavaScript код остается без изменений
let pendingColorChange = null;

function showWinModal() {
const modal = document.getElementById('win-modal');
if (modal) {
modal.style.display = 'flex';

// Заполняем данные из игровой статистики
const timeM = document.getElementById('TimeM').textContent;
const timeS = document.getElementById('TimeS').textContent;
const steps = document.getElementById('Step').textContent;
const speed = document.getElementById('Speed').textContent;

document.getElementById('win-time').textContent = `${timeM}:${timeS}`;
document.getElementById('win-steps').textContent = steps;
document.getElementById('win-speed').textContent = speed;

// Устанавливаем текущую дату (вы можете заменить на свою дату позже)
//const currentDate = new Date();
//const formattedDate = currentDate.toLocaleDateString('ru-RU');
//document.getElementById('win-date').textContent = formattedDate;
}
}

function closeWinModal() {
const modal = document.getElementById('win-modal');
if (modal) {
modal.style.display = 'none';
}
}

function restartGame() {
closeWinModal();
// Ваш код для перезапуска игры
console.log('Игра перезапускается...');
}

function handleButtonClick(colorType) {
  pendingColorChange = colorType;
  document.getElementById('warning-modal').style.display = 'block';
}

function confirmColorChange() {
  if (pendingColorChange && window.unityInstance) {
    window.unityInstance.SendMessage('Cube', 'CreateCube', pendingColorChange);
  } else if (!window.unityInstance) {
    alert('Unity еще не загрузился! Пожалуйста, подождите.');
  }
  closeWarning();
  pendingColorChange = null;
}

function closeWarning() {
  document.getElementById('warning-modal').style.display = 'none';
  pendingColorChange = null;
}

function showStats() {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('Cube', 'GetStats');
    document.getElementById('stats-modal').style.display = 'block';
    updateStatsDisplay();
  } else {
    alert('Unity еще не загрузился! Пожалуйста, подождите.');
  }
}

function closeStats() {
  document.getElementById('stats-modal').style.display = 'none';
}

function updateStatsDisplay() {
  var statsContent = document.getElementById('stats-content');
  statsContent.textContent = 'Загрузка статистики...';
}

function solveCube() {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('Cube', 'SolveCube');
    var solveButton = document.getElementById('solve-button');
    solveButton.disabled = true;
    solveButton.innerHTML = '<span>⏳</span><span>Решение...</span>';
    
    setTimeout(function() {
      solveButton.disabled = false;
      solveButton.innerHTML = '<span>🎯</span><span>Решить кубик</span>';
    }, 30000);
  } else {
    alert('Unity еще не загрузился! Пожалуйста, подождите.');
  }
}

function exportStats() {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('Cube', 'ExportStatsToJSON');
  } else {
    alert('Unity еще не загрузился! Пожалуйста, подождите.');
  }
}

function updateSpeed(value) {
  if (window.unityInstance) {
    window.unityInstance.SendMessage('Cube', 'WebGL_SetSpeed', parseFloat(value));
    // Обновляем отображение скорости в статистике
    document.getElementById('Speed').textContent = parseFloat(value).toFixed(1) + 'x';
  }
}

function setButtonsEnabled(enabled) {
  document.getElementById('solve-button').disabled = !enabled;
  document.getElementById('stats-button').disabled = !enabled;
  document.getElementById('export-button').disabled = !enabled;
  document.getElementById('pastel-button').disabled = !enabled;
  document.getElementById('standard-button').disabled = !enabled;
  document.getElementById('neon-button').disabled = !enabled;
  document.getElementById('grey-button').disabled = !enabled;
  document.getElementById('speed-slider').disabled = !enabled;
}

var container = document.querySelector("#unity-container");
var canvas = document.querySelector("#unity-canvas");
var loadingBar = document.querySelector("#unity-loading-bar");
var progressBarFull = document.querySelector("#unity-progress-bar-full");
var fullscreenButton = document.querySelector("#unity-fullscreen-button");
var warningBanner = document.querySelector("#unity-warning");
var diagnostics_icon = document.getElementById("diagnostics-icon");

function unityShowBanner(msg, type) {
  function updateBannerVisibility() {
    warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
  }
  var div = document.createElement('div');
  div.innerHTML = msg;
  warningBanner.appendChild(div);
  if (type == 'error') div.style = 'background: red; padding: 10px;';
  else {
    if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
    setTimeout(function() {
      warningBanner.removeChild(div);
      updateBannerVisibility();
    }, 5000);
  }
  updateBannerVisibility();
}

var buildUrl = "Build";
var loaderUrl = buildUrl + "/WebGLBuild.loader.js";
var config = {
dataUrl: "Build/WebGLBuild.data",
frameworkUrl: "Build/WebGLBuild.framework.js",
codeUrl: "Build/WebGLBuild.wasm",
  streamingAssetsUrl: "StreamingAssets",
  companyName: "RubiksCube",
  productName: "Rubiks Cube 3D",
  productVersion: "0.1",
  showBanner: unityShowBanner,
};

if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
  var meta = document.createElement('meta');
  meta.name = 'viewport';
  meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
  document.getElementsByTagName('head')[0].appendChild(meta);
  container.className = "unity-mobile";
  canvas.className = "unity-mobile";

  diagnostics_icon.style.position = "fixed";
  diagnostics_icon.style.bottom = "10px";
  diagnostics_icon.style.right = "0px";
  canvas.after(diagnostics_icon);
} else {
  canvas.style.width = "100%";
  canvas.style.height = "100%";
}

loadingBar.style.display = "block";
setButtonsEnabled(false);

var script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
  createUnityInstance(canvas, config, (progress) => {
    progressBarFull.style.width = 100 * progress + "%";
  }).then((unityInstance) => {
    window.unityInstance = unityInstance;
    loadingBar.style.display = "none";
    setButtonsEnabled(true);
    
    diagnostics_icon.onclick = () => {
      unityDiagnostics.openDiagnosticsDiv(unityInstance.GetMemoryInfo);
    };
    fullscreenButton.onclick = () => {
      unityInstance.SetFullscreen(1);
    };
  }).catch((message) => {
    alert(message);
    setButtonsEnabled(true);
  });
};

document.body.appendChild(script);

// Функции для работы с файлами
function triggerSave() {
if (window.unityInstance) {
  window.unityInstance.SendMessage('SaveSystem', 'SaveToJSON');
}
}

function triggerLoad() {
// Создаем скрытый input для выбора файла
const input = document.createElement('input');
input.type = 'file';
input.accept = '.json';
input.style.display = 'none';

input.onchange = function(e) {
  const file = e.target.files[0];
  if (file) {
      const reader = new FileReader();
      reader.onload = function(e) {
          const content = e.target.result;
          // Отправляем в Unity
          if (window.unityInstance) {
              // Нужно передать как строку
              window.unityInstance.SendMessage('SaveSystem', 'LoadFromJSON', content);
          }
      };
      reader.readAsText(file);
  }
  document.body.removeChild(input);
};

document.body.appendChild(input);
input.click();
}

// Функция для скачивания файла (вызывается из Unity)
window.DownloadFile = function(filename, data) {
const blob = new Blob([data], { type: 'application/json' });
const url = URL.createObjectURL(blob);

const a = document.createElement('a');
a.href = url;
a.download = filename;
a.style.display = 'none';

document.body.appendChild(a);
a.click();

setTimeout(() => {
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}, 100);
};

// Функция для загрузки файла (вызывается из Unity)
window.TriggerFileUpload = function() {
triggerLoad();
};

function triggerSave() {
if (window.unityInstance) {
  window.unityInstance.SendMessage('SaveSystem', 'SaveToJSON');
} else {
  alert('Unity еще не загрузился! Пожалуйста, подождите.');
}
}

// function triggerLoad() {
//     if (window.unityInstance) {
//         window.unityInstance.SendMessage('SaveSystem', 'LoadFromFile');
//     } else {
//         alert('Unity еще не загрузился! Пожалуйста, подождите.');
//     }
// }

// Обновленная функция exportStats
function exportStats() {
if (window.unityInstance) {
  window.unityInstance.SendMessage('SaveSystem', 'SaveToJSON');
} else {
  alert('Unity еще не загрузился! Пожалуйста, подождите.');
}
}

// В функции setButtonsEnabled добавьте новые кнопки:
function setButtonsEnabled(enabled) {
// Существующие кнопки
document.getElementById('solve-button').disabled = !enabled;
document.getElementById('stats-button').disabled = !enabled;
//document.getElementById('export-button').disabled = !enabled;
document.getElementById('pastel-button').disabled = !enabled;
document.getElementById('standard-button').disabled = !enabled;
document.getElementById('neon-button').disabled = !enabled;
document.getElementById('grey-button').disabled = !enabled;
document.getElementById('speed-slider').disabled = !enabled;

// Новые кнопки сохранения/загрузки
const saveBtn = document.getElementById('save-button');
const loadBtn = document.getElementById('load-button');
const exportStatsBtn = document.getElementById('export-stats-button');

if (saveBtn) saveBtn.disabled = !enabled;
if (loadBtn) loadBtn.disabled = !enabled;
if (exportStatsBtn) exportStatsBtn.disabled = !enabled;
}
