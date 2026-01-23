var canvas = document.querySelector("#unity-canvas");
var unityInstance = null;

createUnityInstance(canvas, {
    dataUrl: "Build/WebGLBuild.data",
    frameworkUrl: "Build/WebGLBuild.framework.js",
    codeUrl: "Build/WebGLBuild.wasm",
    streamingAssetsUrl: "StreamingAssets",
    companyName: "DefaultCompany",
    productName: "KubikRubik",
    productVersion: "0.1",
}, (progress) => {
    // console.log("Loading... " + Math.round(progress * 100) + "%");
}).then((instance) => {
    unityInstance = instance;
});

window.onUnityEvent = function(e) {
    switch (e.type) {
        case "SET_TIME":
            const time = JSON.parse(e.payload);
            document.getElementById("time").innerText = `${time.min}:${time.sec.toString().padStart(2, '0')}`;
            break;
        case "SET_STEP":
            document.getElementById("steps").innerText = e.payload;
            break;
        case "WIN_GAME":
            alert("You won!");
            break;
    }
};

document.getElementById("theme-selector").addEventListener("change", (e) => {
    const theme = e.target.value;
    const msg = { type: "SET_THEME", payload: theme };
    unityInstance.SendMessage("WebGLEvent", "Command", JSON.stringify(msg));
});
