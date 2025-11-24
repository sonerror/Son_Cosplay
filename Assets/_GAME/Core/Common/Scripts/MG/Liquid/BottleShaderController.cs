using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the water surface always be pointing upwards
/// </summary>
public class BottleShaderController : MonoBehaviour
{
    public Material bottleMaterial; // using Bottle shader
    public float ratio = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AdjustWaterRotation();
    }

    public void AdjustWaterRotation()
    {
        // Get the current rotation of the water surface
        float objectRotation = transform.rotation.eulerAngles.z;

        // Set the rotation of the water surface to 0
        bottleMaterial.SetFloat("_Rotation", -objectRotation * ratio);
    }
}
