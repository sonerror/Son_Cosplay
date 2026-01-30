using UnityEngine;
using System.Collections;
using sonnv;
public class SnapVFX : SonMonoBehaviour
{
    [SerializeField] private float scaleBase;
    [SerializeField] private float cameraSizeBase;
    [SerializeField] private ParticleSystem particle;

    public void Show(MiniPool<SnapVFX> pooling)
    {
        float cameraSize = Camera.main.orthographicSize;
        float scaleFactor = cameraSize / cameraSizeBase;
        Tf.localScale = Vector3.one * (scaleBase * scaleFactor);

        StartCoroutine(WaitAnimEnd(pooling));
    }

    private IEnumerator WaitAnimEnd(MiniPool<SnapVFX> pooling)
    {
        if (particle != null)
        {
            yield return new WaitForSeconds(particle.main.duration);
        }
        else
        {
            yield return null;
        }

        pooling.Despawn(this);
    }
}
