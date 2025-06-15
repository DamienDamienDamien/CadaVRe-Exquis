using UnityEngine;
using System.Collections.Generic;

public class ColorChange : MonoBehaviour
{
    public Color nextColor;
    private GameObject palette;
    public float minH = 0f;
    public float maxH = 0f;
    public float minS = 0f;
    public float maxS = 0f;
    public float minV = 0f;
    public float maxV = 0f;

    public List<Color> couleurs = new List<Color>();


    void Start()
    {
        foreach (Transform child in transform)
        {
            palette = GameObject.Find("pPlane1");
            GenerateCouleurs();

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateCouleurs();
        }
    }

    void GenerateCouleurs()
    {
        couleurs.Clear();

        for (int i = 0; i < 5; i++)
        {
            float h = Random.Range(minH, maxH);
            float s = Random.Range(minS, maxS);
            float v = minV + i * ((maxV - minV) / 4f);

            Color color = Color.HSVToRGB(h, s, v, false);
            couleurs.Add(color);
        }

        int index = 0;
        foreach (Transform child in transform)
        {
            if (index < couleurs.Count)
            {
                palette.GetComponent<Renderer>().material.SetColor("_" + child.name, couleurs[index]);
                index++;
            }
        }
    }
}
