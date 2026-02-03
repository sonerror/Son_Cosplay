using UnityEngine;

public class SpriteMaskFill : MonoBehaviour
{
    public Transform fillTransform;
    public float fillAmount = 0.5f;
    public float minY = -1.0f;
    public float maxY = 0.0f;

    private float lastFillAmount = -1f;

    public void UpdateFill()
    {
        if (fillTransform == null) return;
        float newY = Mathf.Lerp(minY, maxY, fillAmount);

        Vector3 pos = fillTransform.localPosition;
        pos.y = newY;
        fillTransform.localPosition = pos;

        lastFillAmount = fillAmount;
    }
    public void SetFill(float amount)
    {
        fillAmount = Mathf.Clamp01(amount);
        UpdateFill();
    }
}