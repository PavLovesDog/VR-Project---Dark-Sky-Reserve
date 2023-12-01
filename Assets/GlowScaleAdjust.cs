using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowScaleAdjust : MonoBehaviour
{
    //singleton instance
    public static GlowScaleAdjust Instance { get; private set; }

    public GameObject[] horizonGlowObjects;

    public float yGlowScale5 = 220;
    public float yGlowScale4 = 180;
    public float yGlowScale3 = 140;
    public float yGlowScale2 = 100;
    public float yGlowScale1 = 50;

    private int currentScaleLevel = 5;
    public float lerpDuration = 1.0f; // Duration for lerp transition

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

    void Start()
    {
        //LPM = GetComponent<LightPollutionManager>();
        //currentScaleLevel = transform.localScale.y;
    }

    // Call this method to start the lerp to the next scale
    public void StartLerpToNextScale()
    {
        if (currentScaleLevel > 1)
        {
            float startScale = GetScaleForLevel(currentScaleLevel);
            float endScale = GetScaleForLevel(currentScaleLevel - 1);
            StartCoroutine(LerpScale(startScale, endScale, lerpDuration));
            currentScaleLevel--;
        }
    }

    private IEnumerator LerpScale(float startScale, float endScale, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float scale = Mathf.Lerp(startScale, endScale, time / duration);
            // Apply the scale
            foreach(GameObject glow in horizonGlowObjects)
                glow.transform.localScale = new Vector3(glow.transform.localScale.x, scale, glow.transform.localScale.z);
            time += Time.deltaTime;
            yield return null;
        }
        // Ensure the final scale is set
        foreach (GameObject glow in horizonGlowObjects)
            glow.transform.localScale = new Vector3(glow.transform.localScale.x, endScale, glow.transform.localScale.z);
    }

    private float GetScaleForLevel(int level)
    {
        switch (level)
        {
            case 5: return yGlowScale5;
            case 4: return yGlowScale4;
            case 3: return yGlowScale3;
            case 2: return yGlowScale2;
            case 1: return yGlowScale1;
            default: return 1; // Default scale if level is not recognized
        }
    }
}
