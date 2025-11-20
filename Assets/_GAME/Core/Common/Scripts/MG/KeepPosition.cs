using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the initial position or local position of the object
/// </summary>
public class KeepPosition : MonoBehaviour
{
    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate
    }
    
    public UpdateType updateType = UpdateType.Update;
    public bool local = false;

    private Vector3 _initialPosition;
    private Vector3 _initialLocalPosition;

    private void OnEnable()
    {
        if (local)
        {
            _initialLocalPosition = transform.localPosition;
        }
        else
        {
            _initialPosition = transform.position;
        }
    }

    public void ResetPosition()
    {
        if (local)
        {
            transform.localPosition = _initialLocalPosition;
        }
        else
        {
            transform.position = _initialPosition;
        }
    }

    private void Update()
    {
        if (updateType == UpdateType.Update)
        {
            ResetPosition();
        }
    }
    
    private void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
        {
            ResetPosition();
        }
    }
    
    private void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
        {
            ResetPosition();
        }
    }
}
