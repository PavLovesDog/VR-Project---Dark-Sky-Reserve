using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    public Transform rotationCenter; // The point around which the object will rotate
    [Header("Speed")]
    public float speed = 5f; // Speed of rotation
    public bool randomizeSpeed = true;
    public float minSpeed = 0.15f; // Minimum speed of rotation
    public float maxSpeed = 2f; // Maximum speed of rotation
    [Header("Rotations")]
    public Vector2 radius = new Vector2(5f, 5f); // Radius of rotation
    private Vector3 rotationAxis = Vector3.up; // The axis around which the object will rotate
    public bool randomizeRotation = true;
    public bool clockwise = true; // Direction of rotation
    public bool figureEight = false; // Toggle for figure-8 translation
    public bool horizontalFigureEight = false; // Toggle for horizontal figure-eight
    public float figureEightScale = 1f; // Scale the figure-eight pattern
    [Header("Wobble Effect")]
    public bool wobble = false; // Control wobble effect
    public float wobbleSpeed = 2f; // Speed of wobble
    public float wobbleMagnitude = 15f; // Magnitude of wobble (degrees)
    [Header("Sprite Flip State")]
    public bool flipSpriteAtTips = false; // Control sprite flipping
    public bool previousFlipState = true; // To avoid continuous flipping
    private float previousY = 0f; // To track previous y value for flipping
    [Header("Other")]
    public SpriteRenderer sprite;
    public float axisVisualizationLength = 2.0f;
    private float timeCounter = 0f; // A time counter to control the speed

    void Start()
    {
        if(randomizeSpeed)
            speed = Random.Range(minSpeed, maxSpeed); // Randomize the speed within the given range

        if(randomizeRotation)
        clockwise = (Random.value > 0.5f); //randomize the direction

        // Get the sprite component
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Dynamically update the rotation axis based on the rotationCenter's rotation
        // use the rotationCenter's forward direction as the rotation axis
        rotationAxis = rotationCenter.forward;

        timeCounter += Time.deltaTime * speed * (clockwise ? 1 : -1);

        // Circular motion
        float x = Mathf.Cos(timeCounter) * radius.x;
        float y = Mathf.Sin(timeCounter) * radius.y;

        if (figureEight)
        {
            if (horizontalFigureEight)
            {
                x *= Mathf.Cos(timeCounter * 2f) * figureEightScale;
            }
            else
            {
                y *= Mathf.Cos(timeCounter * 2f) * figureEightScale;
            }
        }

        #region flip at apex of figure 8 
        // Calculate the derivative sign
        float currentYDerivative = Mathf.Cos(timeCounter) * (clockwise ? 1 : -1);
        bool currentFlipState = currentYDerivative > 0;

        // Check if we need to flip the sprite
        if (flipSpriteAtTips && currentFlipState != previousFlipState)
        {
            sprite.flipX = !sprite.flipX; // Flip the sprite on X axis
        }

        // Update previous values
        previousY = y;
        previousFlipState = currentFlipState;
        #endregion

        #region Wobble Effect
        if (wobble)
        {
            // Calculate wobble angle
            float wobbleAngle = Mathf.Sin(timeCounter * wobbleSpeed) * wobbleMagnitude;

            // Apply wobble rotation around the Z-axis
            transform.localRotation = Quaternion.Euler(0, 0, wobbleAngle);
        }
        #endregion

        Vector3 newPosition = new Vector3(x, y, 0);

        // Apply rotation to switch axis
        newPosition = Quaternion.AngleAxis(90, rotationAxis) * newPosition;

        // Set the position relative to the rotation center
        transform.position = rotationCenter.position + newPosition;
    }

    // Add this method to draw the rotation axis line in the Scene view
    private void OnDrawGizmos()
    {
        if (rotationCenter != null)
        {
            // determine the current rotation axis
            Vector3 currentRotationAxis = rotationCenter.forward;

            Gizmos.color = Color.blue; // Set the color of the Gizmo line
            Gizmos.DrawLine(rotationCenter.position, rotationCenter.position + currentRotationAxis * axisVisualizationLength);
        }
    }
}
