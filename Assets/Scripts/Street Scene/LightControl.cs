using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [Tooltip("List of Light to turn off")]
    public GameObject[] lightObjects;

    public Material offMaterial;
    public Material lightMaterial = null;

    private void Awake()
    {
        // store original glow material
        // find light object by tag, set its material
        foreach (GameObject obj in lightObjects)
        {
            Debug.Log("Searching for Original materials in " + obj.name);

            // find transform of light object in children
            Transform light = obj.transform.FindChildWithTag("Light");

            if (light)
            {
                Debug.Log("Found Light material in " + light.name);
                lightMaterial = light.GetComponent<Renderer>().material;
                break;
            }
        }
    }

    public void TurnOffLights()
    {
        foreach (GameObject obj in lightObjects)
        {
            // find transform of light object in children
            Transform[] materials = obj.transform.FindChildrenWithTag("Light");

            //Switch MATERIALS
            foreach (Transform material in materials)
            {
                Debug.Log("OFF Material: " + material.name);
                material.GetComponent<Renderer>().material = offMaterial;
            }

            // Check for LIGHTS and disable them
            Light[] lights = obj.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                Debug.Log("Light disabled: " + light.name);
                light.enabled = false;
            }

            // Find BLOOM png objects
            Transform[] blooms = obj.transform.FindChildrenWithTag("Bloom");
            // Disable BLOOM png objects
            foreach(Transform bloom in blooms)
            {
                Debug.Log("Bloom sprite disabled: " + bloom.name);
                SpriteRenderer bloomSprite = bloom.GetComponent<SpriteRenderer>();
                bloomSprite.enabled = false;
            }
        }
    }

    public void TurnOnLights()
    {
        foreach (GameObject obj in lightObjects)
        {
            // find transform of light object in children
            Transform[] materials = obj.transform.FindChildrenWithTag("Light");

            foreach (Transform material in materials)
            {
                Debug.Log("ON Material: " + material.name);
                material.GetComponent<Renderer>().material = lightMaterial;
            }

            // Check for light and enable it.
            Light[] lights = obj.GetComponentsInChildren<Light>(true);
            foreach (Light light in lights)
            {
                Debug.Log("Light enabled: " + light.name);
                light.enabled = true;
            }

            // Find BLOOM png objects
            Transform[] blooms = obj.transform.FindChildrenWithTag("Bloom");
            // Enable BLOOM png objects
            foreach (Transform bloom in blooms)
            {
                Debug.Log("Bloom sprite enabled: " + bloom.name);
                SpriteRenderer bloomSprite = bloom.GetComponent<SpriteRenderer>();
                bloomSprite.enabled = true;
            }
        }
    }
}
