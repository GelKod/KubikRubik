// ui.js — UI и модальные окна
let pendingColorChange = null;

function showWinModal(){
  const m=document.getElementById('win-modal');
  if(!m)return;
  m.style.display='flex';
  document.getElementById('win-time').textContent =
    document.getElementById('TimeM').textContent+':' +
    document.getElementById('TimeS').textContent;
  document.getElementById('win-steps').textContent =
    document.getElementById('Step').textContent;
  document.getElementById('win-speed').textContent =
    document.getElementById('Speed').textContent;
}

function closeWinModal(){const m=document.getElementById('win-modal');if(m)m.style.display='none'}
function restartGame(){closeWinModal();console.log('restart')}

function handleButtonClick(type){
  pendingColorChange=type;
  document.getElementById('warning-modal').style.display='block';
}
function closeWarning(){
  document.getElementById('warning-modal').style.display='none';
  pendingColorChange=null;
}
