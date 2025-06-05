using UnityEngine;
using System.Collections;

public class SYSTEM_DRAWZONE : MonoBehaviour
{
    [Header("REFERENCES")]
    public GameObject fadeReference; //logiquement, une caméra ou le pinceau
    public Material lineMaterial;
    public Material drawCubeMaterial;

    [Header("SETTINGS")]
    public bool canDraw;
    public string pencilTag;
    public float fadeDistance = 2f;
    public float fullOpacityLineAlpha = 1f;
    public float fullOpacityDrawCubeAlpha = 0.1f;
    public float midOpacityLineAlpha = 0.5f;
    public float midOpacityDrawCubeAlpha = 0.025f;
    public float minOpacityLineAlpha = 0.1f;
    public float minOpacityDrawCubeAlpha = 0f;
    

    //PRIVATE VARIABLES
    public float distanceCheck;
    private bool fadeIn;
    private bool fadeOut;
    private bool triggerIn;

    void Start()
    {
        fadeReference = GameObject.FindGameObjectWithTag(pencilTag);
    }
    void Update()
    {
        distanceCheck = Vector3.Distance(fadeReference.transform.position, transform.position);
        if (distanceCheck < fadeDistance && fadeOut == false && triggerIn == false)
        {
            StopAllCoroutines();
            print("FULL OPACITY");
            StartCoroutine(ChangeMaterialAlpha(1f, lineMaterial, fullOpacityLineAlpha));
            StartCoroutine(ChangeMaterialAlpha(1f, drawCubeMaterial, fullOpacityDrawCubeAlpha));

            fadeOut = true;
            fadeIn = false;
        }
        else if (distanceCheck > fadeDistance && fadeIn == false && triggerIn == false)
        {
            StopAllCoroutines();
            print("MID OPACITY");
            StartCoroutine(ChangeMaterialAlpha(1f, lineMaterial, midOpacityLineAlpha));
            StartCoroutine(ChangeMaterialAlpha(1f, drawCubeMaterial, midOpacityDrawCubeAlpha));

            fadeIn = true;
            fadeOut = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == pencilTag)
        {
            canDraw = true;
            StopAllCoroutines();
            StartCoroutine(ChangeMaterialAlpha(1f, lineMaterial, minOpacityLineAlpha));
            StartCoroutine(ChangeMaterialAlpha(1f, drawCubeMaterial, minOpacityDrawCubeAlpha));

            triggerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == pencilTag)
        {
            canDraw = false;
            triggerIn = false;
        }
    }


    private IEnumerator ChangeMaterialAlpha(float Duration, Material material, float NewAlpha)
    
    {
        Color color = material.color;
        float startAlpha = color.a;
        float timeElapsed = 0f;

        while (timeElapsed < Duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, NewAlpha, timeElapsed / Duration);
            color.a = alpha;
            material.color = color;
            yield return null;
        }

        color.a = NewAlpha;
        material.color = color;
    }

}
