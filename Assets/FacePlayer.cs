using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [Tooltip("Offsets used only when Y-Axis is locked")]
    public float xOffset = 0;
    public float zOffset = 0;
    public bool lockYAxis;

    private void LateUpdate()
    {
        Transform cameraTransform = Camera.main.transform;
        transform.LookAt(cameraTransform); // look at the camera
        if(lockYAxis)
            transform.rotation = Quaternion.Euler(xOffset, transform.rotation.eulerAngles.y, zOffset); //only rotate around the Y axis

        ////Look at the camera with offset
        //Vector3 targetPos = Camera.main.transform.position;
        //Vector3 lookDir = (targetPos - transform.position);
        //Quaternion lookR = Quaternion.LookRotation(lookDir, Vector3.up);
        //transform.rotation = lookR * Quaternion.Euler(90, yOffset, 0);

    }
}
