using UnityEngine;
using TMPro;

public class TMPTextTyper : MonoBehaviour
{
    public string fullText;
    public float totalTime = 2f;

    private TextMeshProUGUI tmpText;
    private float timer;
    private int lastCharIndex;
    private string lastText;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        ResetTyping();
    }

    void Update()
    {
        if (fullText != lastText)
        {
            ResetTyping();
        }

        if (timer < totalTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / totalTime);
            int charCount = Mathf.FloorToInt(t * fullText.Length);

            if (charCount != lastCharIndex)
            {
                tmpText.text = fullText.Substring(0, charCount);
                lastCharIndex = charCount;
            }
        }
    }

    void ResetTyping()
    {
        timer = 0f;
        lastCharIndex = 0;
        lastText = fullText;
        tmpText.text = "";
    }
}
