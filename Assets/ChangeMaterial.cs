using UnityEngine;

/// <summary>
/// This script makes it easier to toggle between a new material, and the objects original material.
/// </summary>
public class ChangeMaterial : MonoBehaviour
{
    [Tooltip("The material that's switched to.")]
    public Material otherMaterial = null;

    [Tooltip("A list of materioals to switch between.")]
    public Material[] switchMaterials = null;
    private int currentMaterialIndex = 0;

    private bool usingOther = false;
    private MeshRenderer meshRenderer = null;
    private Material originalMaterial = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public void SetOtherMaterial()
    {
        usingOther = true;
        meshRenderer.material = otherMaterial;
    }

    public void SetOriginalMaterial()
    {
        usingOther = false;
        meshRenderer.material = originalMaterial;
    }

    public void ToggleMaterial()
    {
        usingOther = !usingOther;

        if(usingOther)
        {
            meshRenderer.material = otherMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }

    public void NextMaterial()
    {
        if (switchMaterials == null || switchMaterials.Length == 0)
        {
            Debug.LogWarning("No materials to switch between!");
            return;
        }

        currentMaterialIndex++; // Move to the next material

        if (currentMaterialIndex >= switchMaterials.Length)
        {
            currentMaterialIndex = 0; // Loop back to the start if we've passed the end of the array
        }

        meshRenderer.material = switchMaterials[currentMaterialIndex];
    }
}
