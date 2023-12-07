using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditsSceneManager : MonoBehaviour
{
    public static CreditsSceneManager Instance { get; private set; }

    [SerializeField]
    private CanvasGroup fadeOverlay;
    [SerializeField]
    private float fadeDuration;
    [SerializeField]
    private float[] creditsDisplayTime;
    [SerializeField]
    private GameObject[] canvas;
    public string mainMenuSceneName = "0 - Main Menu";
    

    private void Awake()
    {
        //create singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Find the GazeInteraction script and hook up the events
        GazeInteraction gazeInteraction = FindObjectOfType<GazeInteraction>();
        if (gazeInteraction)
        {
            gazeInteraction.onBeginButtonGazeComplete.AddListener(ShowCreditsWrapper);
            gazeInteraction.onContinueButtonGazeComplete.AddListener(ResetExperienceWrapper);
        }
    }

    // Wrapper methods to start coroutines
    public void ShowCreditsWrapper()
    {
        StartCoroutine(ShowCredits());
    }

    public void ResetExperienceWrapper()
    {
        StartCoroutine(ResetExperience());
    }

    private IEnumerator ShowCredits()
    {
        // Fade to black
        yield return StartCoroutine(Fade(1));
        // Wait until fully faded to black
        yield return new WaitWhile(() => fadeOverlay.alpha < 1);

        // Iterate over all canvases except the last one
        for (int i = 0; i < canvas.Length - 1; i++)
        {
            // Deactivate the previous canvas (if not the first iteration)
            if (i > 0)
            {
                canvas[i - 1].SetActive(false);
            }

            // Activate the current canvas
            canvas[i].SetActive(true);

            // Fade in to show current canvas
            yield return StartCoroutine(Fade(0));
            // Wait until fully faded in
            yield return new WaitWhile(() => fadeOverlay.alpha > 0);

            // Display current canvas for a set amount of time
            yield return new WaitForSeconds(creditsDisplayTime[i]); // Assuming an array of display times for each canvas

            // Fade back to black before showing the next canvas
            yield return StartCoroutine(Fade(1));
            // Wait until fully faded to black
            yield return new WaitWhile(() => fadeOverlay.alpha < 1);
        }
        //turn off last canvas from loop
        canvas[canvas.Length - 2].SetActive(false);
        
        // After all canvases have been shown, show the final canvas
        canvas[canvas.Length - 1].SetActive(true);

        // Fade in for the final canvas
        yield return StartCoroutine(Fade(0));
        // Wait until fully faded in
        yield return new WaitWhile(() => fadeOverlay.alpha > 0);

        // The final canvas is now visible
        // The next phase is activated by the user gazing at the Reset button in the final canvas
        //// Fade to black
        //yield return StartCoroutine(Fade(1));
        //// Wait until fully faded to black
        //yield return new WaitWhile(() => fadeOverlay.alpha < 1);
        //
        //// Now that we are at black, change canvas visibility
        //canvas[0].SetActive(false); //deactivate main menu canvas
        //canvas[1].SetActive(true); //activate disclaimer 1 canvas
        //
        //// Fade in to show first disclaimer
        //yield return StartCoroutine(Fade(0));
        //// Wait until fully faded in
        //yield return new WaitWhile(() => fadeOverlay.alpha > 0);
        //
        //// Credits 1 is visible
        //yield return new WaitForSeconds(credits1DisplayTime); //wait an amount of time
        //
        //// Fade back to black before showing second Credits canvas
        //yield return StartCoroutine(Fade(1));
        //// Wait until fully faded to black again
        //yield return new WaitWhile(() => fadeOverlay.alpha < 1);
        //yield return new WaitForSeconds(2);
        //
        //// Screen is black, safe to change canvas visibility
        //canvas[1].SetActive(false); //deactivate disclaimer 1 canvas
        //canvas[2].SetActive(true); //activate disclaimer 2 canvas
        //
        //// Fade in to show second disclaimer
        //yield return StartCoroutine(Fade(0));
        //// Wait until fully faded in
        //yield return new WaitWhile(() => fadeOverlay.alpha > 0);
        //
        //// Credits 2 is visible
        //
        //
        //// The next phase is activated by the user gazing at the Reset button (Tagged "continue") in Credits 2 canvas
    }

    private IEnumerator ResetExperience()
    {
        yield return StartCoroutine(Fade(1)); // fade out

        //destroy persistent objects
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("PersistentObject");
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }

        //ReLoad the experience back to main menu
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeOverlay.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeOverlay.alpha = targetAlpha; // Ensure it ends at the targetAlpha
    }
}
