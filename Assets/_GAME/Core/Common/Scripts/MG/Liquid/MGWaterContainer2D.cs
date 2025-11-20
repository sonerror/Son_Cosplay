using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Creates an illusion of 2D water level reflecting the correct volume of the container, as it rotates. Assumming the container is a rectangle.
/// </summary>
public class MGWaterContainer2D : MonoBehaviour
{
  public UnityEvent onEmpty;
  public bool flyOutOnEmpty;
  public GameObject visualContainer;

  public ParticleSystem fxSplash;

  public SpriteRenderer waterRenderer;

  public TextMeshPro txtWaterAmount;

  public Transform center;

  // [Tooltip("Container's wall layer")]
  public LayerMask wallLayer = 1 << 30;

  public Transform waterFlowContainer;
  public Vector2 flowSpawnOffset;

  public Transform water;
  public Transform container;

  public float maxWaterLevel = 0.95f; // the maximum water level acceptable
  public float containerWidth = 1.33f; // the width of the container (max water X)
  public float containerHeight = 2.4f; // the height of the container (max water Y)

  public float containerArea = 1; // the area of the container, auto calculated
  private float containerDiagonal;

  [Range(0, 1)]
  public float waterLevel = 0.25f; // the current water level, 0 is empty, 1 is full

  public float pourSpeed = 1f; // amount per second

  public bool pourLeft = true; // rotate left or right when pouring

  public float WaterAmount
  {
    get { return containerArea * waterLevel; }
    set
    {
      if (LevelBase.Instance.isEndingGame) return;
      waterLevel = Mathf.Clamp(value / containerArea, 0, 1);
      if (waterLevel <= 0) OnEmpty();
      UpdateWaterLevel();
    }
  }

  public void OnEmpty()
  {
    onEmpty?.Invoke();
  }

  private Vector3 oriPos;

  public MGWaterFlow2D waterFlowPrefab; // to be spawned every time a new water flow is poured

  protected virtual void Awake()
  {
    if (center == null) center = transform;
    oriPos = transform.position;
    containerArea = containerWidth * containerHeight;
    containerDiagonal = Mathf.Sqrt(containerWidth * containerWidth + containerHeight * containerHeight) / 2;
  }

  protected virtual void Start()
  {

  }


  public Transform openEdge; // if the water Y is higher than this, it can pour out
  private float minRotateSpeed = 15;

  public bool IsWaterReachedEdge()
  {
    return water.position.y >= openEdge.position.y;
  }

  [SerializeField] private float rotateSpeed = 5; // will be ramped up to 360 over time

  protected virtual void Update()
  {
    if (isPouring)
    {
      if (WaterAmount > 0 && receiveContainer != null && receiveContainer.waterLevel <= receiveContainer.maxWaterLevel)
      {
        if (!IsWaterReachedEdge())
        {
          // rotate the container until the water reaches the edge
          rotateSpeed = minRotateSpeed + 360 * Mathf.Clamp(openEdge.position.y - water.position.y, 0, containerDiagonal) / containerDiagonal;
          transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, pourLeft ? 170 : -170), Time.deltaTime * rotateSpeed);
        }
        else
        {
          // keep rotating at minimum speed
          transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, pourLeft ? 170 : -170), Time.deltaTime * rotateSpeed);

