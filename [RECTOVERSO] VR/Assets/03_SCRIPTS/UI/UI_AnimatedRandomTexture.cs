using UnityEngine;
using UnityEngine.UI;

public class RandomSpriteChanger : MonoBehaviour
{
    public Sprite[] sprites;
    public float changeInterval = 2f;
    public bool invertColors;

    private Image imageComponent;
    private float timer;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        ChangeSprite();
        ApplyInversion();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            ChangeSprite();
            timer = 0f;
        }
    }

    void ChangeSprite()
    {
        if (sprites.Length == 0) return;
        int randomIndex = Random.Range(0, sprites.Length);
        imageComponent.sprite = sprites[randomIndex];
    }

    public void ApplyInversion()
    {
        if (invertColors)
        {
            imageComponent.material = new Material(Shader.Find("UI/InvertColor"));
        }
        else
        {
            imageComponent.material = null;
        }
    }
}
