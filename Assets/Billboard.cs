using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool trackCamera;

    public float xOffset = 0;
    public float yOffset = 180;
    public float zOffset = 0;


    private void OnWillRenderObject()
    {
        Camera cam = Camera.main;
        if (cam != null && trackCamera)
        {
            Vector3 targetPos = cam.transform.position;
            Vector3 lookDir = (targetPos - transform.position);
            Quaternion lookR = Quaternion.LookRotation(lookDir, Vector3.up);
            transform.rotation = lookR * Quaternion.Euler(xOffset, yOffset, zOffset);
        }
    }
}
