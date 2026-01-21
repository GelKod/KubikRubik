mergeInto(LibraryManager.library, {
  SendTimeMToJS: function (val) {
    document.getElementById("TimeM").innerText = val;
  },
  SendTimeSToJS: function (val) {
    document.getElementById("TimeS").innerText = val;
  },
  SendStepToJS: function (val) {
    document.getElementById("Step").innerText = val;
  },
  SendSpeedToJS: function (val) {
    document.getElementById("Speed").innerText = val;
  },
  SendDataToJS: function (val) {
    document.getElementById("Data").innerText = val;
  }
});