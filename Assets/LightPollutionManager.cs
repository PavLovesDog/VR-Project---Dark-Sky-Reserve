using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LightPollutionManager : MonoBehaviour
{
    [Tooltip("The Skybox to manipulate")]
    public SkyboxController skyboxController;

    [Tooltip("All the sockets that need to be tracked.")]
    public SocketManager[] sockets;

    public ChangeMaterial[] bulbLights;
    public LightControl[] LampLots;

    // Bools for specifric plugs
    public bool socket1Plugged;
    public bool socket2Plugged;
    public bool socket3Plugged;
    public bool socket4Plugged;

    // To store the previous count and check for changes
    private int previousOccupiedCount = 0;

    // Duration of the Light transition in seconds.
    private float transitionDuration = 3.0f;

    public float blendValue1 = 3f;
    public float blendValue2 = 1.85f;
    public float blendValue3 = 1.25f;
    public float blendValue4 = 0.75f;

    // A flag to determine if a transition is currently in progress.
    private bool isTransitioning = false;

    private void Awake()
    {
        skyboxController = GameObject.FindGameObjectWithTag("SkyboxController").GetComponent<SkyboxController>();
    }

    private void Update()
    {
        TrackSockets();

        int currentOccupiedCount = CountOccupiedSockets();

        // Check if the occupied count has changed since the last frame
        if (currentOccupiedCount != previousOccupiedCount)
        {
            Debug.Log($"Number of plugs connected: {currentOccupiedCount}");
            previousOccupiedCount = currentOccupiedCount;

            float targetBlendValue = 0f;

            switch (currentOccupiedCount)
            {
                case 0: targetBlendValue = blendValue1; break;
                case 1: targetBlendValue = blendValue2; break;
                case 2: targetBlendValue = blendValue3; break;
                case 3: targetBlendValue = blendValue4; break;
                case 4: targetBlendValue = 0f; break;
            }

            if(!isTransitioning)
                StartCoroutine(TransitionSkyboxBlend(targetBlendValue));

            //// Adjust Skybox Light based on plugs
            //// NOTE: change the setting of blend values to  Lerps within coroutines.
            //switch (currentOccupiedCount)
            //{
            //    case 0: //No plugs connected
            //        // skybox slider 3
            //        skyboxController.blendValue = 3f;
            //        break;
            //
            //    case 1: // 1 plug connected
            //        // skybox slider 2
            //        skyboxController.blendValue = 1.85f;
            //        break;
            //
            //    case 2: // 2 plugs connected
            //        // skybox slider 1.75
            //        skyboxController.blendValue = 1.25f;
            //        break;
            //
            //    case 3: // 3 plugs connected
            //        // skybox slider 0.75
            //        skyboxController.blendValue = 0.75f;
            //        break;
            //
            //    case 4: // 4 plugs connected
            //        // skybox slider 0
            //        skyboxController.blendValue = 0;
            //        break;
            //}
        }
    }
    private IEnumerator TransitionSkyboxBlend(float targetBlendValue)
    {
        isTransitioning = true;

        float initialBlendValue = skyboxController.blendValue;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            skyboxController.blendValue = Mathf.Lerp(initialBlendValue, targetBlendValue, elapsedTime / transitionDuration);
            yield return null; // wait for the next frame
        }

        // Ensure we reach the exact target value at the end of the transition.
        skyboxController.blendValue = targetBlendValue;

        isTransitioning = false;
    }


    public void TrackSockets()
    {
        if (sockets[0].IsOccupied)
        {
            socket1Plugged = true;
            bulbLights[0].SetOriginalMaterial();
            LampLots[0].TurnOnLights();
        }
        else
        {
            socket1Plugged = false;
            bulbLights[0].SetOtherMaterial(); // stop red bulb
            LampLots[0].TurnOffLights(); // set lights
        }

        if (sockets[1].IsOccupied)
        {
            socket2Plugged = true;
            bulbLights[1].SetOriginalMaterial();
            LampLots[1].TurnOnLights();
        }
        else
        {
            socket2Plugged = false;
            bulbLights[1].SetOtherMaterial();
            LampLots[1].TurnOffLights();
        }

        if (sockets[2].IsOccupied)
        {
            socket3Plugged = true;
            bulbLights[2].SetOriginalMaterial();
            LampLots[2].TurnOnLights();
        }
        else
        {
            socket3Plugged = false;
            bulbLights[2].SetOtherMaterial();
            LampLots[2].TurnOffLights();
        }

        if (sockets[3].IsOccupied)
        { 
            socket4Plugged = true;
            bulbLights[3].SetOriginalMaterial();
            LampLots[3].TurnOnLights();
        }
        else
        {
            socket4Plugged = false;
            bulbLights[3].SetOtherMaterial();
            LampLots[3].TurnOffLights();
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
