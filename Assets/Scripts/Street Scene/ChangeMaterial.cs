using UnityEngine;

/// <summary>
/// This script makes it easier to toggle between a new material, and the objects original material.
/// </summary>
public class ChangeMaterial : MonoBehaviour
{
    [Tooltip("The material that's switched to.")]
    public Material lightOffMaterial = null;
    public Material lightOnMaterial = null;

    private bool usingOther = false;
    public MeshRenderer meshRenderer = null;
    public Material[] originalMaterials = null;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterials = meshRenderer.materials.Clone() as Material[];
    }

    public void SetMaterial(int index, Material newMaterial)
    {
        // First, check if the index is valid
        if (index < 0 || index >= meshRenderer.materials.Length)
        {
            Debug.LogError("Material index out of range.");
            return;
        }

        // Get a copy of the current materials array from the renderer
        Material[] materials = meshRenderer.materials;

        // Change the material at the specified index
        materials[index] = newMaterial;

        // Re-assign the modified materials array back to the renderer
        meshRenderer.materials = materials;
    }

    public void SetOriginalMaterial(int index)
    {
        if (originalMaterials != null && index >= 0 && index < originalMaterials.Length)
        {
            // Get the current materials array
            Material[] currentMaterials = meshRenderer.materials;

            // Replace the specified material with the original one
            currentMaterials[index] = originalMaterials[index];
            
            // Assign the modified materials array back to the renderer
            meshRenderer.materials = originalMaterials;
        }
        else
        {
            Debug.LogError("Original materials array not set or index out of range.");
        }
    }


    //public void ToggleMaterial(int materialIndex)
    //{
    //    usingOther = !usingOther;
    //
    //    // Make sure we don't access out of bounds
    //    if (materialIndex < 0 || materialIndex >= meshRenderer.materials.Length)
    //    {
    //        Debug.LogError("Material index out of range.");
    //        return;
    //    }
    //
    //    // Get the current materials array
    //    Material[] currentMaterials = meshRenderer.materials;
    //
    //    // Toggle the specified material
    //    if (usingOther)
    //    {
    //        currentMaterials[materialIndex] = redLightOffMaterial;
    //    }
    //    else
    //    {
    //        currentMaterials[materialIndex] = originalMaterials[1];
    //    }
    //
    //    // Assign the modified materials array back to the renderer
    //    meshRenderer.materials = currentMaterials;
    //
    //    //OLD shit
    //    //usingOther = !usingOther;
    //    //
    //    //if(usingOther)
    //    //{
    //    //    meshRenderer.material = otherMaterial;
    //    //}
    //    //else
    //    //{
    //    //    meshRenderer.material = originalMaterial;
    //    //}
    //}

    //public void NextMaterial()
    //{
    //    if (switchMaterials == null || switchMaterials.Length == 0)
    //    {
    //        Debug.LogWarning("No materials to switch between!");
    //        return;
    //    }
    //
    //    currentMaterialIndex++; // Move to the next material
    //
    //    if (currentMaterialIndex >= switchMaterials.Length)
    //    {
    //        currentMaterialIndex = 0; // Loop back to the start if we've passed the end of the array
    //    }
    //
    //    meshRenderer.material = switchMaterials[currentMaterialIndex];
    //}

    public void TurnOffBloom()
    {
        // Find BLOOM png objects
        Transform[] blooms = this.transform.FindChildrenWithTag("Bloom");
        // Disable BLOOM png objects
        foreach (Transform bloom in blooms)
        {
            SpriteRenderer bloomSprite = bloom.GetComponent<SpriteRenderer>();
            bloomSprite.enabled = false;
        }
    }
    public void TurnOnBloom()
    {
        // Find BLOOM png objects
        Transform[] blooms = this.transform.FindChildrenWithTag("Bloom");
        // Enable BLOOM png objects
        foreach (Transform bloom in blooms)
        {
            SpriteRenderer bloomSprite = bloom.GetComponent<SpriteRenderer>();
            bloomSprite.enabled = true;
        }
    }
}
