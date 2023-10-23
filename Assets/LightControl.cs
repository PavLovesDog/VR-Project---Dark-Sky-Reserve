using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [Tooltip("List of Lamp Lights to turn off")]
    public GameObject[] Lamps;

    [Tooltip("The material to set on each game object.")]
    public Material offMaterial;

    private MeshRenderer meshRenderer = null;
    private Material originalMaterial = null;

    private void Awake()
    {
        meshRenderer = Lamps[0].GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    // Call this function to modify the objects in the list.
    public void TurnOffLights()
    {
        foreach (GameObject obj in Lamps)
        {
            // find its Renderer component and set the material.
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = offMaterial;
            }

            // Check for light and disable it.
            Light[] lights = obj.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                light.enabled = false;
            }
        }
    }

    public void TurnOnLights()
    {
        foreach (GameObject obj in Lamps)
        {
            // find its Renderer component and set the material.
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = originalMaterial;
            }

            // Check for light and enable it.
            Light[] lights = obj.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                light.enabled = true;
            }
        }
    }
}
