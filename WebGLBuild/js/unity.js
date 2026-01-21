// unity.js — Unity WebGL интеграция
function confirmColorChange(){
  if(pendingColorChange && window.unityInstance){
    unityInstance.SendMessage('Cube','CreateCube',pendingColorChange);
  }
  closeWarning();
}

function solveCube(){
  if(!window.unityInstance)return alert('Unity не загружен');
  unityInstance.SendMessage('Cube','SolveCube');
}

function updateSpeed(v){
  if(!window.unityInstance)return;
  unityInstance.SendMessage('Cube','WebGL_SetSpeed',parseFloat(v));
  document.getElementById('Speed').textContent=parseFloat(v).toFixed(1)+'x';
}

function setButtonsEnabled(e){
  ['solve-button','stats-button','pastel-button','standard-button',
   'neon-button','grey-button','speed-slider','save-button','load-button','export-stats-button']
   .forEach(id=>{const el=document.getElementById(id);if(el)el.disabled=!e});
}
