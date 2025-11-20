using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace VinhLB
{
  public static class VinhLBUtility
  {

    public static Vector3 GetPointerScreenPosition(int touchIndex = 0)
    {
      Vector3 pointerScreenPosition = Vector3.zero;
      if (IsOnEditor())
      {
        if (Input.GetMouseButton(0))
        {
          pointerScreenPosition = Input.mousePosition;
        }
      }
      else
      {
        if (Input.touchCount > 0 && touchIndex < Input.touchCount)
        {
          pointerScreenPosition = Input.GetTouch(touchIndex).position;
        }
      }

      return pointerScreenPosition;
    }


    public static Vector3 GetPointerWorldPosition(Camera camera = null, float zPosition = 0, int touchIndex = 0)
    {
      Vector3 pointerWorldPosition = Vector3.zero;
      camera ??= Camera.main;
      if (camera != null)
      {
        Vector3 pointerScreenPosition = GetPointerScreenPosition(touchIndex);
        pointerScreenPosition.z = zPosition;
        pointerWorldPosition = camera.ScreenToWorldPoint(pointerScreenPosition);
      }

      return pointerWorldPosition;
    }
    public static bool CheckClickOnUI(int touchIndex = 0)
    {
      PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
      eventDataCurrentPosition.position = GetPointerScreenPosition(touchIndex);
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
      foreach (var item in results)
      {
        if (item.gameObject.layer == VinhLBLayer.UI)
        {
          return true;
        }
      }

      return false;
    }

    public static bool IsPointerActive()
    {
      bool result;
      if (IsOnEditor())
      {
        result = Input.GetMouseButton(0);
      }
      else
      {
        result = Input.touchCount > 0;
      }

      return result;
    }

    public static bool IsPointerDown()
    {
      bool result;
      if (IsOnEditor())
      {
        result = Input.GetMouseButtonDown(0);
      }
      else
      {
        result = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
      }

      return result;
    }

    public static bool IsPointerUp()
    {
      bool result;
      if (IsOnEditor())
      {
        result = Input.GetMouseButtonUp(0);
      }
      else
      {
        result = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
      }

      return result;
    }


    private static bool IsOnEditor()
    {
      return Application.platform == RuntimePlatform.WindowsEditor ||
             Application.platform == RuntimePlatform.OSXEditor ||
             Application.platform == RuntimePlatform.LinuxEditor;
    }
  }
}