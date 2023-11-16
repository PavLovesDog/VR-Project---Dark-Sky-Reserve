using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl : MonoBehaviour
{
    //rotation angles for on and off positions
    public Vector3 onRotation = new Vector3(-48.5f, 0f, 0f);
    public Vector3 offRotation = new Vector3(-110f, 0f, 0f);

    public float leverSpeed = 2f; // Speed at which the lever moves
    public bool isOn; // Current state of the lever;
    private float lastActivationTime = 0f;
    public float activationCooldown = 0.5f; // Minimum time between activations
    private Coroutine rotationCoroutine; // Reference to current running coroutine

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hand") && Time.time - lastActivationTime >= activationCooldown)
        {
            Debug.Log("Lever TOUCHED Hand");

            // If a rotation coroutine is running, stop it
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
                AudioManager.Instance.StopSFX();
            }

            // Determine the new target rotation based on the current state
            Vector3 targetRotation = isOn ? offRotation : onRotation;

            // Start the new rotation coroutine and store its reference
            rotationCoroutine = StartCoroutine(RotateLever(targetRotation));

            //play audio dependent on state
            if(isOn)
                AudioManager.Instance.PlaySFX(1, 0.35f, Random.Range(0.5f, 1.0f)); // power down
            else // is off
                AudioManager.Instance.PlaySFX(0, 0.65f, Random.Range(0.5f, 1.0f)); // power up sound
            
            // Toggle the state of the lever
            isOn = !isOn;

            // Update last activation time
            lastActivationTime = Time.time;
        }
        else
        {
            Debug.Log("Lever is cooling down.");
        }
    }

    private IEnumerator RotateLever(Vector3 targetEulerAngles)
    {
        // Calculate the target rotation as a Quaternion
        Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

        // Rotate until the rotation is approximately equal to the target rotation
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            // Interpolate rotation over time
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, leverSpeed * Time.deltaTime);
            yield return null; // Wait until next frame
        }

        // Snap rotation to target to avoid overshooting
        transform.rotation = targetRotation;
    }
}
