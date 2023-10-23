using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketManager : MonoBehaviour
{
    [Tooltip("The XR Grabbable that fits this socket.")]
    public XRBaseInteractable correctGrabbable;

    public bool IsOccupied { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        XRBaseInteractable interactable = other.GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            // Optional: Check if it's the correct plug for this socket
            if (correctGrabbable == null || interactable == correctGrabbable)
            {
                IsOccupied = true;
                // Execute any additional logic,
                // enable light material
                // play buzz sound
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        XRBaseInteractable interactable = other.GetComponent<XRBaseInteractable>();
        if (interactable != null && (correctGrabbable == null || interactable == correctGrabbable))
        {
            IsOccupied = false;
            // Execute any additional logic,
            // Disable light material
            // stop buzz sound
        }
    }
}
