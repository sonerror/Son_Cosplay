using UnityEngine;
using sonnv;
using Utilities;
public class ControllerDoneVFX : MonoSingleton<ControllerDoneVFX>
{
    [SerializeField] private SnapVFX snapVfxPrefab;
    [SerializeField] private DoneVFX vfxPrefab;

    private readonly MiniPool<SnapVFX> _snapVfxPool = new();
    private readonly MiniPool<DoneVFX> _vfxPool = new();

    private void Start()
    {
        _snapVfxPool.OnInit(snapVfxPrefab, 5, transform);
        _vfxPool.OnInit(vfxPrefab, 5, transform);
    }

    [Sirenix.OdinInspector.Button]
    public void SpawnSnapVFX(Transform tfPosition, AudioClip vfxSound = null)
    {
        SnapVFX snapVfx = _snapVfxPool.Spawn();
        snapVfx.Tf.position = tfPosition.position;
        snapVfx.Show(_snapVfxPool);
        if (vfxSound) SoundManager.PlaySfx(vfxSound);
    }

    [Sirenix.OdinInspector.Button]
    public void SpawnVFX(Transform tfPosition)
    {
        DoneVFX vfx = _vfxPool.Spawn();
        vfx.Tf.position = tfPosition.position;
        vfx.Show(_vfxPool);
    }

}
