using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // start and end postions for directional light rotation
    public Quaternion startRotation;
    public Quaternion endRotation;
    public Quaternion currentRotation;

    PlanetScenePositionManager positionManager;

    [Header("Rotation Speeds")]
    public float cloudSpeed;
    public float daylightSpeed;
    public float daylightFastSpeed;
    public float currentSpeed;
    public float lerpSpeed;
    private float lerpProgress = 0.0f;
    public float rotationThreshold = 0.1f; // Threshold in degrees

    [Header("Light Movement Varibales")]
    public float sunMovementTimer;

    [Header("Object Types")]
    public bool rotate;
    public bool isLight;
    public bool isClouds;

    private void Start()
    {
        if (isLight)
            startRotation = gameObject.transform.rotation;
           
        positionManager = GameObject.FindObjectOfType<PlanetScenePositionManager>();
    }

    void Update()
    {
        //countdown from
        sunMovementTimer -= Time.deltaTime;

        //track rotation 
        currentRotation = gameObject.transform.localRotation;

        if (rotate)
        {
            if (isClouds)
                transform.Rotate(0, 0, cloudSpeed * Time.deltaTime);
            else // is light object
            {
                //// Check if the rotation is close enough to the target
                //if (Quaternion.Angle(transform.rotation, endRotation) < rotationThreshold)
                //{
                //    rotate = false;
                //    //transform.rotation = endRotation;
                //    return;
                //}

                if(sunMovementTimer <= 0)
                {
                    sunMovementTimer = 0;
                    rotate = false;
                    //transform.rotation = endRotation;
                    return;
                }

                if (positionManager.speedUpNight) // user view is far back, we want night changing faster
                {
                    // Update the lerp progress towards the fast speed
                    lerpProgress += lerpSpeed * Time.deltaTime;
                    lerpProgress = Mathf.Clamp01(lerpProgress); // Clamp between 0 and 1

                    // lerp from current speed to faster speed, using progress
                    currentSpeed = Mathf.Lerp(daylightSpeed, daylightFastSpeed, lerpProgress);
                    //speed up transition of day to night
                    transform.Rotate(-currentSpeed * Time.deltaTime, 0, 0);
                }
                else // user view is still close, want don't night changing faster
                {
                    //change day to night at normal speed
                    transform.Rotate(-daylightSpeed * Time.deltaTime, 0, 0);
                }
            }
        }
    }
}