          // spawn a new water flow
          if (currentWaterFlow == null)
          {
            currentWaterFlow = Instantiate(waterFlowPrefab, (Vector2)waterFlowContainer.position + flowSpawnOffset, Quaternion.identity);
            currentWaterFlow.sourceContainer = this;
            currentWaterFlow.targetContainer = receiveContainer;
          }
          else
          {
            // if the water amount of the water flow plus the receving container's water level is greater or equal to 1, stop pouring
            if (currentWaterFlow.WaterAmount + receiveContainer.WaterAmount >= receiveContainer.containerArea * receiveContainer.maxWaterLevel)
            {
              StopPour();
            }
          }
        }
      }
      else
      {
        StopPour();
      }
    }
    UpdateWaterLevel();
  }

  public void UpdateWaterLevel()
  {
    if (waterLevel <= 0)
    {
      //water.gameObject.SetActive(false);
      waterRenderer.enabled = false;
      if (txtWaterAmount != null) txtWaterAmount.text = "";
      return;
    }
    waterRenderer.enabled = true;
    if (txtWaterAmount != null) txtWaterAmount.text = WaterAmount.ToString("F2") + " L";
    AdjustWaterLevel(container.GetRotationZ());
  }

  private float rotationRatio;
  private void AdjustWaterLevel(float z)
  {
    float disToBtm = CalculateDistanceToBtmEdge();
    float delta = containerHeight / 2f - disToBtm;

    rotationRatio = Mathf.Abs(z) / 90;
    float relativeHeight = Mathf.Lerp(containerHeight, containerWidth, rotationRatio) * waterLevel;
    Vector3 newPos = new Vector3(0, relativeHeight + delta, water.localPosition.z);
    water.localPosition = Vector3.Lerp(water.localPosition, newPos, Time.deltaTime * 20); // smooth the water level
  }

  Vector2 debugHit;

  public float CalculateDistanceToBtmEdge()
  {
    RaycastHit2D hit = Physics2D.Raycast(center.position, Vector2.down, 14, wallLayer);
    if (hit.collider != null)
    {
      debugHit = hit.point;
      return hit.distance;
    }
    else
    {
      debugHit = Vector2.zero;
    }
    return 0;
  }

  public bool isPouring;

  private MGWaterContainer2D receiveContainer;
  private MGWaterFlow2D currentWaterFlow;

  public void Pour(Vector3 targetPos, MGWaterContainer2D toContainer)
  {
    if (waterLevel <= 0 || toContainer.waterLevel >= toContainer.maxWaterLevel) return;
    rotateSpeed = 5;
    transform.DOKill();
    transform.DOMove(targetPos, 0.35f);
    receiveContainer = toContainer;
    isPouring = true;

  }

  public void StopPour()
  {
    transform.DOKill();
    if (!flyOutOnEmpty || waterLevel > 0)
    {
      transform.DORotate(Vector3.zero, 0.35f);
      transform.DOMove(oriPos, 0.35f);
    }
    receiveContainer = null;
    isPouring = false;
    if (currentWaterFlow != null)
    {
      currentWaterFlow.OnStopPouring();
      currentWaterFlow = null;
    }
  }

  public AudioClip[] acMerge;
  public AudioClip[] acContact;
  public AudioSource asrc;
  public void OnWaterFlowContact(Vector2 point)
  {
    asrc.PlayOneShot(acContact[Random.Range(0, acContact.Length)]);
    asrc.clip = acMerge[Random.Range(0, acMerge.Length)];
    asrc.time = asrc.clip.length * waterLevel;
    asrc.Play();
    if (fxSplash != null)
    {
      fxSplash.transform.position = point;
      fxSplash.Play();
    }
  }

  public void OnWaterFlowEnd()
  {
    asrc.Stop();
  }

  //private void OnDrawGizmos()
  //{
  //    if (debugHit != Vector2.zero)
  //    {
  //        Gizmos.color = Color.red;
  //        Gizmos.DrawWireSphere(debugHit, 0.1f);

  //        // draw a line to the hit point
  //        Gizmos.color = Color.green;
  //        Gizmos.DrawLine(center.position, debugHit);
  //    }
  //}

  [ContextMenu("Increase Layer")]
  public void IncreaseLayer()
  {
    // increase the layer of all the sprite renderers in the cup, and also spritemasks
    var renderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (var r in renderers)
    {
      r.sortingOrder++;
    }
    var masks = GetComponentsInChildren<SpriteMask>();
    foreach (var m in masks)
    {
      m.frontSortingOrder++;
      m.backSortingOrder++;
    }
  }

  [ContextMenu("Decrease Layer")]
  public void DecreaseLayer()
  {
    // decrease the layer of all the sprite renderers in the cup, and also spritemasks
    var renderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (var r in renderers)
    {
      r.sortingOrder--;
    }
    var masks = GetComponentsInChildren<SpriteMask>();
    foreach (var m in masks)
    {
      m.frontSortingOrder--;
      m.backSortingOrder--;
    }
  }

  [ContextMenu("Multiply Layer by 10")]
  public void MultiplyLayer()
  {
    // multiply the layer of all the sprite renderers in the cup, and also spritemasks by 10
    var renderers = GetComponentsInChildren<SpriteRenderer>();
    foreach (var r in renderers)
    {
      r.sortingOrder *= 10;
    }
    var masks = GetComponentsInChildren<SpriteMask>();
    foreach (var m in masks)
    {
      m.frontSortingOrder *= 10;
      m.backSortingOrder *= 10;
    }
  }
}
