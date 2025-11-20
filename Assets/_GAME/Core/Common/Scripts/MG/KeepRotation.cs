using System.Collections;
using UnityEngine;

/// <summary>
/// Keeps rotation of the object straight
/// </summary>
public class KeepRotation : MonoBehaviour
{
    public Quaternion rotation = Quaternion.identity;

    void Update()
    {
        transform.rotation = rotation;
    }

    private void OnEnable()
    {
        transform.rotation = rotation;
    }
}