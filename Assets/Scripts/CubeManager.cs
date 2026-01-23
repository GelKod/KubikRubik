using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeManager : MonoBehaviour
{
    public GameObject CubePrefab;
    private Transform cubeRoot;
    public List<GameObject> allPieces = new List<GameObject>();
    private GameObject centerPiece;
    private bool canRotate = true;
    private bool canTimer = false;
    [SerializeField] public float rotationSpeed = 1f;
    [SerializeField] public int step = 0;
    public TimerClass timerClass;
    [SerializeField] public float _elapsedTime = 0f;
    [SerializeField] public List<float[]> _cubes = new List<float[]>();
    [SerializeField] public string _style = "Standart";
    public SaveSystem saveSystem;
    public string data = DateTime.Now.ToString();
    int lastSec = -1;

    void Start()
    {
        cubeRoot = transform;
        CreateCube("Standart");
    }

    void Update()
    {
        if (canRotate)
        {
            CheckKeyboardInput();
        }
        if (canTimer)
        {
            Timer();
            CheckSolved();
        }
    }

    public int GetTimeS()
    {
        return timerClass.sec;
    }
    public int GetTimeM()
    {
        return timerClass.min;
    }
    public int GetStep()
    {
        return step;
    }
    public float GetEclipsedT()
    {
        return _elapsedTime;
    }
    public List<float[]> GetPozition()
    {
        foreach (var piece in allPieces)
        {
            float[] _coordsCubs = new float[7];
            _coordsCubs[0] = piece.transform.position.x;
            _coordsCubs[1] = piece.transform.position.y;
            _coordsCubs[2] = piece.transform.position.z;
            _coordsCubs[3] = piece.transform.localRotation.x;
            _coordsCubs[4] = piece.transform.localRotation.y;
            _coordsCubs[5] = piece.transform.localRotation.z;
            _coordsCubs[6] = piece.transform.localRotation.w;
            _cubes.Add(_coordsCubs);
        }
        return _cubes;
    }
    public void SetSpeed(float speed)
    {
        rotationSpeed=speed;
    }
    public void RefreshCube(string style, List<float[]> allCubes)
    {
        // Удаляем старые кубики
        foreach (GameObject go in allPieces)
            DestroyImmediate(go);
        allPieces.Clear();
        ClearTimer();
        step = 0;
        _style = style;

        // Создаём новый кубик 3x3x3
        foreach (float[] coords in allCubes)
        {
            GameObject piece = Instantiate(CubePrefab, cubeRoot, false);
            piece.transform.localPosition = new Vector3(coords[0], coords[1], coords[2]);
            piece.transform.localRotation = new Quaternion(coords[3], coords[4], coords[5], coords[6]);
            var pieceScript = piece.GetComponent<CubePiaceScr>();
            if (pieceScript != null)
            {
                pieceScript.SetColor2(_style);
            }
            allPieces.Add(piece);
        }
        centerPiece = allPieces[13]; // центр кубика для вращения
    }
    void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(RotateFace(GetFacePieces(0), Vector3.up));
        else if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(RotateFace(GetFacePieces(1), Vector3.down));
        else if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(RotateFace(GetFacePieces(2), Vector3.back));
        else if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(RotateFace(GetFacePieces(3), Vector3.forward));
        else if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(RotateFace(GetFacePieces(4), Vector3.right));
        else if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(RotateFace(GetFacePieces(5), Vector3.left));
        else if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(ShuffleCube());
        else if (Input.GetKeyDown(KeyCode.E)) CreateCube(); // сброс
    }

    public void CreateCube(string style = "Standart")
    {
        // Удаляем старые кубики
        foreach (GameObject go in allPieces)
            DestroyImmediate(go);
        allPieces.Clear();
        ClearTimer();
        step = 0;
        _style = style;

        // Создаём новый кубик 3x3x3
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    GameObject piece = Instantiate(CubePrefab, cubeRoot, false);
                    piece.transform.localPosition = new Vector3(-x, -y, z);
                    var pieceScript = piece.GetComponent<CubePiaceScr>();
                    if (pieceScript != null)
                    {
                        pieceScript.SetColor(-x, -y, z);
                        pieceScript.SetColor2(_style);
                    }
                    allPieces.Add(piece);
                }
            }
        }
        centerPiece = allPieces[13]; // центр кубика для вращения
    }

    List<GameObject> GetFacePieces(int faceIndex)
    {
        return faceIndex switch
        {
            0 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.y) == 0),     // Up
            1 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.y) == -2),  // Down
            2 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.z) == 0),   // Left
            3 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.z) == 2),   // Right
            4 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.x) == 0),   // Front
            5 => allPieces.FindAll(p => Mathf.Round(p.transform.localPosition.x) == -2),  // Back
            _ => new List<GameObject>()
        };
    }

    IEnumerator ShuffleCube()
    {
        canRotate = false;
        int moves = Random.Range(15, 30);
        System.Random rnd = new System.Random();

        for (int i = 0; i < moves; i++)
        {
            int face = rnd.Next(6);
            bool clockwise = rnd.NextDouble() > 0.5;
            Vector3 axis = face switch
            {
                0 => Vector3.up,
                1 => Vector3.down,
                2 => Vector3.back,
                3 => Vector3.forward,
                4 => Vector3.right,
                5 => Vector3.left,
                _ => Vector3.zero
            };
            yield return StartCoroutine(RotateFace(GetFacePieces(face), clockwise ? axis : -axis));
            yield return new WaitForSeconds(0.15f);
        }

        canRotate = true;
    }

    IEnumerator RotateFace(List<GameObject> pieces, Vector3 axis, float angle = 90f)
    {
        if (pieces == null || pieces.Count == 0 || axis == Vector3.zero) yield break;

        canRotate = false;
        float rotated = 0f;
        float step = 5f;

        while (rotated < angle)
        {
            foreach (var piece in pieces)
            {
                piece.transform.RotateAround(centerPiece.transform.position, axis, step);
            }
            rotated += step;
            yield return new WaitForSeconds(0.01f / rotationSpeed);
        }

        // Доводим до точных 90 градусов (чтобы не накапливалась ошибка)
        float remaining = angle - rotated;
        if (remaining > 0.1f)
        {
            foreach (var piece in pieces)
            {
                piece.transform.RotateAround(centerPiece.transform.position, axis, remaining);
            }
        }
        this.step++;
        WebGLEvent.SendEvent("SET_STEP", this.step.ToString());
        canTimer = true;
        canRotate = true;
    }

    // Вызывается из CameraMovement или MouseInput при свайпе/выделении двух кубиков
    public void DetectRotate(List<GameObject> pieces, List<GameObject> planes, Vector3 mouseDirection)
    {
        if (!canRotate || pieces == null || pieces.Count != 2)
            return;

        Vector3 pos1 = pieces[0].transform.localPosition;
        Vector3 pos2 = pieces[1].transform.localPosition;

        // Определяем направление свайпа
        bool horizontalDrag = Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.y);
        float mainDrag = horizontalDrag ? mouseDirection.x : mouseDirection.y;
        if (Mathf.Abs(mainDrag) < 10f) return; // порог чувствительности

        List<GameObject> facePieces = null;
        Vector3 rotationAxis = Vector3.zero;

        // === Определяем грань по координатам двух кубиков ===

        // Верх / Низ (Y одинаковый)
        if (Mathf.Round(pos1.y) == Mathf.Round(pos2.y))
        {
            float y = Mathf.Round(pos1.y);
            if (y == 0) // Верхняя грань
            {
                facePieces = GetFacePieces(0); // Up
                rotationAxis = mainDrag > 0 ? Vector3.up : Vector3.down;
            }
            else if (y == -2) // Нижняя грань
            {
                facePieces = GetFacePieces(1); // Down
                rotationAxis = mainDrag > 0 ? Vector3.down : Vector3.up;
            }
        }
        // Фронт / Бэк (X одинаковый)
        else if (Mathf.Round(pos1.x) == Mathf.Round(pos2.x))
        {
            float x = Mathf.Round(pos1.x);
            if (x == 0) // Фронт
            {
                facePieces = GetFacePieces(4); // Front
                rotationAxis = mainDrag > 0 ? Vector3.right : Vector3.left;
            }
            else if (x == -2) // Бэк
            {
                facePieces = GetFacePieces(5); // Back
                rotationAxis = mainDrag > 0 ? Vector3.left : Vector3.right;
            }
        }
        // Лево / Право (Z одинаковый)
        else if (Mathf.Round(pos1.z) == Mathf.Round(pos2.z))
        {
            float z = Mathf.Round(pos1.z);
            if (z == 0) // Левая грань
            {
                facePieces = GetFacePieces(2); // Left
                rotationAxis = mainDrag > 0 ? Vector3.back : Vector3.forward;
            }
            else if (z == 2) // Правая грань
            {
                facePieces = GetFacePieces(3); // Right
                rotationAxis = mainDrag > 0 ? Vector3.forward : Vector3.back;
            }
        }

        if (facePieces != null && rotationAxis != Vector3.zero)
        {
            StartCoroutine(RotateFace(facePieces, rotationAxis));
        }
    }

    [System.Serializable]
    public class TimerClass
    {
        public int min;
        public int sec;
    }

    void Timer()
    {
        _elapsedTime += Time.deltaTime;

        int min = Mathf.FloorToInt(_elapsedTime / 60);
        int sec = Mathf.FloorToInt(_elapsedTime % 60);

        if (sec != lastSec)
        {
            lastSec = sec;
            timerClass = new TimerClass { min = min, sec = sec };
            WebGLEvent.SendEvent("SET_TIME", JsonUtility.ToJson(timerClass));
        }
    }


    void ClearTimer()
    {
        _elapsedTime = 0f;
        timerClass = null;
        canTimer = false;
    }

    // Методы для вызова из HTML (если нужно)
    public void WebGL_CreateCube(string style) => CreateCube(style);
    public void WebGL_Shuffle() { if (canRotate) StartCoroutine(ShuffleCube()); }

    public void CheckSolved()
    {
        // Проверяем, совпадает ли текущее состояние с начальным
        for (int i = 0; i < allPieces.Count; i++)
        {
            Vector3 currentPos = allPieces[i].transform.localPosition;
            Quaternion currentRot = allPieces[i].transform.localRotation;

            // Начальные позиции кубиков (как в CreateCube)
            int x = i / 9;
            int y = (i % 9) / 3;
            int z = i % 3;
            Vector3 targetPos = new Vector3(-x, -y, z);
            Quaternion targetRot = Quaternion.identity;

            // Сравниваем позицию и вращение с точностью до 0.1
            if (Vector3.Distance(currentPos, targetPos) > 0.1f ||
                Quaternion.Angle(currentRot, targetRot) > 0.1f)
            {
                return; // Кубик не собран
            }


        }
        saveSystem.SaveToJSON();
        // Если все кубики на своих местах
        Debug.Log("Победа! Кубик Рубика собран!");
        canTimer = false; // Останавливаем таймер

        WebGLEvent.SendEvent("WIN_GAME","");

        CreateCube();
    }

    public void HandleJSEvent(string type, string payload)
    {
        switch (type)
        {
            case "SET_SPEED":
                if (float.TryParse(payload, out float speed))
                {
                    SetSpeed(speed);
                }
                break;
            case "SHUFFLE":
                WebGL_Shuffle();
                break;
            case "RESET":
                CreateCube();
                break;
            case "LOAD_DATA":
                saveSystem.LoadFromJSON(payload);
                break;
        }
    }
}
