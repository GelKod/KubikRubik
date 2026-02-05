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
        WebGLEvent.SendEvent("SAVE_DATA", JsonUtility.ToJson(CollectSaveData(), true));
    }

    // Загрузка из JSON строки
    public void LoadFromJSON(string json)
    {
        CubeSaveData saveData = JsonUtility.FromJson<CubeSaveData>(json);
        ApplySaveData(saveData);
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
            allCubes.Add(cubelet);
        }

        cubeManager.RefreshCube(saveData.cubeStyle, allCubes);

        cubeManager.step = saveData.step;
        cubeManager.timerClass.min = saveData.min;
        cubeManager.timerClass.sec = saveData.sec;
        cubeManager._elapsedTime = saveData.elapsedTime;
        cubeManager.rotationSpeed = saveData.rotationSpeed;
        cubeManager._style = saveData.cubeStyle;

        WebGLEvent.SendEvent("SET_STEP", saveData.step.ToString());
        WebGLEvent.SendEvent("SET_SPEED", saveData.rotationSpeed.ToString());
        WebGLEvent.SendEvent("SET_TIME", cubeManager.timerClass.ToString());

        Debug.Log($"Сохранение загружено! Шаги: {saveData.step}, Время: {saveData.min}:{saveData.sec}");
    }
}