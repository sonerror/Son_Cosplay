using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sonnv;

public class DoneVFX : SonMonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private List<Sprite> doneSprites;
    [SerializeField] private Animation anim;
    [SerializeField] private float scaleBase;
    [SerializeField] private float cameraSizeBase;

    public void Show(MiniPool<DoneVFX> pooling)
    {
        float cameraSize = Camera.main.orthographicSize;
        float scaleFactor = cameraSize / cameraSizeBase;
        transform.localScale = Vector3.one * scaleBase * scaleFactor;

        if (doneSprites != null && doneSprites.Count > 0)
        {
            Sprite sprite = doneSprites[Random.Range(0, doneSprites.Count)];
            sr.sprite = sprite;
        }

        if (anim != null)
        {
            anim.Play();
            StartCoroutine(WaitAnimEnd(anim.clip.length, pooling));
        }
        else
        {
            pooling.Despawn(this);
        }
    }

    private IEnumerator WaitAnimEnd(float duration, MiniPool<DoneVFX> pooling)
    {
        yield return new WaitForSeconds(duration);
        pooling.Despawn(this);
    }
}
