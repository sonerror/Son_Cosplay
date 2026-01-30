using UnityEngine;

public class SpriteEditor : MonoBehaviour
{
    public float fillAmount = 0.5f;

    public Color fillColor = Color.green;

    public Color emptyColor = Color.white;

    private Texture2D writableTexture;
    private Color[] originalPixels;
    private int textureWidth;
    private int textureHeight;
    private float lastFillAmount = -1f;

    void Awake()
    {
        SetupExistingSprite();
    }
    private void SetupExistingSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.sprite == null)
        {
            return;
        }
        Texture2D sourceTexture = spriteRenderer.sprite.texture;
        if (!sourceTexture.isReadable)
        {
            return;
        }

        textureWidth = sourceTexture.width;
        textureHeight = sourceTexture.height;

        Rect spriteRect = spriteRenderer.sprite.rect;
        Vector2 spritePivot = spriteRenderer.sprite.pivot;
        float ppu = spriteRenderer.sprite.pixelsPerUnit;

        textureWidth = sourceTexture.width;
        textureHeight = sourceTexture.height;

        originalPixels = sourceTexture.GetPixels();

        writableTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        writableTexture.filterMode = FilterMode.Point;

        writableTexture.SetPixels(originalPixels);
        writableTexture.Apply();

        Vector2 normalizedPivot = new Vector2(
            spritePivot.x / textureWidth,
            spritePivot.y / textureHeight
        );
        Sprite newSprite = Sprite.Create(
            writableTexture,
            spriteRect,
            normalizedPivot,
            ppu,
            0,
            SpriteMeshType.FullRect
        );

        spriteRenderer.sprite = newSprite;
        ApplyVerticalFill();
    }

    public void TryApplyVerticalFill()
    {
        if (Mathf.Abs(fillAmount - lastFillAmount) > 0.001f)
        {
            ApplyVerticalFill();
        }
    }
    private void ApplyVerticalFill()
    {
        if (writableTexture == null || originalPixels == null) return;
        fillAmount = Mathf.Clamp01(fillAmount);
        int filledRows = Mathf.RoundToInt(fillAmount * textureHeight);
        Color[] newPixels = new Color[textureWidth * textureHeight];
        for (int y = 0; y < textureHeight; y++)
        {
            bool isFilledRow = (y < filledRows);
            for (int x = 0; x < textureWidth; x++)
            {
                int index = y * textureWidth + x;
                Color originalPixel = originalPixels[index];
                if (originalPixel.a < 0.001f)
                {
                    newPixels[index] = new Color(0, 0, 0, 0);
                    continue;
                }
                Color targetColor = isFilledRow ? fillColor : emptyColor;
                targetColor.a = originalPixel.a;
                newPixels[index] = targetColor;
            }
        }
        writableTexture.SetPixels(newPixels);
        writableTexture.Apply();
        lastFillAmount = fillAmount;
    }
}
