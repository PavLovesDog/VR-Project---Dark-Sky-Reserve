using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup fadeOverlay;
    [SerializeField]
    private float fadeDuration;
    [SerializeField]
    private float canvasDisplayTime;
    [SerializeField]
    private GameObject[] canvas;
    public string streetSceneName = "1 - Street Scene";

    private void Awake()
    {
        // Find the GazeInteraction script and hook up the events
        GazeInteraction gazeInteraction = FindObjectOfType<GazeInteraction>();
        if (gazeInteraction)
        {
            gazeInteraction.onBeginButtonGazeComplete.AddListener(ShowDisclaimersWrapper);
            gazeInteraction.onContinueButtonGazeComplete.AddListener(BeginExperienceWrapper);
        }
    }

    // Wrapper methods to start coroutines
    public void ShowDisclaimersWrapper()
    {
        StartCoroutine(ShowDisclaimers());
    }

    public void BeginExperienceWrapper()
    {
        StartCoroutine(BeginExperience());
    }

    private IEnumerator ShowDisclaimers()
    {
        // Fade to black
        yield return StartCoroutine(Fade(1));
        // Wait until fully faded to black
        yield return new WaitWhile(() => fadeOverlay.alpha < 1);

        // Now that we are at black, change canvas visibility
        canvas[0].SetActive(false); //deactivate main menu canvas
        canvas[1].SetActive(true); //activate disclaimer 1 canvas

        // Fade in to show first disclaimer
        yield return StartCoroutine(Fade(0));
        // Wait until fully faded in
        yield return new WaitWhile(() => fadeOverlay.alpha > 0);

        // Disclaimer 1 is visible
        yield return new WaitForSeconds(canvasDisplayTime); //wait an amount of time

        // Fade back to black before showing second disclaimer
        yield return StartCoroutine(Fade(1));
        // Wait until fully faded to black again
        yield return new WaitWhile(() => fadeOverlay.alpha < 1);
        yield return new WaitForSeconds(2);

        // Screen is black, safe to change canvas visibility
        canvas[1].SetActive(false); //deactivate disclaimer 1 canvas
        canvas[2].SetActive(true); //activate disclaimer 2 canvas

        // Fade in to show second disclaimer
        yield return StartCoroutine(Fade(0));
        // Wait until fully faded in
        yield return new WaitWhile(() => fadeOverlay.alpha > 0);

        // Disclaimer 2 is visible
        // The next phase is activated by the user gazing at the continue button in disclaimer 2 canvas
    }

    private IEnumerator BeginExperience()
    {
        yield return StartCoroutine(Fade(1));

        //Load the experience
        SceneManager.LoadScene(streetSceneName);
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
