using UnityEngine;

public class SpriteEditor : MonoBehaviour
{
    public float fillAmount = 0.5f;
    public Color fillColor = Color.green;
    public Color emptyColor = Color.white;

    private Texture2D writableTexture;
    private Color[] originalPixels;
    private int width;
    private int height;
    private float lastFillAmount = -1f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        //SetupExistingSprite();
    }
    public void OnSetUp()
    {
        SetupExistingSprite();
    }
    private void SetupExistingSprite()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        Sprite currentSprite = spriteRenderer.sprite;

        // CHỐT CHẶN 1: Phải dùng đúng kích thước vùng chọn của Sprite trong Texture
        Rect rect = currentSprite.rect;
        width = Mathf.RoundToInt(rect.width);
        height = Mathf.RoundToInt(rect.height);

        // CHỐT CHẶN 2: Lấy đúng vùng pixels (Tránh lệch do Atlas)
        originalPixels = currentSprite.texture.GetPixels(
            Mathf.RoundToInt(rect.x),
            Mathf.RoundToInt(rect.y),
            width,
            height
        );

        // Tạo texture mới cùng kích thước chính xác
        writableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        writableTexture.filterMode = FilterMode.Point;
        writableTexture.wrapMode = TextureWrapMode.Clamp;

        // CHỐT CHẶN 3: Thay thế Texture nhưng GIỮ NGUYÊN Pivot của Sprite cũ
        // Thay vì Sprite.Create phức tạp, ta tạo một Sprite đơn giản đè lên
        spriteRenderer.sprite = Sprite.Create(
            writableTexture,
            new Rect(0, 0, width, height),
            new Vector2(currentSprite.pivot.x / rect.width, currentSprite.pivot.y / rect.height),
            currentSprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect // Bắt buộc phải là FullRect trên Luna
        );

        ApplyVerticalFill();
    }

    public void TryApplyVerticalFill()
    {
        if (Mathf.Abs(fillAmount - lastFillAmount) > 0.001f)
            ApplyVerticalFill();
    }

    private void ApplyVerticalFill()
    {
        if (writableTexture == null || originalPixels == null) return;

        int filledRows = Mathf.RoundToInt(Mathf.Clamp01(fillAmount) * height);
        Color[] newPixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            bool isFilled = y < filledRows;
            int rowOffset = y * width;
            for (int x = 0; x < width; x++)
            {
                int index = rowOffset + x;
                Color src = originalPixels[index];

                if (src.a < 0.01f)
                {
                    newPixels[index] = Color.clear;
                    continue;
                }

                Color targetColor = isFilled ? fillColor : emptyColor;
                targetColor.a = src.a;
                newPixels[index] = targetColor;
            }
        }

        writableTexture.SetPixels(newPixels);
        writableTexture.Apply();
        lastFillAmount = fillAmount;
    }
}