using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public Material skyboxMaterial;

    [Range(0, 3)]
    public float blendValue;

    private void Update()
    {
        // Clamps the blendValue between 0 and 3
        blendValue = Mathf.Clamp(blendValue, 0f, 3f);
        
        SetBlendValue(blendValue);
    }

    public void SetBlendValue(float value)
    {
        if (skyboxMaterial)
        {
            skyboxMaterial.SetFloat("_BlendValue", value);
        }
    }
}
