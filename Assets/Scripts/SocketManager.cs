using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketManager : MonoBehaviour
{
    // Declare events
    public event Action<int> OnSocketOccupied;  // Invoked when a socket gets occupied
    public event Action<int> OnSocketVacated;   // Invoked when a socket gets vacated

    [Tooltip("ID of this specific socket.")]
    public int socketID;

    [Tooltip("The XR Grabbable that fits this socket.")]
    public XRBaseInteractable correctGrabbable;

    public LightPollutionManager LPM;

    public bool IsOccupied { get; private set; }

    private void Awake()
    {
        // Check for any interactables already in the socket
        foreach (var interactable in GetComponentsInChildren<XRBaseInteractable>())
        {
            if (correctGrabbable == null || interactable == correctGrabbable)
            {
                IsOccupied = true;
                break;  // exit the loop if we find a plug
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        XRBaseInteractable interactable = other.GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            // Optional: Check if it's the correct plug for this socket
            if (correctGrabbable == null || interactable == correctGrabbable)
            {
                IsOccupied = true;
            }
        }

        // If a correct plug is inserted
        if (IsOccupied)
        {
            OnSocketOccupied?.Invoke(socketID); // Raise the event
        }
    }

    private void OnTriggerExit(Collider other)
    {
        XRBaseInteractable interactable = other.GetComponent<XRBaseInteractable>();
        if (interactable != null && (correctGrabbable == null || interactable == correctGrabbable))
        {
            IsOccupied = false;
        }

        // If a correct plug is removed
        if (!IsOccupied)
        {
            OnSocketVacated?.Invoke(socketID); // Raise the event
        }
    }
}
