using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [SerializeField]
    private LightPollutionManager lightPollutionManager; // Assign in inspector

    [SerializeField]
    private float initialNarrationDelay = 5.0f; // Time before starting the narration

    [SerializeField]
    private float narrationDelay = 3.0f; // Time before starting the narration

    [SerializeField]
    private int currentNarrationIndex = 0;

    [SerializeField]
    private int uniqueDeactivatedLeversCount = 0; // Tracks how many unique levers have been deactivated

    [SerializeField]
    private bool beginNarration = true;

    [SerializeField]
    private bool pauseLeverCount = true;

    [Header("Narration Clips")]
    [SerializeField]
    private bool allRMIDSRNarrationPlayed = false;
    [SerializeField]
    private bool planetNarrationClip1Played = false;

    [SerializeField]
    private string[] sceneNames; // Assign this array in the Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the ExperienceManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Subscribe to the SceneManager's sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //THIS WON"T WORK STARTING FEROM THE MAIN MENU!!! Account for the searching of light pollution manager...
    private void OnEnable()
    {   
        //subcsribe to events in light pollution manager
        if (lightPollutionManager != null)
        {
            lightPollutionManager.OnSocketStateChange += HandleSocketStateChange;
        }
    }

    private void OnDisable()
    {
        if (lightPollutionManager != null)
        {
            lightPollutionManager.OnSocketStateChange -= HandleSocketStateChange;
        }

        // Unsubscribe to avoid any potential memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        //BELOW WONT BE REQUIRED WHEN WE START FROM MAIN MENU. leave as is  for now
        // Start the initial sequence 
        StartCoroutine(InitialNarrationSequence());
        pauseLeverCount = true;
    }

    // This method is called every time a scene is loaded
    //---------------------------------------------------------------------------------- SCENE LOAD EVENT
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "0 - Main Menu":
                // Handle main menu logic, potentially resetting flags
                ResetExperience();
                break;
            case "1 - Street Scene":
                if (beginNarration)
                {
                    beginNarration = false;
                    StartCoroutine(InitialStreetSceneNarration());
                }
                break;
            case "2 - RMIDSR Scene":
                StartCoroutine(PlayRMIDSRNarrationSequence());
                break;
            case "3 - Planet Scene":
                StartCoroutine(PlayPlanetSceneNarrationSequence());
                break;
                // Add cases for other scenes if necessary
        }
    }

    private void ResetExperience()
    {
        beginNarration = true;
        // Reset other flags HERE as needed
    }

    // Street Scene logic
    // --------------------------------------------------------------------------------------- STREET SCENE
    private IEnumerator InitialStreetSceneNarration()
    {
        // Wait a short time before starting the street scene narration sequence
        yield return new WaitForSeconds(1.0f);

        //find light pollution manager as it will not have been set in main menu
        if (lightPollutionManager == null)
            lightPollutionManager = GameObject.FindObjectOfType<LightPollutionManager>(); // as there will only be one
        StartCoroutine(InitialNarrationSequence());

        // NOTE* scene change is handled in EndOfStreetSceneLevelClips() below,
        // it is called at the end of clips played in street scene for now
    }

    // Function to listen for lever/light states
    private void HandleSocketStateChange(int socketIndex, bool isOccupied)
    {
        // Only act on sockets being vacated (turned off)
        if (!isOccupied && !AudioManager.Instance.IsNarrationPlaying() && !pauseLeverCount)
        {
            // Check if the socket has not been deactivated before
            if (!lightPollutionManager.HasSocketBeenDeactivatedBefore(socketIndex))
            {
                // Mark the socket as deactivated
                lightPollutionManager.MarkSocketAsDeactivated(socketIndex);

                // Increment the count of unique deactivated levers
                uniqueDeactivatedLeversCount++;

                // MAYBE activate coroutine to delay play of clip slightly
                // Play the next narration clip
                StartCoroutine(ContinueNarration());

            }
        }

        // Check if all unique levers have been deactivated at least once
        if (uniqueDeactivatedLeversCount >= lightPollutionManager.sockets.Length)
        {
            // All levers are confirmed to have been deactivated at least once
            // Trigger the end-of-level logic
            StartCoroutine(EndOfStreetScene());
        }
    }

    //TODO CREATE LOGIC TOP INITIATE A TRANSITION!

    //Coroutine to call to initiate change
    private IEnumerator EndOfStreetScene()
    {
        Debug.Log("All sockets are empty. lets move on");

        //wait for prevoius clip to end
        yield return new WaitForSeconds(narrationDelay); //Match wait with marrative delay
        //Activate transition

        // Wait for the clip to finish playing before continuing
        while (AudioManager.Instance.IsNarrationPlaying())
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(6.0f); // DEBUG NOTE; ALLOW TIME FOR TRANSITION

        ChangeScene(sceneNames[2]);
    }

    // RMIDSR Scene logic
    // --------------------------------------------------------------------------------------- RMIDSR SCENE
    private IEnumerator PlayRMIDSRNarrationSequence()
    {
        // Wait a short time before starting the narration sequence
        yield return new WaitForSeconds(1.0f);

        // Play each RMIDSR narration clip in sequence
        for (int i = 5; i <= 9; i++)
        {
            //AudioManager.Instance.PlayNarration(i);
            PlayNarrationClip();
            // Wait for the clip to finish playing before continuing
            while (AudioManager.Instance.IsNarrationPlaying())
            {
                yield return null;
            }
            // Optionally, wait a bit between clips
            yield return new WaitForSeconds(1.0f);
        }

        //NOTE ABOVE RUNS TOO FAST, it triggers the last scene before all clips from this scene are read through
        allRMIDSRNarrationPlayed = true;
        yield return new WaitForSeconds(3.0f); // wait next clip to play

        //Trigger FADE TO BLACK.... maybe not

        StartCoroutine(ChangeToPlanetScene());
    }

    private IEnumerator ChangeToPlanetScene()
    {
        // After all clips are done playing, change the scene
        while (AudioManager.Instance.IsNarrationPlaying())
        {
            yield return null;
        }

        ////Trigger FADE TO BLACK and change scene there?

        yield return new WaitForSeconds(3.0f); // wait for fadeout
        if(allRMIDSRNarrationPlayed)
            ChangeScene(sceneNames[3]);
    }

    // Planet Scene logic
    // --------------------------------------------------------------------------------------- PLANET SCENE

    private IEnumerator PlayPlanetSceneNarrationSequence()
    {
        // Wait a short time before starting the planet scene narration sequence
        yield return new WaitForSeconds(1.0f);

        // Play each Planet scene narration clip in sequence
        for (int i = 10; i <= 11; i++)
        {
            //AudioManager.Instance.PlayNarration(i);
            PlayNarrationClip();
            while (AudioManager.Instance.IsNarrationPlaying())
            {
                yield return null;
            }
            yield return new WaitForSeconds(1.0f);
        }

        // Trigger any end-of-experience actions, such as sshowing credits
        // ...

        // Optionally, after all clips and credits are done, provide a way to reset to the main menu
        ShowResetButton();
    }

    public void ShowResetButton()
    {
        // Logic to show a reset button on the screen
        // This button would call a method to load the "Main Menu" scene
    }

    public void ResetToMainMenu()
    {
        SceneManager.LoadScene("0 - Main Menu");
    }

    // NARRATION MANAGEMENT
    //---------------------------------------------------------------------------------------- NARRATION MANAGEMENT
    // Coroutine to Play some Narration with a delay
    private IEnumerator InitialNarrationSequence()
    {
        // Wait for the initial delay before starting the narration
        yield return new WaitForSeconds(initialNarrationDelay);

        // Start the first narration clip
        PlayNarrationClip();
        pauseLeverCount = false; //allow fir counting of first trigger after first clip starts!
    }

    // Coroutine to be used if a delay before starting of next line is required
    private IEnumerator ContinueNarration()
    {
        pauseLeverCount = true;
        // Wait a short time before starting the next narration sequence
        yield return new WaitForSeconds(narrationDelay);

        PlayNarrationClip();
        pauseLeverCount = false;
    }

    // Function to play the next line of narration
    public void PlayNarrationClip()
    {
        // Only play the next clip if the current index is valid and narration is not currently playing
        if (currentNarrationIndex < AudioManager.Instance.narrationClips.Count && !AudioManager.Instance.IsNarrationPlaying())
        {
            AudioManager.Instance.PlayNarration(currentNarrationIndex);
            // Increment the index for the next clip
            currentNarrationIndex++;
        }
        else
        {
            Debug.Log("No more narration clips to play.");
            // Here you can trigger the end of experience, scene change, etc.
        }
    }

    //Scene Management
    // -------------------------------------------------------------------------------------------------------- SCENE MANAGEMENT
    // Call this method when you want to move to the next scene
    public void ChangeScene(string sceneName)
    {
        // Use the SceneManager to load the new scene by name
        SceneManager.LoadScene(sceneName);
    }

    // Call this method when you want to move to Street, to be used from main menu
    public void StartStreetScene()
    {
        // Use the SceneManager to load the new scene by name
        SceneManager.LoadScene(sceneNames[1]);
    }

    //check if the current scene is the Main Menu
    public bool IsMainMenuActive()
    {
        return SceneManager.GetActiveScene().name == sceneNames[0];
    }
    //check if the current scene is the street scene
    public bool IsStreetSceneActive()
    {
        return SceneManager.GetActiveScene().name == sceneNames[1];
    }
    //check if the current scene is the RMIDSR scene
    public bool IsRmidsrSceneActive()
    {
        return SceneManager.GetActiveScene().name == sceneNames[2];
    }
    //check if the current scene is the Planet scene
    public bool IsPlanetSceneActive()
    {
        return SceneManager.GetActiveScene().name == sceneNames[3];
    }

    //add more scene checks if needed

    //need reference to audio manager singleton
    //need reference to light pollution manager
    //need reference to scene manager

    //activate in first scene (main menu scene)

    //listen for what scene is active, if street scene has been activated
    // start narration after initial delay
    // once through first street scene clip played from AudioManager, wait for user to trigger first light off
    // - i.e socket 0 removed
    // also, if socket reactivated, no change will happen, it will not queue the same clip again,
    // it will just continue until next clip is done (this is so user can play with initial switches first)
    // Play second street scene clip, wait for user to trigger next lever
    // play third street scene clip, wait for user to pull ther next lever
    // play fourth street scene clip, wait for user to switch off last light.
    // Play fifth street scene clip.

    // once clip is done, trigger scene change transition event. (this will make some environmental things happen within scene, at the end of which will change scene)
    // wait for scene change trigger point in transition event.
    // change scene to RMIDSR scene

    //queue all RMIDSR scene narration clips and play them in order
    //  (this will play over the top of visuals within scene)

    //when aall clips are done playing, initiate scene change to Planet Scene
    //  (this will be a fade to black)
    //change scene via scene mannager to planet scene

    //play the first planet clip
    //once the clip hasd finished, trigger switch object to appear before the player
    //  (the player hits the switch and turns off all planet lights)
    //play final clip

    //activate credits
    //display reset button to go back to main menu

}
