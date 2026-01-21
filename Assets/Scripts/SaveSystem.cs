using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class CubeSaveData
{
    // Состояние каждого кубика
    public List<CubeletData> cubelets = new List<CubeletData>();

    // Статистика
    public int step;
    public int min;
    public int sec;
    public float elapsedTime;
    public float rotationSpeed;
    public string cubeStyle = "Standart";
    public string Data;
}

[System.Serializable]
public class CubeletData
{
    public float px, py, pz; // Position
    public float rx, ry, rz, rw; // Rotation (quaternion)
}

public class SaveSystem : MonoBehaviour
{
    [Header("Ссылки")]
    public CubeManager cubeManager;

    void Start()
    {
        // Автоматически находим CubeManager, если не присвоен
        if (cubeManager == null)
        {
            cubeManager = GetComponent<CubeManager>();
            if (cubeManager == null)
            {
                cubeManager = FindObjectOfType<CubeManager>();
            }
        }

        if (cubeManager == null)
        {
            Debug.LogError("SaveSystem: CubeManager not found!");
        }
    }

    // Сбор данных для сохранения
    private CubeSaveData CollectSaveData()
    {
        var saveData = new CubeSaveData();
        saveData.cubelets = new List<CubeletData>();

        // Получаем все кубики через публичное свойство
        List<float[]> allPieces = cubeManager.GetPozition();

        //if (allPieces == null || allPieces.Count == 0)
        //{
        //    Debug.LogError("Список кубиков пуст!");
        //    return saveData;
        //}

        foreach (float[] piece in allPieces)
        {
            if (piece == null) continue;
            saveData.cubelets.Add(new CubeletData
            {
                px = piece[0],
                py = piece[1],
                pz = piece[2],
                rx = piece[3],
                ry = piece[4],
                rz = piece[5],
                rw = piece[6],
            });
        }

        saveData.step = cubeManager.GetStep();
        saveData.min = cubeManager.GetTimeM();
        saveData.sec = cubeManager.GetTimeS();
        saveData.elapsedTime = cubeManager.GetEclipsedT();
        saveData.cubeStyle = cubeManager._style;
        saveData.rotationSpeed = cubeManager.rotationSpeed;
        saveData.Data = cubeManager.data;

        return saveData;
    }

    // Вызывается из HTML кнопки "Сохранить"
    public void SaveToJSON()
    {
        try
        {
            if (cubeManager == null)
            {
                Debug.LogError("CubeManager не найден!");
                return;
            }

            var saveData = CollectSaveData();

            if (saveData.cubelets.Count == 0)
            {
                Debug.LogError("Нет данных для сохранения!");
                return;
            }

            string json = JsonUtility.ToJson(saveData, true);

#if UNITY_WEBGL && !UNITY_EDITOR
            string fileName = $"RubikSave_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
            DownloadFile(fileName, json);
            ShowSaveSuccess("Сохранение успешно создано!");
#else
            string path = Path.Combine(Application.persistentDataPath, "save.json");
            File.WriteAllText(path, json);
            Debug.Log($"Успешно сохранено: {path}");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения: {e.Message}\n{e.StackTrace}");
#if UNITY_WEBGL && !UNITY_EDITOR
            ShowSaveError($"Ошибка сохранения: {e.Message}");
#endif
        }
    }

    // Загрузка из JSON строки
    public void LoadFromJSON(string json)
    {
        try
        {
            CubeSaveData saveData = JsonUtility.FromJson<CubeSaveData>(json);
            ApplySaveData(saveData);
            Debug.Log("Ебало будет бито");

#if UNITY_WEBGL && !UNITY_EDITOR
            ShowSaveSuccess("Сохранение загружено!");
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
#if UNITY_WEBGL && !UNITY_EDITOR
            ShowSaveError($"Ошибка загрузки: {e.Message}");
#endif
        }
    }

    // Применение сохраненных данных
    private void ApplySaveData(CubeSaveData saveData)
    {
        if (saveData.cubelets.Count != 27)
        {
            Debug.LogError($"Некорректные данные: {saveData.cubelets.Count} кубиков вместо 27");
            return;
        }


        List<float[]> allCubes = new List<float[]>();
        // Восстанавливаем позиции и вращения
        foreach(CubeletData c in saveData.cubelets) 
        {
            float[] cubelet = new float[7];
            cubelet[0] = c.px;
            cubelet[1] = c.py;
            cubelet[2] = c.pz;
            cubelet[3] = c.rx;
            cubelet[4] = c.ry;
            cubelet[5] = c.rz;
            cubelet[6] = c.rw;
            Debug.Log(cubelet[0]);
            allCubes.Add(cubelet);
        }

        cubeManager.RefreshCube(saveData.cubeStyle, allCubes);

        cubeManager.step = saveData.step;
        cubeManager.min = saveData.min;
        cubeManager.sec = saveData.sec;
        cubeManager._elapsedTime = saveData.elapsedTime;
        cubeManager.rotationSpeed = saveData.rotationSpeed;
        cubeManager._style = saveData.cubeStyle;

        Debug.Log(cubeManager._style);

        // ОБНОВЛЯЕМ UI - КРИТИЧЕСКИ ВАЖНО!
        // Вызываем публичные методы CubeManager для обновления JavaScript
        cubeManager.SendNumberInt(saveData.step, "Step");
        cubeManager.SendNumberFloat(saveData.rotationSpeed, "Speed");
        cubeManager.SendTime(saveData.min, saveData.sec);

        Debug.Log($"Сохранение загружено! Шаги: {saveData.step}, Время: {saveData.min}:{saveData.sec}");
    }

    // Автосохранение
    //void OnApplicationQuit()
    //{
    //    AutoSave();
    //}

    //void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus) AutoSave();
    //}

    //private void AutoSave()
    //{
    //    try
    //    {
    //        if (cubeManager != null)
    //        {
    //            var saveData = CollectSaveData();
    //            string json = JsonUtility.ToJson(saveData);
    //            PlayerPrefs.SetString("RubikCube_AutoSave", json);
    //            PlayerPrefs.Save();
    //            Debug.Log("Автосохранение выполнено");
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"Ошибка автосохранения: {e.Message}");
    //    }
    //}

    //// Автозагрузка при старте
    //public void LoadAutoSave()
    //{
    //    if (PlayerPrefs.HasKey("RubikCube_AutoSave"))
    //    {
    //        string json = PlayerPrefs.GetString("RubikCube_AutoSave");
    //        LoadFromJSON(json);
    //        Debug.Log("Автосохранение загружено");
    //    }
    //}

    // JavaScript функции
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string filename, string data);
    
    [DllImport("__Internal")]
    private static extern void TriggerFileUpload();
    
    [DllImport("__Internal")]
    private static extern void ShowSaveSuccess(string message);
    
    [DllImport("__Internal")]
    private static extern void ShowSaveError(string message);
#endif
}