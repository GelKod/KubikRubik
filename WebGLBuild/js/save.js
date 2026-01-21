// save.js — сохранение / загрузка
function triggerSave(){
  if(window.unityInstance)
    unityInstance.SendMessage('SaveSystem','SaveToJSON');
}

function triggerLoad(){
  const i=document.createElement('input');
  i.type='file';i.accept='.json';i.style.display='none';
  i.onchange=e=>{
    const f=e.target.files[0]; if(!f)return;
    const r=new FileReader();
    r.onload=e=>unityInstance &&
      unityInstance.SendMessage('SaveSystem','LoadFromJSON',e.target.result);
    r.readAsText(f);
  };
  document.body.appendChild(i); i.click(); document.body.removeChild(i);
}

window.DownloadFile=function(name,data){
  const b=new Blob([data],{type:'application/json'});
  const a=document.createElement('a');
  a.href=URL.createObjectURL(b);a.download=name;a.click();
};
