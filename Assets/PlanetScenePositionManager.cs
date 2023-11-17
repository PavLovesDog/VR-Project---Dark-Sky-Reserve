using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScenePositionManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField]
    private GameObject user;

    [SerializeField]
    private GameObject lightPollution;

    [SerializeField]
    private Transform[] positions; // Set these in the inspector

    [SerializeField]
    private CanvasGroup fadeOverlay; // Assign a UI CanvasGroup that will act as the fade overlay

    [Header("Timing")]
    [SerializeField]
    private float fadeDuration = 2.0f; // Time it takes to fade in/out

    [SerializeField]
    private float waitTimeAtStart = 2.0f; // Time to wait at the start

    [SerializeField]
    private float waitTimeBeforeTeleport = 1.0f; // Time to wait before teleporting

    [SerializeField]
    private float waitTimeAtFinal = 4.0f; // Time to wait at the final position

    [SerializeField]
    private float moveSpeed = 1.0f; // Speed of translation between final positions

    [Header("Light Switch")]
    [SerializeField]
    private GameObject finalLever; // The final lever to activate

    private int currentPositionIndex = 0; // Tracks the current position index

    void Start()
    {
        StartCoroutine(PositionSequence());
        finalLever.SetActive(false); // maybe access and just turn of the renderer
        lightPollution.SetActive(false);
    }

    private IEnumerator PositionSequence()
    {
        // Start at position 1
        TeleportToPosition(0);
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(waitTimeAtStart);

        // Fade to black and teleport to position 2
        yield return StartCoroutine(FadeOut());
        TeleportToPosition(1);
        yield return StartCoroutine(FadeIn());

        // Begin moving to position 3
        StartCoroutine(MoveToPosition(2));
    }

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(Fade(1));
    }

    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0));
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

    private void TeleportToPosition(int index)
    {
        if (index == 1) //moved to second position
            lightPollution.SetActive(true);

        if (index >= 0 && index < positions.Length)
        {
            user.transform.position = positions[index].position;
            user.transform.rotation = positions[index].rotation;
            currentPositionIndex = index;
        }
    }

    private IEnumerator MoveToPosition(int index)
    {
        if (index >= 0 && index < positions.Length)
        {
            Vector3 startPosition = user.transform.position;
            Quaternion startRotation = user.transform.rotation;
            Vector3 endPosition = positions[index].position;
            Quaternion endRotation = positions[index].rotation;

            float distance = Vector3.Distance(startPosition, endPosition);
            float journeyLength = distance / moveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < journeyLength)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / journeyLength;

                // Easing effect - here using a simple ease out quadratic function, t * t
                // You can replace this with any other easing function as needed
                t = t * t;

                user.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }

            // Confirm the final position and rotation
            user.transform.position = endPosition;
            user.transform.rotation = endRotation;

            // Wait at the final position
            yield return new WaitForSeconds(waitTimeAtFinal);

            // Activate the final lever here if automatic, or just allow the player to interact with it
            finalLever.SetActive(true);
        }
    }

    // Public method to call when the final lever is activated, if required
    public void OnFinalLeverActivated()
    {
        // Trigger the final piece of narration or any other required action
    }

    //have access to 3 locations/positions (set in inspector)
    //have access to a dark filter/overlay to black out view
    //have 3 bools to indicate which location/position user is at?

    //upon scene start.
    //Position 1
    //  - fade in from black view into position one
    //  - wait an elapsed moment of time
    //  - fade view to black
    //Position 2
    //  - teleport player to position 2
    //  - fade in from black overlay
    //Position 3
    //  - from position 2, begin slowly translating to position 3
    //  - (note, maybe add another position or 2, so translation isn't along a linear line)
    //  - (would be good to zoom out form the easrth first, then bend way up to last position)
    //  - once at final position, wait a specific amount of time (will match this to the narration)
    //  - activate the single, final lever
    // now the ExperienceManager will wait for the user to activate the lever and play the final piece of narration
}
