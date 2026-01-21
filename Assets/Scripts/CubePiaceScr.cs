using System.Collections.Generic;
using UnityEngine;

public class CubePiaceScr : MonoBehaviour
{
    public List<GameObject> Planes = new List<GameObject>();
    public GameObject Cube;

    public Material[] PastelCollors;
    public Material[] NeonCollors;
    public Material[] GreyCollors;
    public Material[] StandartCollors;

    public void SetColor(int x, int y, int z)
    {
        if (y == 0) Planes[0].SetActive(true);
        else if (y == -2) Planes[1].SetActive(true);

        if (z == 0) Planes[2].SetActive(true);
        else if (z == 2) Planes[3].SetActive(true);

        if (x == 0) Planes[4].SetActive(true);
        else if (x == -2) Planes[5].SetActive(true);
    }

    public void SetColor2(string type)
    {
        Material[] materials = null;
        switch (type)
        {
            case "Pastel": materials = PastelCollors; break;
            case "Standart": materials = StandartCollors; break;
            case "Neon": materials = NeonCollors; break;
            case "Grey": materials = GreyCollors; break;
            default: materials = StandartCollors; break;
        }

        for (int i = 0; i < 6 && i < materials.Length; i++)
        {
            if (Planes.Count > i)
                Planes[i].GetComponent<Renderer>().material = materials[i];
        }
        if (Cube != null && materials.Length > 6)
            Cube.GetComponent<Renderer>().material = materials[6];
    }

    public List<Color> GetActiveColors()
    {
        List<Color> colors = new List<Color>();
        for (int i = 0; i < Planes.Count; i++)
        {
            if (Planes[i].activeInHierarchy)
            {
                colors.Add(Planes[i].GetComponent<Renderer>().material.color);
            }
        }
        return colors;
    }
}