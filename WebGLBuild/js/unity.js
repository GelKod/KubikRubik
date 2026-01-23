let unityInstance = null;

createUnityInstance(document.querySelector("canvas"), config)
  .then(instance => {
    unityInstance = instance;
  });

function sendCommand(cmd, payload = "") {
  if (!unityInstance) return;
  unityInstance.SendMessage(
    "WebGLEvent",
    "Command",
    cmd + "|" + payload
  );
}

// Unity → JS
window.onUnityEvent = function (event) {
  console.log("Unity event:", event);

  switch (event.type) {

    case "SET_TIME": {
      const t = JSON.parse(event.payload);
      document.getElementById("TimeM").textContent = t.min;
      document.getElementById("TimeS").textContent = t.sec;
      break;
    }

    case "SET_STEP":
      document.getElementById("Step").textContent = event.payload;
      break;

    case "WIN_GAME":
      alert("ПОБЕДА!");
      break;
  }
};
