using System;
using System.Collections;
using System.Collections.Generic;
using HoangHH;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Costopia.Gameplay
{
  public class H3SlotPaintableTexture : MonoBehaviour
  {
    private const float ApplyCooldown = 0.1f;
    private const byte Opaque = 255;
    private const byte Transparent = 0;
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");
    private static readonly int MaskTexID = Shader.PropertyToID("_MaskTex");
    private static readonly int AlphaMultiplierID = Shader.PropertyToID("_AlphaMultiplier");

    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Material skeletonMaterial;
    [SerializeField] private Material paintableMaterialTemplate;

    [SerializeField][Range(16, 1024)] private int maskResolution = 32;

    [SerializeField][Range(1, 20)] private int waitProcessPerFrame = 5;

    [SerializeField] private bool hideOnStart = true;

    [SerializeField][HideInInspector] private SkeletonDataAsset skeletonDataAsset;

    //     [ValueDropdown("GetSlotNames", IsUniqueList = true, DropdownWidth = 300)]
    //     [SpineSlot(dataField: nameof(skeletonDataAsset))]
    // #if UNITY_EDITOR
    //     [OnValueChanged(nameof(OnSlotNameChanged))]
    // #endif
    [SerializeField]
    private string slotName;

    [SpineAttachment(false, slotField: nameof(slotName), dataField: nameof(skeletonDataAsset))]
    [SerializeField]
    private string attachmentName;

    private readonly Dictionary<int, List<Vector2Int>> _circleCache = new Dictionary<int, List<Vector2Int>>();

    private readonly HashSet<int> _dirtyPixels = new HashSet<int>();

    private readonly Queue<CircleDrawRequest> _drawQueue = new Queue<CircleDrawRequest>();
    private Coroutine _drawRoutine;
    private float _lastApplyTime;

    private SlotPaintableData _slotData;

    public Action<float> OnRatioChange;
    public Action OnTransitionDone;

    public SkeletonAnimation SkeletonAnimation => skeletonAnimation;
    public Material Material => _slotData?.cloneMaterial;
    public string SlotName => slotName;
    public string AttachmentName => attachmentName;
    public float MaskResolution => maskResolution;
    public bool IsTransitioning { get; private set; }
    public float RevealRatio => _slotData?.RevealRatio ?? 0f;

    private void Start()
    {
      ApplySlotReveal();
    }

    private void OnEnable()
    {
      if (_slotData != null && _drawRoutine == null)
        _drawRoutine = StartCoroutine(ProcessDrawQueue());
    }

    private void OnDisable()
    {
      if (_drawRoutine != null)
      {
        StopCoroutine(_drawRoutine);
        _drawRoutine = null;
      }

      _drawQueue.Clear();
    }

    [Button]
    public void FillAll(float timeFill = 0.3f, bool removeMaterial = false)
    {
      if (_slotData == null || IsTransitioning) return;
      StartCoroutine(TransitionMaskCoroutine(true, timeFill, removeMaterial, false));
    }

    [Button]
    public void ClearAll(float timeFill = 0.3f, bool removeMaterial = false, bool turnOffSlot = true)
    {
      if (_slotData == null || IsTransitioning) return;
      StartCoroutine(TransitionMaskCoroutine(false, timeFill, removeMaterial, turnOffSlot));
    }

    public void DisableSlot()
    {
      skeletonAnimation.Skeleton.SetAttachment(slotName, null);
    }

    public void EnableSlot()
    {
      skeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
    }

    private void ApplySlotReveal()
    {
      Slot slot = skeletonAnimation.Skeleton.FindSlot(slotName);
      if (slot == null)
      {
        Debug.LogWarning($"Slot '{slotName}' not found.");
        return;
      }

      if (slot.Attachment == null)
      {
        EnableSlot();
        if (slot.Attachment == null)
        {
          Debug.LogWarning($"Slot '{slotName}' has no attachment assigned.");
          return;
        }
      }

      // Create material and assign base texture + mask
      Material mat = new Material(paintableMaterialTemplate);
      mat.SetTexture(MainTexID, skeletonMaterial.mainTexture);
      mat.SetFloat(AlphaMultiplierID, 1f);
      skeletonAnimation.CustomSlotMaterials[slot] = mat;

      // Create blank mask texture
      Texture2D maskTex = new Texture2D(maskResolution, maskResolution, TextureFormat.Alpha8, false)
      {
        wrapMode = TextureWrapMode.Clamp,
        filterMode = FilterMode.Bilinear
      };
      var pixels = new Color32[maskResolution * maskResolution];
      byte alpha = hideOnStart ? Transparent : Opaque;
      for (int i = 0; i < pixels.Length; i++)
        pixels[i] = new Color32(0, 0, 0, alpha);
      maskTex.SetPixels32(pixels);
      maskTex.Apply(false, false);

      mat.SetTexture(MaskTexID, maskTex);

      _slotData = new SlotPaintableData
      {
        maskDirty = false,
        maskPixels = pixels,
        maskTexture = maskTex,
        cloneMaterial = mat,
        paintable = this
      };
      _slotData.InitializeActiveArea();
      _drawRoutine ??= StartCoroutine(ProcessDrawQueue());
    }

    public void DrawCircle(int cx, int cy, int radius, bool isErasing)
    {
      if (IsTransitioning || _slotData == null) return;
      _drawQueue.Enqueue(new CircleDrawRequest(cx, cy, radius, isErasing));
    }

    private IEnumerator ProcessDrawQueue()
    {
      WaitForSeconds idleWait = new WaitForSeconds(0.016f);
      while (true)
      {
        if (_drawQueue.Count == 0)
        {
          yield return idleWait;
          continue;
        }

        int processed = 0;
        while (_drawQueue.Count > 0 && processed < waitProcessPerFrame)
        {
          CircleDrawRequest request = _drawQueue.Dequeue();
          ProcessCircle(request.cx, request.cy, request.radius, request.isErasing);
          processed++;
        }

        if (_slotData.maskDirty && _dirtyPixels.Count > 0 && Time.time - _lastApplyTime >= ApplyCooldown)
        {
          _slotData.ApplyMask();
          _lastApplyTime = Time.time;
          OnRatioChange?.Invoke(_slotData.RevealRatio);
        }

        yield return null;
      }
    }

    private void ProcessCircle(int cx, int cy, int radius, bool isErasing)
    {
      if (!_circleCache.TryGetValue(radius, out var offsets))
      {
        offsets = new List<Vector2Int>();
        int r2 = radius * radius;
        for (int y = -radius; y <= radius; y++)
        {
          for (int x = -radius; x <= radius; x++)
          {
            if (x * x + y * y <= r2)
              offsets.Add(new Vector2Int(x, y));
          }
        }

        _circleCache[radius] = offsets;
      }

      bool changed = false;
      byte targetA = isErasing ? Transparent : Opaque;
      foreach (Vector2Int off in offsets)
      {
        int px = cx + off.x;
        int py = cy + off.y;
        if (px < 0 || px >= maskResolution || py < 0 || py >= maskResolution) continue;
        int idx = py * maskResolution + px;
        ref Color32 pix = ref _slotData.maskPixels[idx];
        if (pix.a != targetA)
        {
          pix.a = targetA;
          _dirtyPixels.Add(idx);
          changed = true;
        }
      }

      if (changed)
        _slotData.maskDirty = true;
    }

    private IEnumerator TransitionMaskCoroutine(bool fill, float duration, bool removeMaterial, bool turnOffSlot)
    {
      IsTransitioning = true;
      var pixels = _slotData.maskPixels;
      List<int> activePixels = new List<int>(pixels.Length);
      byte target = fill ? Opaque : Transparent;

      for (int i = 0; i < pixels.Length; i++)
        if (pixels[i].a != target)
          activePixels.Add(i);

      for (int i = activePixels.Count - 1; i > 0; i--)
      {
        int j = Random.Range(0, i + 1);
        (activePixels[i], activePixels[j]) = (activePixels[j], activePixels[i]);
      }

      float elapsed = 0;
      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        foreach (int i in activePixels)
        {
          byte a = (byte)Mathf.Lerp(pixels[i].a, target, t);
          pixels[i].a = a;
          _dirtyPixels.Add(i);
        }

        _slotData.maskDirty = true;
        _slotData.ApplyMask();
        yield return null;
      }

      for (int i = 0; i < pixels.Length; i++) pixels[i].a = target;
      _dirtyPixels.Clear();
      _slotData.maskDirty = true;
      _slotData.ApplyMask();

      IsTransitioning = false;
      OnTransitionDone?.Invoke();
      if (removeMaterial)
        // change material back to original
        skeletonAnimation.CustomSlotMaterials[skeletonAnimation.Skeleton.FindSlot(slotName)] = skeletonMaterial;

      if (turnOffSlot) DisableSlot();
    }

    private class SlotPaintableData
    {
      // Indices corresponding to *visible* (non-transparent) pixels of the slot's texture
      private List<int> _activeIndices;
      public Material cloneMaterial;
      public bool maskDirty;
      public Color32[] maskPixels;
      public Texture2D maskTexture;
      public H3SlotPaintableTexture paintable;
      public float RevealRatio { get; private set; }

      /// <summary>
      ///     Precompute which pixels in the mask correspond to the slot's visible (non-transparent) texture area.
      ///     Supports both RegionAttachment and MeshAttachment.
      /// </summary>
      public void InitializeActiveArea()
      {
        _activeIndices = new List<int>();

        Slot slot = paintable.SkeletonAnimation.Skeleton.FindSlot(paintable.SlotName);
        if (slot == null || slot.Attachment == null)
        {
          for (int i = 0; i < maskPixels.Length; i++)
            _activeIndices.Add(i);
          return;
        }

        // Determine UV range from the slot's attachment
        float minU = 1f, maxU = 0f, minV = 1f, maxV = 0f;
        float[] uvs = null;

        if (slot.Attachment is RegionAttachment regionAttachment)
        {
          uvs = regionAttachment.UVs;
          minU = Mathf.Min(uvs[0], uvs[2], uvs[4], uvs[6]);
          maxU = Mathf.Max(uvs[0], uvs[2], uvs[4], uvs[6]);
          minV = Mathf.Min(uvs[1], uvs[3], uvs[5], uvs[7]);
          maxV = Mathf.Max(uvs[1], uvs[3], uvs[5], uvs[7]);
        }
        else if (slot.Attachment is MeshAttachment meshAttachment)
        {
          uvs = meshAttachment.UVs;
          for (int i = 0; i < uvs.Length; i += 2)
          {
            float u = uvs[i];
            float v = uvs[i + 1];
            minU = Mathf.Min(minU, u);
            maxU = Mathf.Max(maxU, u);
            minV = Mathf.Min(minV, v);
            maxV = Mathf.Max(maxV, v);
          }
        }

        Texture2D mainTex = paintable.Material.GetTexture("_MainTex") as Texture2D;
        if (mainTex == null)
        {
          // fallback: assume fully opaque
          for (int i = 0; i < maskPixels.Length; i++)
            _activeIndices.Add(i);
          return;
        }

        int res = paintable.maskResolution;
        var basePixels = mainTex.GetPixels();

        int baseW = mainTex.width;
        int baseH = mainTex.height;

        for (int y = 0; y < res; y++)
          for (int x = 0; x < res; x++)
          {
            float u = (float)x / res;
            float v = (float)y / res;
            if (u < minU || u > maxU || v < minV || v > maxV)
              continue;

            // sample main texture alpha at this UV
            int texX = Mathf.Clamp((int)(u * baseW), 0, baseW - 1);
            int texY = Mathf.Clamp((int)(v * baseH), 0, baseH - 1);
            Color baseColor = basePixels[texY * baseW + texX];

            if (baseColor.a > 0.01f)
              _activeIndices.Add(y * res + x);
          }

        // fallback if no opaque pixels found (avoid divide by zero)
        if (_activeIndices.Count == 0)
          for (int i = 0; i < maskPixels.Length; i++)
            _activeIndices.Add(i);
      }

      public void ApplyMask()
      {
        if (!maskDirty || paintable._dirtyPixels.Count == 0) return;

        const int threshold = 512;
        if (paintable._dirtyPixels.Count < threshold)
        {
          foreach (int idx in paintable._dirtyPixels)
          {
            int x = idx % maskTexture.width;

            int y = idx / maskTexture.width;

            maskTexture.SetPixel(x, y, maskPixels[idx]);
          }
          maskTexture.Apply(false, false);
        }
        else
        {
          maskTexture.SetPixels32(maskPixels);
          maskTexture.Apply(false, false);
        }

        paintable._dirtyPixels.Clear();
        maskDirty = false;

        CalculateRevealRatio();
      }

      private void CalculateRevealRatio()
      {
        if (_activeIndices == null || _activeIndices.Count == 0) InitializeActiveArea();
        int total = _activeIndices?.Count ?? 0;
        if (total == 0)
        {
          RevealRatio = 0;
          return;
        }

        int visible = 0;
        for (int i = 0; i < total; i++)
        {
          if (maskPixels[_activeIndices[i]].a > 0)
            visible++;
        }

        RevealRatio = (float)visible / total;
      }
    }


    private struct CircleDrawRequest
    {
      public readonly int cx;
      public readonly int cy;
      public readonly int radius;
      public readonly bool isErasing;

      public CircleDrawRequest(int cx, int cy, int radius, bool isErasing)
      {
        this.cx = cx;
        this.cy = cy;
        this.radius = radius;
        this.isErasing = isErasing;
      }
    }

#if UNITY_EDITOR
    [Button]
    private void GetReferences()
    {
      skeletonAnimation = GetComponentInParent<SkeletonAnimation>();
      if (skeletonAnimation == null)
        Debug.LogWarning("SkeletonAnimation not found.");
      skeletonDataAsset = skeletonAnimation?.SkeletonDataAsset;
    }

    private void OnValidate()
    {
      skeletonDataAsset = skeletonAnimation != null ? skeletonAnimation.SkeletonDataAsset : null;
    }

    private IEnumerable<string> GetSlotNames()
    {
      if (skeletonAnimation == null)
        return new[] { "(Skeleton not assigned)" };

      Skeleton skeleton = skeletonAnimation.Skeleton;
      if (skeleton == null)
        return new[] { "(Skeleton not initialized)" };

      var names = new List<string>();
      for (int i = 0; i < skeleton.Slots.Count; i++)
        names.Add(skeleton.Slots.Items[i].Data.Name);
      return names;
    }

    private void OnSlotNameChanged()
    {
      if (string.IsNullOrEmpty(slotName) || skeletonDataAsset == null)
        return;

      SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
      SlotData slotData = skeletonData?.FindSlot(slotName);
      if (slotData == null) return;

      Skin skin = skeletonData.DefaultSkin;
      if (skin == null) return;

      var entries = new List<Skin.SkinEntry>();
      skin.GetAttachments(slotData.Index, entries);
      if (entries.Count > 0)
        attachmentName = entries[0].Name;
    }

    #region DEBUG

    [Header("Debug Gizmo")]
    [SerializeField]
    private bool showDebugGizmo = true;

    private readonly Color _gizmoUVColor = new Color(1f, 0f, 0f, 0.3f);
    private readonly Color _gizmoMaskColor = new Color(0f, 1f, 0f, 0.15f);
    private readonly Vector3 _gizmoLabelOffset = new Vector3(0, 0.5f, 0);

    private void OnDrawGizmosSelected()
    {
      if (!showDebugGizmo || !Application.isPlaying || _slotData == null)
        return;

      Slot slot = skeletonAnimation.Skeleton.FindSlot(slotName);
      if (slot?.Attachment == null)
        return;

      // Handle both Region and Mesh attachments
      if (slot.Attachment is RegionAttachment regionAttachment)
        DrawRegionAttachmentGizmo(regionAttachment, slot);
      else if (slot.Attachment is MeshAttachment meshAttachment)
        DrawMeshAttachmentGizmo(meshAttachment, slot);

      // Draw floating text label
      Handles.Label(
          skeletonAnimation.transform.position + _gizmoLabelOffset,
          $"Reveal: {RevealRatio * 100f:0.0}%",
          new GUIStyle
          {
            fontSize = 14,
            normal = new GUIStyleState { textColor = Color.yellow },
            alignment = TextAnchor.MiddleCenter
          });
    }

    private void DrawRegionAttachmentGizmo(RegionAttachment regionAttachment, Slot slot)
    {
      float[] verts = new float[8];
      regionAttachment.ComputeWorldVertices(slot, verts, 0);

      // Convert Spine coords to Unity world space
      Transform t = skeletonAnimation.transform;
      Vector3 p0 = t.TransformPoint(new Vector3(verts[0], verts[1], 0));
      Vector3 p1 = t.TransformPoint(new Vector3(verts[2], verts[3], 0));
      Vector3 p2 = t.TransformPoint(new Vector3(verts[4], verts[5], 0));
      Vector3 p3 = t.TransformPoint(new Vector3(verts[6], verts[7], 0));

      // Draw filled quad for mask
      Handles.color = _gizmoMaskColor;
      Handles.DrawAAConvexPolygon(p0, p1, p2, p3);

      // Draw outline (UV bounding box)
      Handles.color = _gizmoUVColor;
      Handles.DrawLine(p0, p1);
      Handles.DrawLine(p1, p2);
      Handles.DrawLine(p2, p3);
      Handles.DrawLine(p3, p0);
    }

    private void DrawMeshAttachmentGizmo(MeshAttachment meshAttachment, Slot slot)
    {
      int vertexCount = meshAttachment.WorldVerticesLength;
      float[] verts = new float[vertexCount];
      meshAttachment.ComputeWorldVertices(slot, 0, vertexCount, verts, 0);

      Transform t = skeletonAnimation.transform;
      int triCount = meshAttachment.Triangles.Length / 3;
      int[] triangles = meshAttachment.Triangles;

      Handles.color = _gizmoMaskColor;

      for (int i = 0; i < triCount; i++)
      {
        int i0 = triangles[i * 3 + 0] * 2;
        int i1 = triangles[i * 3 + 1] * 2;
        int i2 = triangles[i * 3 + 2] * 2;

        Vector3 p0 = t.TransformPoint(new Vector3(verts[i0], verts[i0 + 1], 0));
        Vector3 p1 = t.TransformPoint(new Vector3(verts[i1], verts[i1 + 1], 0));
        Vector3 p2 = t.TransformPoint(new Vector3(verts[i2], verts[i2 + 1], 0));

        Handles.DrawAAConvexPolygon(p0, p1, p2);
      }

      // Outline in red
      Handles.color = _gizmoUVColor;
      for (int i = 0; i < vertexCount; i += 2)
      {
        Vector3 p = t.TransformPoint(new Vector3(verts[i], verts[i + 1], 0));
        Handles.DotHandleCap(0, p, Quaternion.identity, 0.01f, EventType.Repaint);
      }
    }

    #endregion

#endif
  }
}