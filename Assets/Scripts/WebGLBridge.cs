using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLBridge : MonoBehaviour
{
    public CubeManager cubeManager;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DispatchUnityEvent(string json);
#endif

    public static void SendEvent(string type, string payload)
    {
        EventMessage msg = new EventMessage
        {
            type = type,
            payload = payload
        };

        string json = JsonUtility.ToJson(msg);

#if UNITY_WEBGL && !UNITY_EDITOR
        DispatchUnityEvent(json);
#else
        Debug.Log("UNITY → JS: " + json);
#endif
    }

    [System.Serializable]
    private class EventMessage
    {
        public string type;
        public string payload;
    }

    // JS → Unity
    public void ReceiveEvent(string json)
    {
        EventMessage msg = JsonUtility.FromJson<EventMessage>(json);
        cubeManager.HandleJSEvent(msg.type, msg.payload);
    }
}
