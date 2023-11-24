using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSprite : MonoBehaviour
{
    public Transform rotationCenter; // The point around which the object will rotate
    public float speed = 5f; // Speed of rotation
    public Vector2 radius = new Vector2(5f, 5f); // Radius of rotation
    public bool clockwise = true; // Direction of rotation
    public bool figureEight = false; // Toggle for figure-8 translation
    public Vector3 rotationAxis = Vector3.up; // The axis around which the object will rotate

    public float minSpeed = 0.15f; // Minimum speed of rotation
    public float maxSpeed = 2f; // Maximum speed of rotation

    private float timeCounter = 0f; // A time counter to control the speed

    void Start()
    {
        // Randomize the speed within the given range
        speed = Random.Range(minSpeed, maxSpeed);

        // Optionally, randomize the direction as well
        clockwise = (Random.value > 0.5f);
    }

    void Update()
    {
        timeCounter += Time.deltaTime * speed * (clockwise ? 1 : -1);

        // Circular motion
        float x = Mathf.Cos(timeCounter) * radius.x;
        float y = Mathf.Sin(timeCounter) * radius.y;

        // Modify y for figure-8 if enabled
        if (figureEight)
        {
            y *= Mathf.Cos(timeCounter * 2f);
        }

        Vector3 newPosition = new Vector3(x, y, 0);

        // Apply rotation to switch axis
        newPosition = Quaternion.AngleAxis(90, rotationAxis) * newPosition;

        // Set the position relative to the rotation center
        transform.position = rotationCenter.position + newPosition;
    }

    // Call this method to toggle the motion between circular and figure-8
    public void ToggleMotion()
    {
        figureEight = !figureEight;
    }

    // Call this method to change the rotation axis
    public void SetRotationAxis(Vector3 axis)
    {
        rotationAxis = axis;
    }
}
