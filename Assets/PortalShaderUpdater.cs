using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalShaderUpdater : MonoBehaviour
{
    public Material portalMaterial; // Assign your custom portal material here
    public Transform cameraTransform; // Assign your main camera transform here
    public Transform portalTransform; // Assign your portal plane transform here

    public float distanceMultiplier = 1.0f;
    public Vector2 uvOffset = Vector2.zero;
    public Vector2 uvScale = Vector2.one;

    void Update()
    {
        if (portalMaterial != null)
        {
            // Pass the camera's world position to the shader
            portalMaterial.SetVector("_CameraWorldPos", cameraTransform.position);

            // Pass the portal plane's world position to the shader
            portalMaterial.SetVector("_PlaneWorldPos", portalTransform.position);

            // Pass the portal plane's normal to the shader. Assuming the portal's forward vector is its normal.
            portalMaterial.SetVector("_PlaneNormal", portalTransform.forward);

            // Set the shader property for the distance multiplier
            portalMaterial.SetFloat("_DistanceMultiplier", distanceMultiplier);

            // Set the UV offset and scale
            portalMaterial.SetVector("_UVOffset", new Vector4(uvOffset.x, uvOffset.y, 0, 0));
            portalMaterial.SetVector("_UVScale", new Vector4(uvScale.x, uvScale.y, 0, 0));
        }
    }
}
