mergeInto(LibraryManager.library, {
  DispatchUnityEvent: function (ptr) {
    const json = UTF8ToString(ptr);
    window.onUnityEvent(JSON.parse(json));
  }
});