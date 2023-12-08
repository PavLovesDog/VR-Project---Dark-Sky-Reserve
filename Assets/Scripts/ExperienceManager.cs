using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//using UnityEditor.Presets;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [SerializeField]
    private LightPollutionManager lightPollutionManager; // Assign in inspector

    [Header("Narration Variables")]
    [SerializeField]
    private float initialNarrationDelay = 5.0f; // Time before starting the narration
    [SerializeField]
    private float narrationDelay = 3.0f; // Time before starting the narration
    [SerializeField]
    public int currentNarrationIndex = 0;
    [SerializeField]
    private int uniqueDeactivatedLeversCount = 0; // Tracks how many unique levers have been deactivated
    [SerializeField]
    private bool beginNarration = true;
    [SerializeField]
    public bool pauseLeverCount = true;
    [SerializeField]
    public bool canInteractLever = false;

    [Header("Narration Clips")]
    [SerializeField]
    private bool allRMIDSRNarrationPlayed = false;
    [SerializeField]
    private bool planetNarrationClip1Played = false;

    [Header("UI Elements")]
    [SerializeField]
    private Canvas[] leverUIs;

    [Header("Bloom Indicators")]
    [SerializeField]
    private GrowShrink[] bloom;

    [Header("Scene Management")]
    [SerializeField]
    private string[] sceneNames; // Assign this array in the Inspector
    [SerializeField]
    private CanvasGroup fadeOverlay;
    [SerializeField]
    private float fadeDuration;

    [Header("Current Scene")]
    [SerializeField]
    private bool inMainMenu;
    [SerializeField]
    private bool inStreetScene;
    [SerializeField]
    private bool inRmidsrScene;
    [SerializeField]
    private bool inPlanetScene;
    [SerializeField]
    private bool inCreditsScene;

    [Header("Reset Management")]
    public Coroutine RMIDSRNarrationCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //gameObject.tag = "PersistentObject"; // tag for easy deletion in end //DON'T DELETE
            DontDestroyOnLoad(gameObject); // Keep the ExperienceManager across scenes

            // Subscribe to the SceneManager's sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        pauseLeverCount = true;
    }

    // This method is called every time a scene is loaded
    //---------------------------------------------------------------------------------- SCENE LOAD EVENT
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Add a check to ensure this is the valid instance
        if (this != Instance) return;

        switch (scene.name)
        {
            case "0 - Main Menu":
                // Handle main menu logic, potentially resetting flags
                inCreditsScene = false;
                inMainMenu = true;
                ResetExperience();
                break;

            case "1 - Street Scene":
                SetExperienceManagerSettings(); // set up the inital settings of the ExperienceManger
                currentNarrationIndex = 0; // set index directly
                inStreetScene = true;
                inMainMenu = false;
                //FadeIn
                StartCoroutine(FadeIn()); // THIS is line 120
                if (beginNarration)
                {
                    beginNarration = false;
                    StartCoroutine(InitialStreetSceneNarration());
                }
                break;

            case "2 - RMIDSR Scene":
                currentNarrationIndex = 5; // set index directly
                inRmidsrScene = true;
                inStreetScene = false;
                StartCoroutine(FadeIn());
                RMIDSRNarrationCoroutine = StartCoroutine(PlayRMIDSRNarrationSequence());
                break;

            case "3 - Planet Scene":
                currentNarrationIndex = 12; // set index directly
                //FadeIn handled in position manager
                //StartCoroutine(FadeIn()); // THIS MIGHT BREAK!? may need to re-find the fade canvas?
                inPlanetScene = true;
                inRmidsrScene = false;
                StartCoroutine(PlayPlanetSceneNarrationSequence());
                break;
            // Add cases for other scenes if necessary

            case "4 - Credits":
                inPlanetScene = false;
                inCreditsScene = true;
                StartCoroutine(FadeIn());
                StartCoroutine(StartCreditSceneLogic());
                break;
        }
    }

    
    #region Street Scene Logic
    // Street Scene logic
    // --------------------------------------------------------------------------------------- STREET SCENE
    
    //Function to set up necessary references for Experienmce Manager
    private void SetExperienceManagerSettings()
    {
        // Assign leverUIs Canvases by tag
        leverUIs[0] = GameObject.FindGameObjectWithTag("Lever1Canvas").GetComponent<Canvas>();
        leverUIs[1] = GameObject.FindGameObjectWithTag("Lever2Canvas").GetComponent<Canvas>();
        leverUIs[2] = GameObject.FindGameObjectWithTag("Lever3Canvas").GetComponent<Canvas>();
        leverUIs[3] = GameObject.FindGameObjectWithTag("Lever4Canvas").GetComponent<Canvas>();

        // Assign bloomIndicators GrowShrink objects by name
        bloom[0] = GameObject.Find("sphere_bloom_red").GetComponent<GrowShrink>();
        bloom[1] = GameObject.Find("sphere_bloom_red (1)").GetComponent<GrowShrink>();
        bloom[2] = GameObject.Find("sphere_bloom_red (2)").GetComponent<GrowShrink>();
        bloom[3] = GameObject.Find("sphere_bloom_red (3)").GetComponent<GrowShrink>();

        // Assign fade canvas
        fadeOverlay = GameObject.FindGameObjectWithTag("FadeCanvas").GetComponent<CanvasGroup>();

        //assign light pollution manager for street scene
        lightPollutionManager = GameObject.FindObjectOfType<LightPollutionManager>();

        //subcsribe to events in light pollution manager
        if (lightPollutionManager != null)
        {
            lightPollutionManager.OnSocketStateChange += HandleSocketStateChange;
        }

    }

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

    // Coroutine to start the narration with a delay
    private IEnumerator InitialNarrationSequence()
    {
        // Wait for the initial delay before starting the narration
        yield return new WaitForSeconds(initialNarrationDelay);

        // Start the first narration clip
        PlayStreetNarrationClip();

        //Wait for first narration clip to finish
        yield return new WaitWhile(() => AudioManager.Instance.IsNarrationPlaying());
        pauseLeverCount = false; //allow for counting of first trigger after first clip starts!

        leverUIs[0].enabled = true; //Allow for first UI element to be visible
        bloom[0].oscilate = true; // set first levers bloom to expand/contract
        canInteractLever = true;// activate ability to hit levers
    }

    // Coroutine to be used if a delay before starting of next line is required
    private IEnumerator ContinueNarration()
    {
        pauseLeverCount = true;
        // Wait a short time before starting the next narration sequence
        yield return new WaitForSeconds(narrationDelay);

        PlayStreetNarrationClip();

        //Wait for narration clip to finish
        yield return new WaitWhile(() => AudioManager.Instance.IsNarrationPlaying());
        
        pauseLeverCount = false; // allow counting of levers
        canInteractLever = true; // allow use of levers

        //display next Lever UI
        if (uniqueDeactivatedLeversCount == 1)
        {
            leverUIs[1].enabled = true;
            bloom[1].oscilate = true;
        }
        else if (uniqueDeactivatedLeversCount == 2)
        {
            leverUIs[2].enabled = true;
            bloom[2].oscilate = true;
        }
        else if (uniqueDeactivatedLeversCount == 3)
        {
            leverUIs[3].enabled = true;
            bloom[3].oscilate = true;
        }
    }

    // Function to listen for lever/light states
    private void HandleSocketStateChange(int socketIndex, bool isOccupied)
    {
        // Only act on sockets being vacated (turned off)
        if (!isOccupied && !AudioManager.Instance.IsNarrationPlaying() && !pauseLeverCount)
        {
            // Check if the socket has not been deactivated before                    // Ensure Only the lever representing the narration is selected
            if (!lightPollutionManager.HasSocketBeenDeactivatedBefore(socketIndex)/* && socketIndex == currentNarrationIndex - 1*/)
            {
                // Mark the socket as deactivated
                lightPollutionManager.MarkSocketAsDeactivated(socketIndex);

                // Increment the count of unique deactivated levers
                uniqueDeactivatedLeversCount++;

                //deactiuvate UI elements
                foreach(Canvas currentUI in leverUIs)
                    currentUI.enabled = false;

                //deactivate bloom oscilations
                foreach (GrowShrink blooms in bloom)
                    blooms.oscilate = false;

                canInteractLever = false; // Stop interactions with Levers

                // Play the next narration clip with slight delay
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

    //Coroutine to call to initiate change
    private IEnumerator EndOfStreetScene()
    {
        //Debug.Log("All sockets are empty. lets move on");

        //wait for prevoius clip to end
        yield return new WaitForSeconds(narrationDelay + 1f); //Match wait with marrative delay
        //Activate transition

        // Wait for the clip to finish playing before continuing
        while (AudioManager.Instance.IsNarrationPlaying())
        {
            yield return null;
        }
        currentNarrationIndex++; // incremennt index -------------------------------------------------------------------------INDEX INCREMENT

        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(4.5f); 

        ChangeScene(sceneNames[2]);
    }
#endregion

    #region RMIDSR Scene Logic
    // RMIDSR Scene logic
    // --------------------------------------------------------------------------------------- RMIDSR SCENE
    private IEnumerator PlayRMIDSRNarrationSequence()
    {
        // Wait a short time before starting the narration sequence
        yield return new WaitForSeconds(1.0f);

        // Start playing from the current index (which should be set to 5 when this scene starts)
        while (currentNarrationIndex <= 11)
        {
            // Only enqueue the clip if the narration is not playing
            if (!AudioManager.Instance.IsNarrationPlaying())
            {
                AudioManager.Instance.PlayNarration(currentNarrationIndex);
            }

            if(currentNarrationIndex == 7) // PLANTS talk starts at narration index 7 
            {
                //Start visual manager to show PLANT Light Drawings
                StartCoroutine(RMIDSRVisualsManager.Instance.VisualsSequence(RMIDSRVisualsManager.Instance.sections[0]));
            }
            else if (currentNarrationIndex == 8) // POLLINATORS
            {
                // show POLLINATOR light drawings
                StartCoroutine(RMIDSRVisualsManager.Instance.VisualsSequence(RMIDSRVisualsManager.Instance.sections[1]));
            }
            else if (currentNarrationIndex == 10) // FAUNA
            {
                // show FAUNA light drawings
                StartCoroutine(RMIDSRVisualsManager.Instance.VisualsSequence(RMIDSRVisualsManager.Instance.sections[2]));
            }
            else if (currentNarrationIndex == 11) // HUMANS
            {
                // show HUMANS light drawings
                StartCoroutine(RMIDSRVisualsManager.Instance.VisualsSequence(RMIDSRVisualsManager.Instance.sections[3]));
            }

            yield return new WaitWhile(() => AudioManager.Instance.IsNarrationPlaying());
            currentNarrationIndex++; // incremennt index -------------------------------------------------------------------------INDEX INCREMENT

            // Additionally, wait a bit between clips
            yield return new WaitForSeconds(1.0f);


        }

        // After all clips are done playing, proceed to change the scene
        ChangeToPlanetScene();
    }

    private void ChangeToPlanetScene()
    {
        // Change the scene after a delay for the fade to black
        StartCoroutine(DelayedSceneChange());
    }

    private IEnumerator DelayedSceneChange()
    {
        // Wait for the fade out before changing the scene
        yield return StartCoroutine(FadeOut());

        //yield return new WaitForSeconds(0.5f);
        ChangeScene(sceneNames[3]);
    }
    #endregion

    #region Planet Scene Logic
    // Planet Scene logic
    // --------------------------------------------------------------------------------------- PLANET SCENE
    private IEnumerator PlayPlanetSceneNarrationSequence() // Narration Index now goes from 12-14
    {
        yield return new WaitForSeconds(2.0f);
        //play the first narration clip
        AudioManager.Instance.PlayNarration(currentNarrationIndex);
        yield return new WaitWhile(() => AudioManager.Instance.narrationSource.isPlaying); // wait for it to complete
        currentNarrationIndex++; // increment the narration index.

        //PLAY NEXT NARRATIUIVE LINE
        AudioManager.Instance.PlayNarration(currentNarrationIndex);
        yield return new WaitWhile(() => AudioManager.Instance.narrationSource.isPlaying); // wait for it to complete
        currentNarrationIndex++;

        //wait for bool to trigger from the lever control script
        yield return new WaitWhile(() => PlanetLeverControl.Instance.isOn);
        //once bool is tripped (this will be the turning off of the lights. i.e. level pulled down)
        yield return new WaitForSeconds(1.5f);
        // play the final clip
        AudioManager.Instance.PlayNarration(currentNarrationIndex);

        //wait for a few seconds
        yield return new WaitForSeconds(4.0f);
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(2.0f);
        //Fade out and change scene to credit scene?
        ChangeScene(sceneNames[4]); // load credits
        //begin credit sequence?
        //display reset button, interacatble by gazing

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

    #endregion

    #region Credits Scene Logic
    // Credits Scene logic
    // --------------------------------------------------------------------------------------- CREDITS SCENE
    private IEnumerator StartCreditSceneLogic()
    {
        //// wait for 10 seconds so people can read first screen
        yield return new WaitForEndOfFrame();


        //engage the following screens
        CreditsSceneManager.Instance.ShowCreditsWrapper();
    }

    #endregion

    #region Narration Management
    // NARRATION MANAGEMENT
    //---------------------------------------------------------------------------------------- NARRATION MANAGEMENT


    // Function to play the next line of narration NOTE WHY AM I USING THIS?? Seems like an useless added function??
    public void PlayStreetNarrationClip()
    {
        // if narration is not currently playing
        if (!AudioManager.Instance.IsNarrationPlaying())
        {
            // Only play the next clip if the current index is valid
            if (currentNarrationIndex < AudioManager.Instance.narrationClips.Count)
            {
                AudioManager.Instance.PlayNarration(currentNarrationIndex);
                //Increment the index for the next clip
                currentNarrationIndex++; // incremennt index -------------------------------------------------------------------------INDEX INCREMENT
            }
            else
            {
                Debug.Log("No more narration clips to play");
                // Here you can trigger the end of experience, scene change, etc.
            }
        }
        else
        {
            Debug.Log("Audio Running: Clip still playing narration");
        }
    }

    #endregion

    #region Scene Management
    //Scene Management
    // -------------------------------------------------------------------------------------------------------- SCENE MANAGEMENT
    //Function to Reset the experience by returning to the main menu
    public IEnumerator ResetExperience()
    {

        yield return new WaitForEndOfFrame();

        // Reset flags HERE as needed
        uniqueDeactivatedLeversCount = 0; // reset lever counter
        currentNarrationIndex = 0;
        beginNarration = true;
        pauseLeverCount = true;
        canInteractLever = false;
        allRMIDSRNarrationPlayed = false;
        planetNarrationClip1Played = false;

        //stop any audio
        if (AudioManager.Instance.IsNarrationPlaying())
        {
            AudioManager.Instance.StopSFX();
            AudioManager.Instance.StopNarration();
        }

        //Stop scene specific Coroutines
        if(inRmidsrScene)
        {
            StopCoroutine(RMIDSRNarrationCoroutine);
        }

        // Clear the HashSet before destroying persistent objects
        PersistGameObject.ClearCreatedObjects();

        yield return new WaitForEndOfFrame();

        //destroy persistent objects
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("PersistentObject");
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(FadeOut());

        yield return new WaitForEndOfFrame();

        //ReLoad the experience back to main menu
        SceneManager.LoadScene(sceneNames[0]);
    }
    private IEnumerator FadeOut()
    {
        //check if fade overlay is not null
        if (fadeOverlay != null)
            yield return StartCoroutine(Fade(1));
        else
        {
            // find and set it
            fadeOverlay = GameObject.FindGameObjectWithTag("FadeCanvas").GetComponent<CanvasGroup>();
            if (fadeOverlay != null)
                yield return StartCoroutine(Fade(1));
        }
    }

    private IEnumerator FadeIn()
    {
        // Check if the ExperienceManager itself is still valid
        if (this == null) yield break;

        //check if fade overlay is not null
        if (fadeOverlay != null)
            yield return StartCoroutine(Fade(0));
        else
        {
            // find and set it
            fadeOverlay = GameObject.FindGameObjectWithTag("FadeCanvas").GetComponent<CanvasGroup>();
            if (fadeOverlay != null)
                yield return StartCoroutine(Fade(0));
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeOverlay.alpha;
        float time = 0f;

        if (fadeOverlay == null) // double catch..
            fadeOverlay = GameObject.FindGameObjectWithTag("FadeCanvas").GetComponent<CanvasGroup>();

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            if(fadeOverlay != null) // catch for reset from mid scene
                fadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        if (fadeOverlay != null) // catch for reset from mid scene
            fadeOverlay.alpha = targetAlpha; // Ensure it ends at the targetAlpha
    }

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

    #endregion


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
