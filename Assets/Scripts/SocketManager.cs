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

    [Tooltip("The Object that fits this socket.")]
    public GameObject correctGrabbable;

    public LightPollutionManager LPM;

    public bool IsOccupied { get; private set; }

    private void Awake()
    {
        // Check for any interactables already in the socket
        if (correctGrabbable != null && correctGrabbable.transform.IsChildOf(transform))
        {
            IsOccupied = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == correctGrabbable/* || correctGrabbable == null*/)
        {
            IsOccupied = true;
            OnSocketOccupied?.Invoke(socketID); // Raise the event
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == correctGrabbable/* || (correctGrabbable == null && IsOccupied*/)
        {
            IsOccupied = false;
            OnSocketVacated?.Invoke(socketID); // Raise the event
        }
    }
}
