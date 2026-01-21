using UnityEngine;
using System.Runtime.InteropServices;

public class SaveSystemBridge : MonoBehaviour
{
    public SaveSystem saveSystem;

    [DllImport("__Internal")]
    private static extern void ShowSaveSuccess(string message);

    [DllImport("__Internal")]
    private static extern void ShowSaveError(string message);

    // Вызывается из HTML кнопки "Сохранить"
    public void WebGL_SaveGame()
    {
        if (saveSystem != null)
        {
            saveSystem.SaveToJSON();

#if UNITY_WEBGL && !UNITY_EDITOR
                ShowSaveSuccess("Сохранение успешно создано!");
#endif
        }
    }

    // Вызывается из HTML кнопки "Загрузить"
    public void WebGL_LoadGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            TriggerFileUpload();
#endif
    }

    // Вызывается при успешной загрузке
    public void OnLoadComplete(string json)
    {
        if (saveSystem != null)
        {
            saveSystem.LoadFromJSON(json);

#if UNITY_WEBGL && !UNITY_EDITOR
                ShowSaveSuccess("Сохранение загружено!");
#endif
        }
    }

    [DllImport("__Internal")]
    private static extern void TriggerFileUpload();
}