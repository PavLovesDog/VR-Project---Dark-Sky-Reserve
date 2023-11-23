using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CreditsSceneManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup fadeOverlay;
    [SerializeField]
    private float fadeDuration;
    [SerializeField]
    private float credits1DisplayTime = 10.0f;
    [SerializeField]
    private GameObject[] canvas;
    public string mainMenuSceneName = "0 - Main Menu";
    

    private void Awake()
    {
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

        // Now that we are at black, change canvas visibility
        canvas[0].SetActive(false); //deactivate main menu canvas
        canvas[1].SetActive(true); //activate disclaimer 1 canvas

        // Fade in to show first disclaimer
        yield return StartCoroutine(Fade(0));
        // Wait until fully faded in
        yield return new WaitWhile(() => fadeOverlay.alpha > 0);

        // Credits 1 is visible
        yield return new WaitForSeconds(credits1DisplayTime); //wait an amount of time

        // Fade back to black before showing second Credits canvas
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

        // Credits 2 is visible
        // The next phase is activated by the user gazing at the Reset button (Tagged "continue") in Credits 2 canvas
    }

    private IEnumerator ResetExperience()
    {
        yield return StartCoroutine(Fade(1));

        //Load the experience
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
