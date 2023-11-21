using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GazeInteraction : MonoBehaviour
{
    public Transform gazeTransform; // Assign the VR camera or head transform here
    public Image loadingImage; // Assign a UI Image that will act as the loading reticule or animation
    public float gazeTime = 3f; // Time in seconds the player has to gaze at the button
    public LayerMask interactableLayer; // Layer mask to filter only interactable objects
    public float maxGazeDistance = 100f; // Set the max distance for the gaze

    [Header("Scene Change")]
    public string streetSceneName = "1 - Street Scene"; // The name of the scene you want to load
    [SerializeField]
    private CanvasGroup fadeOverlay;
    [SerializeField]
    private float fadeDuration;

    private float gazeTimer = 0f;
    private bool isGazing = false;

    private void Update()
    {
        // Cast a ray from the camera/player's position forward
        Ray ray = new Ray(gazeTransform.position, gazeTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxGazeDistance, interactableLayer))
        {
            // Check if the collider has a tag "BeginButton" or a specific component if needed
            if (hit.collider.CompareTag("BeginButton"))
            {
                // Player is gazing at the button
                if (!isGazing)
                {
                    isGazing = true;
                    gazeTimer = 0f; // Reset the timer
                    loadingImage.fillAmount = 0f; // Reset the loading image
                }

                // Increment the timer
                gazeTimer += Time.deltaTime;
                loadingImage.fillAmount = gazeTimer / gazeTime; // Update the loading image

                // If the gaze time has been reached
                if (gazeTimer >= gazeTime)
                {
                    //fade out &  Load the target scene
                    StartCoroutine(FadeOut());
                }
            }
        }
        else
        {
            // Player is not gazing at the button
            if (isGazing)
            {
                isGazing = false;
                loadingImage.fillAmount = 0f; // Reset the loading image
            }
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

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(1));
        //Show Disclaimer,
        //wait for moment
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
