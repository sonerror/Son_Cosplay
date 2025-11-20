using UnityEngine;

namespace HoangHH
{
  public abstract class H3Singleton<T> : H3MonoBehaviour where T : H3MonoBehaviour
  {
    [SerializeField] private bool isDontDestroyOnLoad = true;
    public static bool IsExistInstance => _instance != null;

    private static T _instance;

    public static T Ins
    {
      get
      {
        return _instance;
      }
    }

    protected virtual void Awake()
    {
      if (_instance is null)
      {
        _instance = this as T;
        if (isDontDestroyOnLoad)
        {
          DontDestroyOnLoad(gameObject);
        }
        OnAwakeEvent();
      }
      else if (_instance != this)
      {
        Destroy(gameObject);
      }
    }

    protected virtual void OnDestroy()
    {
      if (this == _instance)
      {
        _instance = null;
      }

      OnDestroyEvent();
    }

    protected virtual void OnAwakeEvent() { }
    protected virtual void OnDestroyEvent() { }

  }

}