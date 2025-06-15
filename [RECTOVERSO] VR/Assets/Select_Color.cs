using System.Collections.Generic;
using UnityEngine;

public class Select_Color : MonoBehaviour
{
    private GameObject target;
    private Color newColor;
    private GameObject palette;
    private ColorChange colorChange;
    void Start()
    {
        //target = pinceaux
        target = GameObject.Find("pCube2");
        palette = GameObject.Find("pPlane1");
        newColor = palette.GetComponent<Renderer>().material.GetColor("_" + gameObject.name);
        colorChange = gameObject.transform.parent.GetComponent<ColorChange>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pen"))
        {
            string indexStr = gameObject.name.Substring(5);

            if (int.TryParse(indexStr, out int index))
            {
                index -= 1;

                if (index >= 0 && index < colorChange.couleurs.Count)
                {
                    newColor = colorChange.couleurs[index];
                    other.transform.parent.GetComponent<Pencil02>().ChangeColor(newColor);
                }
            }
        }
    }
}