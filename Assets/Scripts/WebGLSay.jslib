mergeInto(LibraryManager.library, {
    DispatchUnityEvent: function (ptr) {
        const json = UTF8ToString(ptr);
        if (window.onUnityEvent) {
            window.onUnityEvent(JSON.parse(json));
        } else {
            console.warn("window.onUnityEvent is not defined. Make sure you have a listener for Unity events.");
        }
    },

    SendEventToUnity: function (type, payload) {
        const msg = {
            type: type,
            payload: payload
        };
        const json = JSON.stringify(msg);
        myGameInstance.SendMessage('WebGLBridge', 'ReceiveEvent', json);
    }
});
