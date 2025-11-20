using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shifts the position of other objects relative to this object
/// </summary>
[ExecuteInEditMode]
public class RelativePositionShift : MonoBehaviour
{
    [System.Serializable]
    public struct RelativeTransform
    {
        public Transform transform;
        public Vector3 posMult;
    }

    public RelativeTransform[] relativeTransforms;

    public bool isLocal = true;
    
    // Update is called once per frame
    void Update()
    {
        if (isLocal)
        {
            foreach (RelativeTransform relativeTransform in relativeTransforms)
            {
                if (relativeTransform.transform == null) continue;
                relativeTransform.transform.localPosition = new Vector3(transform.localPosition.x * (1 + relativeTransform.posMult.x), transform.localPosition.y * (1 + relativeTransform.posMult.y), transform.localPosition.z * (1 + relativeTransform.posMult.z));
            }
        }
        else
        {
            foreach (RelativeTransform relativeTransform in relativeTransforms)
            {
                if (relativeTransform.transform == null) continue;
                relativeTransform.transform.position = new Vector3(transform.position.x * (1 + relativeTransform.posMult.x), transform.position.y * (1 + relativeTransform.posMult.y), transform.position.z * (1 + relativeTransform.posMult.z));
            }
        }
    }
}
