using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLEvent : MonoBehaviour
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
        Debug.Log("EVENT → " + json);
#endif
    }

    [System.Serializable]
    private class EventMessage
    {
        public string type;
        public string payload;
    }

    // JS → Unity
    public void Command(string command)
    {
        var parts = command.Split('|');
        var cmd = parts[0];
        var payload = parts.Length > 1 ? parts[1] : "";

        switch (cmd)
        {
            case "SET_SPEED":
                cubeManager.SetSpeed(float.Parse(payload));
                break;

            case "SHUFFLE":
                cubeManager.WebGL_Shuffle();
                break;

            case "RESET":
                cubeManager.CreateCube();
                break;
        }
    }
}
