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

    [Header("Gaze Visuals")]
    public GameObject gazeIndicatorSprite; // Sprite to show where the player is looking
    public float gazeIndicatorDistance = 5f; // Adjustable distance for the gaze indicator

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

        // Always position the gaze indicator sprite along the ray
        if (gazeIndicatorSprite != null)
        {
            Vector3 spritePosition = ray.origin + ray.direction * gazeIndicatorDistance;
            gazeIndicatorSprite.transform.position = spritePosition;
        }

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
                    StartCoroutine(KeepImageFill()); // player has activated button, keepo fill amount
                    loadingImage.fillAmount = 1f;
                    gazeEvent.Invoke(); // Invoke the event
                    //isGazing = false; // Reset gaze
                }
            }
        }
        else if (isGazing & !buttonActivated)
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

    IEnumerator KeepImageFill()
    {
        buttonActivated = true;
        yield return new WaitForSeconds(gazeTime + 2);
        buttonActivated = false;
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
}
