using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events; // Add this to use UnityEvent

public class GazeInteraction : MonoBehaviour
{
    public Transform gazeTransform;
    public Image beginLoadingImage;
    public Image continueLoadingImage;
    public Image resetLoadingImage;
    public float gazeTime = 3f;
    public LayerMask interactableLayer;
    public float maxGazeDistance = 100f;

    private float gazeTimer = 0f;
    private bool isGazing = false;
    private bool buttonActivated = false;

    // Define UnityEvents that can be assigned from the Inspector
    public UnityEvent onBeginButtonGazeComplete;
    public UnityEvent onContinueButtonGazeComplete;
    public UnityEvent onResetButtonGazeComplete;

    private void Start()
    {
        buttonActivated = false;
    }

    private void Update()
    {
        Ray ray = new Ray(gazeTransform.position, gazeTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGazeDistance, interactableLayer))
        {
            Image loadingImage = null;
            UnityEvent gazeEvent = null;

            if (hit.collider.CompareTag("BeginButton"))
            {
                loadingImage = beginLoadingImage;
                gazeEvent = onBeginButtonGazeComplete;
            }
            else if (hit.collider.CompareTag("ContinueButton"))
            {
                loadingImage = continueLoadingImage;
                gazeEvent = onContinueButtonGazeComplete;
            }
            else if(hit.collider.CompareTag("ResetButton"))
            {
                loadingImage = resetLoadingImage;
                gazeEvent = onResetButtonGazeComplete;
            }

            if (loadingImage != null && gazeEvent != null)
            {
                if (!isGazing)
                {
                    isGazing = true;
                    gazeTimer = 0f;
                    if(!buttonActivated) // if the button hasnt activated and the player looks away
                        loadingImage.fillAmount = 0f; // reset fill
                }

                gazeTimer += Time.deltaTime;
                loadingImage.fillAmount = gazeTimer / gazeTime;

                if (gazeTimer >= gazeTime)
                {
                    buttonActivated = true; // player has activated button, keepo fill amount
                    loadingImage.fillAmount = 1f;
                    gazeEvent.Invoke(); // Invoke the event
                    //isGazing = false; // Reset gaze
                }
            }
        }
        else if (isGazing)
        {
            isGazing = false;
            if(beginLoadingImage != null)
               beginLoadingImage.fillAmount = 0f;
            if (continueLoadingImage != null)
                continueLoadingImage.fillAmount = 0f;
            if (resetLoadingImage != null)
                resetLoadingImage.fillAmount = 0f;
        }
    }

    // Draw Gizmos in the Editor to visualize the gaze direction
    private void OnDrawGizmos()
    {
        if (gazeTransform != null)
        {
            Gizmos.color = Color.blue;
            Vector3 direction = gazeTransform.TransformDirection(Vector3.forward) * maxGazeDistance;
            Gizmos.DrawRay(gazeTransform.position, direction);
        }
    }

    public void ReloadMainMenuFromGame()
    {
        StartCoroutine(ExperienceManager.Instance.ResetExperience());
    }

    //---------------------------------------------------------------------------------OLD

    //public Transform gazeTransform; // Assign the VR camera or head transform here
    //public Image beginLoadingImage; // Assign a UI Image that will act as the loading reticule or animation
    //public Image continueLoadingImage; // Assign a UI Image that will act as the loading reticule or animation
    //public float gazeTime = 3f; // Time in seconds the player has to gaze at the button
    //public LayerMask interactableLayer; // Layer mask to filter only interactable objects
    //public float maxGazeDistance = 100f; // Set the max distance for the gaze
    //
    //[Header("Scene Change")]
    //public string streetSceneName = "1 - Street Scene"; // The name of the scene you want to load
    //[SerializeField]
    //private CanvasGroup fadeOverlay;
    //[SerializeField]
    //private float fadeDuration;
    //[SerializeField]
    //private float canvasDisplayTime;
    //
    //[Header("Canvas Control")]
    //[SerializeField]
    //private GameObject[] canvas;
    //
    //private float gazeTimer = 0f;
    //private bool isGazing = false;
    //
    //private void Update()
    //{
    //    // Cast a ray from the camera/player's position forward
    //    Ray ray = new Ray(gazeTransform.position, gazeTransform.forward);
    //    RaycastHit hit;
    //
    //    if (Physics.Raycast(ray, out hit, maxGazeDistance, interactableLayer))
    //    {
    //        // Check if the collider has a tag "BeginButton" or a specific component if needed
    //        if (hit.collider.CompareTag("BeginButton"))
    //        {
    //            // Player is gazing at the button
    //            if (!isGazing)
    //            {
    //                isGazing = true;
    //                gazeTimer = 0f; // Reset the timer
    //                beginLoadingImage.fillAmount = 0f; // Reset the loading image
    //            }
    //
    //            // Increment the timer
    //            gazeTimer += Time.deltaTime;
    //            beginLoadingImage.fillAmount = gazeTimer / gazeTime; // Update the loading image
    //
    //            // If the gaze time has been reached
    //            if (gazeTimer >= gazeTime)
    //            {
    //                //fade out &  Load the target scene
    //                StartCoroutine(ShowDisclaimers());
    //
    //                //Instead activate scene specific manager to do the scene logic
    //            }
    //        }
    //
    //        // Check if the collider has a tag "BeginButton" or a specific component if needed
    //        if (hit.collider.CompareTag("ContinueButton"))
    //        {
    //            // Player is gazing at the button
    //            if (!isGazing)
    //            {
    //                isGazing = true;
    //                gazeTimer = 0f; // Reset the timer
    //                continueLoadingImage.fillAmount = 0f; // Reset the loading image
    //            }
    //
    //            // Increment the timer
    //            gazeTimer += Time.deltaTime;
    //            continueLoadingImage.fillAmount = gazeTimer / gazeTime; // Update the loading image
    //
    //            // If the gaze time has been reached
    //            if (gazeTimer >= gazeTime)
    //            {
    //                //fade out &  Load the target scene
    //                StartCoroutine(BeginExperience());
    //            }
    //        }
    //    }
    //    else
    //    {
    //        // Player is not gazing at the button
    //        if (isGazing)
    //        {
    //            isGazing = false;
    //            beginLoadingImage.fillAmount = 0f; // Reset the loading image
    //            continueLoadingImage.fillAmount = 0f;
    //        }
    //    }
    //}
    //
    //// Draw Gizmos in the Editor to visualize the gaze direction
    //private void OnDrawGizmos()
    //{
    //    if (gazeTransform != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        Vector3 direction = gazeTransform.TransformDirection(Vector3.forward) * maxGazeDistance;
    //        Gizmos.DrawRay(gazeTransform.position, direction);
    //    }
    //}
    //
    //private IEnumerator ShowDisclaimers()
    //{
    //    // Fade to black
    //    yield return StartCoroutine(Fade(1));
    //    // Wait until fully faded to black
    //    yield return new WaitWhile(() => fadeOverlay.alpha < 1);
    //
    //    // Now that we are at black, change canvas visibility
    //    canvas[0].SetActive(false); //deactivate main menu canvas
    //    canvas[1].SetActive(true); //activate disclaimer 1 canvas
    //
    //    // Fade in to show first disclaimer
    //    yield return StartCoroutine(Fade(0));
    //    // Wait until fully faded in
    //    yield return new WaitWhile(() => fadeOverlay.alpha > 0);
    //
    //    // Disclaimer 1 is visible
    //    yield return new WaitForSeconds(canvasDisplayTime); //wait an amount of time
    //
    //    // Fade back to black before showing second disclaimer
    //    yield return StartCoroutine(Fade(1));
    //    // Wait until fully faded to black again
    //    yield return new WaitWhile(() => fadeOverlay.alpha < 1);
    //    yield return new WaitForSeconds(2);
    //
    //   // Screen is black, safe to change canvas visibility
    //   canvas[1].SetActive(false); //deactivate disclaimer 1 canvas
    //    canvas[2].SetActive(true); //activate disclaimer 2 canvas
    //
    //    // Fade in to show second disclaimer
    //    yield return StartCoroutine(Fade(0));
    //    // Wait until fully faded in
    //    yield return new WaitWhile(() => fadeOverlay.alpha > 0);
    //
    //    // Disclaimer 2 is visible
    //    // The next phase is activated by the user gazing at the continue button in disclaimer 2 canvas
    //}
    //
    //private IEnumerator BeginExperience()
    //{
    //    yield return StartCoroutine(Fade(1));
    //
    //    //Load the experience
    //    SceneManager.LoadScene(streetSceneName);
    //}
    //
    //
    //private IEnumerator Fade(float targetAlpha)
    //{
    //    float startAlpha = fadeOverlay.alpha;
    //    float time = 0f;
    //
    //    while (time < fadeDuration)
    //    {
    //        time += Time.deltaTime;
    //        fadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
    //        yield return null;
    //    }
    //
    //    fadeOverlay.alpha = targetAlpha; // Ensure it ends at the targetAlpha
    //}
}
