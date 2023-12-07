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

    [SerializeField]
    private Canvas endCanvas; // Assign a UI CanvasGroup that will act as the fade overlay

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

    [Header("Bezier Curve Points")]
    [SerializeField]
    private GameObject curveStartPoint;
    [SerializeField]
    private GameObject curveEndPoint;

    [Header("Light Switch")]
    [SerializeField]
    private GameObject finalLever; // The final lever to activate
    [SerializeField]
    public bool speedUpNight = false;

    private int currentPositionIndex = 0; // Tracks the current position index

    [Header("RMIDSR Outline")]
    //public SpriteRenderer outline;
    [SerializeField] private GameObject outlineObject; // Assign this in the inspector
    [SerializeField] private float timeAtStart = 3f;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float pulsateScale = 0.1f; // Amount by which the scale will change
    [SerializeField] private float pulsateDuration = 1f; // Duration of one pulsate cycle

    void Start()
    {
        StartCoroutine(PositionSequence());
        finalLever.SetActive(false); // maybe access and just turn of the renderer
        lightPollution.SetActive(false);
        endCanvas.enabled = false;
        speedUpNight = false;

        //Coroutine to display the RMIDSR sprite
        StartCoroutine(ShowRMIDSROutline());
    }

    private IEnumerator ShowRMIDSROutline()
    {
        // Start the pulsating effect in a separate coroutine
        StartCoroutine(PulsateOutline(outlineObject, 20f, 0.16f, pulsateScale, pulsateDuration));

        yield return new WaitForSeconds(timeAtStart);

        // Fade in the outline object
        StartCoroutine(FadeSprite(outlineObject, true, fadeInDuration));

        // Wait for the display duration plus fade out duration before ending the coroutine
        yield return new WaitForSeconds(displayDuration + fadeOutDuration);

        // Fade out the outline sprite renderer
        StartCoroutine(FadeSprite(outlineObject, false, fadeOutDuration));
    }
    private IEnumerator PulsateOutline(GameObject obj, float totalDuration, float baseScale, float pulsateAmount, float pulsateDuration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < totalDuration)
        {
            float pulsateLerp = Mathf.PingPong(Time.time - startTime, pulsateDuration) / pulsateDuration;
            float scale = baseScale + pulsateAmount * pulsateLerp;
            obj.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        // Optionally restore the original scale after pulsating
        obj.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
    }

    private IEnumerator FadeSprite(GameObject obj, bool fadeIn, float duration)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, newAlpha);
            yield return null;
        }
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
        yield return StartCoroutine(MoveToPosition(2));

        // Begin moving to the final position
        yield return StartCoroutine(MoveToPosition(3));
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

            Vector3 controlPoint1 = curveStartPoint.transform.position;// Define the first control point
            Vector3 controlPoint2 = curveEndPoint.transform.position;

            while (elapsedTime < journeyLength)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / journeyLength;

                // Apply different easing functions based on the index
                if (index == 2) // Moving from position 2 to 3
                {
                    // Ease in quadratic function: t^2
                    t = t * t;

                    // lerp
                    user.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                }
                else if (index == 3) // Moving from position 3 to 4 (the final position)
                {
                    speedUpNight = true; // set bool to quicken the turning of nigth 
                    // Ease out quadratic function: -t*(t-2)
                    t = -t * (t - 2);

                    // Calculate the Bezier curve point
                    Vector3 m1 = Vector3.Lerp(startPosition, controlPoint1, t);
                    Vector3 m2 = Vector3.Lerp(controlPoint1, controlPoint2, t);
                    Vector3 m3 = Vector3.Lerp(controlPoint2, endPosition, t);
                    Vector3 b1 = Vector3.Lerp(m1, m2, t);
                    Vector3 b2 = Vector3.Lerp(m2, m3, t);
                    user.transform.position = Vector3.Lerp(b1, b2, t);
                }


                //user.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                user.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }

            // Confirm the final position and rotation
            user.transform.position = endPosition;
            user.transform.rotation = endRotation;

            if (index == positions.Length - 1) // This checks if the current index is the last one
            {

                // Wait at the final position
                yield return new WaitForSeconds(waitTimeAtFinal);

                // Activate the final lever here if automatic, or just allow the player to interact with it
                finalLever.SetActive(true);
                //StartCoroutine(OnFinalLeverActivated()); // TEMP
            }
        }
    }

    //// Public method to call when the final lever is activated, if required
    //public IEnumerator OnFinalLeverActivated()
    //{
    //    // Trigger the final piece of narration or any other required action
    //    yield return new WaitForSeconds(12.0f);
    //    endCanvas.enabled = true;
    //}

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
