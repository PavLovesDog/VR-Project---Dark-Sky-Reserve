using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float cloudSpeed;
    public float daylightSpeed;

    public bool isLight;
    public bool isClouds;

    void Update()
    {
        if(isClouds)
            transform.Rotate(0, 0, cloudSpeed * Time.deltaTime);
        else
            transform.Rotate(daylightSpeed * Time.deltaTime, 0, 0);
    }
}
