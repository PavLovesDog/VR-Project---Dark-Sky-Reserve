using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LightPollutionManager : MonoBehaviour
{
    // Delegate and event for socket state changes
    public delegate void SocketStateChangeAction(int socketIndex, bool isOccupied);
    public event SocketStateChangeAction OnSocketStateChange;

    // track which sockets have been previously deactivated
    [SerializeField]
    public HashSet<int> deactivatedLevers = new HashSet<int>();
    [SerializeField]
    private List<int> deactivatedLeversList = new List<int>(); // list for inspector purposes

    
    [Tooltip("The Skybox to manipulate")]
    public SkyboxController skyboxController;

    [Tooltip("All the sockets that need to be tracked.")]
    public SocketManager[] sockets;

    public ChangeMaterial[] bulbLights;
    public LightControl[] lightManagers;

    // To store the previous count and check for changes
    public int currentSocketsOccupiedCount = 0;
    public int previousSocketsOccupiedCount = 0;

    // Bools for specifric plugs
    public bool socket0Plugged;
    public bool socket1Plugged;
    public bool socket2Plugged;
    public bool socket3Plugged;

    // Duration of the Light transition in seconds.
    private float transitionDuration = 3.0f;

    private Coroutine skyboxTransitionCoroutine;
    public float currentBlendValue;
    public float targetBlendValue;
    public float blendValue1 = 3f;
    public float blendValue2 = 1.75f;
    public float blendValue3 = 1.25f;
    public float blendValue4 = 0.75f;

    private void Awake()
    {
        skyboxController = GameObject.FindGameObjectWithTag("SkyboxController").GetComponent<SkyboxController>();
        previousSocketsOccupiedCount = CountOccupiedSockets(); // set amount of plugs plugged!

        foreach (var socket in sockets)
        {
            socket.OnSocketOccupied += HandleSocketOccupied;
            socket.OnSocketVacated += HandleSocketVacated;
        }

        // Check and set initial blend value based on socket states
        SetInitialBlendValue();
    }

    private void Update()
    {
        TrackSockets();
        UpdateSkyboxBlendIfNeeded();
    }

    //Ensure the sky blend starts at 0
    private void SetInitialBlendValue()
    {
        int initialOccupiedCount = CountOccupiedSockets();
        float targetBlendValue = 0f;

        StartTransition(targetBlendValue);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        foreach (var socket in sockets)
        {
            socket.OnSocketOccupied -= HandleSocketOccupied;
            socket.OnSocketVacated -= HandleSocketVacated;
        }
    }


    //Event to fire when socket is occupied
    private void HandleSocketOccupied(int socketID)
    {
        //update skybox material
        currentSocketsOccupiedCount = CountOccupiedSockets();
        //UpdateSkyboxBlend(currentSocketsOccupiedCount);

        //turn on lights for the specified socket
        lightManagers[socketID].TurnOnLights();

        ////play audio
        //AudioManager.Instance.PlaySFX(0, 0.65f, Random.Range(0.6f,0.9f));

        // Trigger the state change event with the socket ID and the state (occupied)
        OnSocketStateChange?.Invoke(socketID, true);
    }

    //Event to fire when socket is Vacated
    private void HandleSocketVacated(int socketID)
    {
        //update skybox material
        currentSocketsOccupiedCount = CountOccupiedSockets();
       // UpdateSkyboxBlend(currentSocketsOccupiedCount);

        //turn off lights for the specified socket
        lightManagers[socketID].TurnOffLights();

        //Rewduce sizze of background glow
        GlowScaleAdjust.Instance.StartLerpToNextScale();

        // Trigger the state change event with the socket ID and the state (not occupied)
        OnSocketStateChange?.Invoke(socketID, false);
    }

    // Call this method to mark a socket as deactivated
    public void MarkSocketAsDeactivated(int socketIndex)
    {
        if (!deactivatedLevers.Contains(socketIndex))
        {
            deactivatedLevers.Add(socketIndex);
            // Update the debug list as well
            deactivatedLeversList.Add(socketIndex);
        }
    }

    //// Call this method to mark a socket as deactivated
    //public void MarkSocketAsDeactivated(int socketIndex)
    //{
    //    deactivatedSockets.Add(socketIndex);
    //}

    // Call this method to check if a socket has been deactivated before
    public bool HasSocketBeenDeactivatedBefore(int socketIndex)
    {
        return deactivatedLevers.Contains(socketIndex);
    }

    private void UpdateSkyboxBlendIfNeeded()
    {
        currentSocketsOccupiedCount = CountOccupiedSockets();
    
        if (currentSocketsOccupiedCount != previousSocketsOccupiedCount)
        {
            Debug.Log($"Number of plugs connected: {currentSocketsOccupiedCount}");
            previousSocketsOccupiedCount = currentSocketsOccupiedCount;
    
            float targetBlendValue = 0f;
    
            switch (currentSocketsOccupiedCount)
            {
                case 0: targetBlendValue = blendValue1; break;
                case 1: targetBlendValue = blendValue2; break;
                case 2: targetBlendValue = blendValue3; break;
                case 3: targetBlendValue = blendValue4; break;
                case 4: targetBlendValue = 0f; break;
            }

            StartTransition(targetBlendValue);
        }
    }

    // Method to trigger the transition
    public void StartTransition(float newTargetBlendValue)
    {
        targetBlendValue = newTargetBlendValue;

        // If a transition is already ongoing, stop it
        if (skyboxTransitionCoroutine != null)
        {
            StopCoroutine(skyboxTransitionCoroutine);
        }

        // Start a new transition
        skyboxTransitionCoroutine = StartCoroutine(TransitionSkyboxBlend());
    }

    //Coroutine to change the skybox
    private IEnumerator TransitionSkyboxBlend()
    {
        float initialBlendValue = skyboxController.blendValue;
        float elapsedTime = 0f;
        int frameCounter = 0;
        int updateFrequency = 10; // update every 10 frames

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            currentBlendValue = Mathf.Lerp(initialBlendValue, targetBlendValue, elapsedTime / transitionDuration);
            skyboxController.blendValue = currentBlendValue;

            //RenderSettings.ambientIntensity = currentBlendValue;

            // Rate-limit DynamicGI.UpdateEnvironment();
            frameCounter++;
            if (frameCounter >= updateFrequency)
            {
                DynamicGI.UpdateEnvironment();
                frameCounter = 0;
            }

            yield return null; // wait for the next frame
        }

        // Ensure we reach the exact target value at the end of the transition.
        skyboxController.blendValue = targetBlendValue;
        currentBlendValue = targetBlendValue;

        // Final environment update at the end of the transition
        //DynamicGI.UpdateEnvironment();
    }

    public void TrackSockets()
    {
        if (sockets[0].IsOccupied)
        {
            socket0Plugged = true;
            bulbLights[0].SetOriginalMaterial(1); // Reset the second material to the original
            bulbLights[0].TurnOnBloom();
        }
        else
        {
            socket0Plugged = false;
            bulbLights[0].SetMaterial(1, bulbLights[0].lightOffMaterial); // stop red bulb
            bulbLights[0].TurnOffBloom();
        }

        if (sockets[1].IsOccupied)
        {
            socket1Plugged = true;
            bulbLights[1].SetOriginalMaterial(1);
            bulbLights[1].TurnOnBloom();
        }
        else
        {
            socket1Plugged = false;
            bulbLights[1].SetMaterial(1, bulbLights[1].lightOffMaterial);
            bulbLights[1].TurnOffBloom();
        }

        if (sockets[2].IsOccupied)
        {
            socket2Plugged = true;
            bulbLights[2].SetOriginalMaterial(1);
            bulbLights[2].TurnOnBloom();
        }
        else
        {
            socket2Plugged = false;
            bulbLights[2].SetMaterial(1, bulbLights[2].lightOffMaterial); ;
            bulbLights[2].TurnOffBloom();
        }

        if (sockets[3].IsOccupied)
        { 
            socket3Plugged = true;
            bulbLights[3].SetOriginalMaterial(1);
            bulbLights[3].TurnOnBloom();
        }
        else
        {
            socket3Plugged = false;
            bulbLights[3].SetMaterial(1, bulbLights[3].lightOffMaterial);
            bulbLights[3].TurnOffBloom();
        }

    }

    // Function to return the count of occupied sockets.
    private int CountOccupiedSockets()
    {
        int occupiedCount = 0;

        foreach (SocketManager socket in sockets)
        {
            if (socket.IsOccupied)
                occupiedCount++;
        }

        return occupiedCount;
    }
}
