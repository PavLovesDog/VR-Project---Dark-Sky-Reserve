using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLeverControl : MonoBehaviour
{
    public static PlanetLeverControl Instance { get; private set; }

    [Header("Lever Variables")]
    //rotation angles for on and off positions
    public Vector3 onRotation = new Vector3(-15f, -25f, -300f);
    public Vector3 offRotation = new Vector3(-15f, -25f, -250f);
    public float leverSpeed = 2f; // Speed at which the lever moves
    public bool isOn; // Current state of the lever;
    public bool canOperate = false;

    [Header("Objects")]
    public MeshRenderer cityLights;
    public ChangeMaterial lightBulb;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canOperate)
        {
            canOperate = false; // only switch ONE time

            if (other.gameObject.CompareTag("Hand"))
            {
                Debug.Log("Lever TOUCHED Hand");

                StartCoroutine(RotateLever(offRotation));

                AudioManager.Instance.PlaySFX(1, 0.35f, Random.Range(0.5f, 1.0f)); // power down
                lightBulb.TurnOffBloom();
                lightBulb.SetMaterial(1, lightBulb.lightOffMaterial);

                cityLights.enabled = false; // turn off lights
                isOn = false; // bool for ExperienceMaanager to listen for
            }
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
        //isOn = false; // bool for ExperienceMaanager to listen for
    }
}
