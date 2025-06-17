using UnityEngine;

public class StepPencil : MonoBehaviour
{
    private Renderer targetRenderer;
    public string propertyName1 = "_Intensity";
    public string propertyName2 = "_Scale";

    public float minValue = 0f;
    public float maxValue = 7f;
    public float interval = 0.5f; // toutes les 0.5 secondes

    private float timer = 0f;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if (targetRenderer == null) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            float valueI = Random.Range(minValue, maxValue);
            float valueS = Random.Range(minValue, maxValue);
            targetRenderer.material.SetFloat(propertyName1, valueI);
            targetRenderer.material.SetFloat(propertyName2, valueS);
            timer = 0f;
            Debug.Log($"Updated {propertyName1} to {valueI} and {propertyName2} to {valueS}");
        }
    }
}
