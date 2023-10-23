using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCont : MonoBehaviour
{
    [Header("Blend Control")]
    [Range(0, 3)]
    public float blendValue;
    public float blendSpeed = 0.1f;

    [Header("Rotation Control")]
    [Range(-360, 360)]
    public float rotationX = 0;
    [Range(-360, 360)]
    public float rotationY = 0;
    [Range(-360, 360)]
    public float rotationZ = 0;

    private Material skyboxMaterial;

    private void Start()
    {
        if (!RenderSettings.skybox)
        {
            Debug.LogError("No global skybox material set!");
            enabled = false;
            return;
        }

        // Get a reference to the global skybox material
        skyboxMaterial = RenderSettings.skybox;
    }

    private void Update()
    {
        if (skyboxMaterial)
        {
            // Update blend value
            float currentBlend = skyboxMaterial.GetFloat("_BlendValue");
            float newBlend = Mathf.MoveTowards(currentBlend, blendValue, blendSpeed * Time.deltaTime);
            skyboxMaterial.SetFloat("_BlendValue", newBlend);

            // Update rotations
            skyboxMaterial.SetFloat("_RotationX", rotationX);
            skyboxMaterial.SetFloat("_RotationY", rotationY);
            skyboxMaterial.SetFloat("_RotationZ", rotationZ);
        }
    }

    // Call this function to set a new target blend value
    public void SetBlendTarget(float newBlend)
    {
        blendValue = Mathf.Clamp(newBlend, 0, 3);
    }

    // Call this function to instantly set a new blend value without transitioning
    public void SetInstantBlend(float newBlend)
    {
        blendValue = Mathf.Clamp(newBlend, 0, 3);
        if (skyboxMaterial)
        {
            skyboxMaterial.SetFloat("_BlendValue", blendValue);
        }
    }
}
