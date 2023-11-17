using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP : MonoBehaviour
{
    public GameObject lights;

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger && other.gameObject.CompareTag("Hand"))
            lights.SetActive(false);

    }
}
