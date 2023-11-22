using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrink : MonoBehaviour
{
    [Header("Bloom Specific")]
    [SerializeField]
    public bool oscilate;

    [SerializeField]
    private float frequency = 5.0f; // Speed of sine wave cycle
    [SerializeField]
    private float amplitude = 0.25f; // Size of the sine wave cycle
    [SerializeField]
    private Vector3 baseScale =  new Vector3(2.0f, 2.0f, 2.0f); // The base scale from which the object will grow and shrink

    private float elapsedTime = 0f; // Time elapsed since the beginning of the oscillation

    void Update()
    {
        if (oscilate)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the scale factor using a sine wave
            float scaleFactor = Mathf.Sin(elapsedTime * frequency) * amplitude + 1; // '+ 1' ensures that the scale factor oscillates around the base scale

            // Apply the new scale to the GameObject
            transform.localScale = baseScale * scaleFactor;
        }
    }
}
