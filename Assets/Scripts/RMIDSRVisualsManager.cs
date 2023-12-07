using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMIDSRVisualsManager : MonoBehaviour
{
    public static RMIDSRVisualsManager Instance { get; private set; }

    [Header("Ligth Drawings To Display")]
    public Section[] sections;

    [Header("Experience Manager bool")]
    public bool beginSequence = false;

    [System.Serializable]
    public class Section
    {
        public GameObject[] objectsToShow; // list of sprite gameobjects to manipulate in scene
        public float startDelay;
        public float displayTime;
        public float delayBetweenObjects;
        //public float timeBetweenSections; // Time to wait after a section before starting the next one
        // timeBetweenSections may be redundent now I will be show sections individually.
    }

    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Disable all SpriteRenderers on start
        DisableAllSpriteRenderers();
        //StartCoroutine(VisualsSequence());
    }

    private void DisableAllSpriteRenderers()
    {
        foreach (var section in sections)
        {
            foreach (var obj in section.objectsToShow)
            {
                StartCoroutine(FadeSpriteRenderer(obj, false, 0.01f));
            }
        }
    }

    // Function to fire off sequence of events for a specific section
    public IEnumerator VisualsSequence(Section section)
    {
        yield return new WaitForSeconds(section.startDelay);

        // Display each object in order
        foreach (GameObject obj in section.objectsToShow)
        {
            // Find its audio source
            AudioSource audio = obj.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.volume = 0.65f;
                audio.pitch = Random.Range(0.75f, 1f);
                audio.PlayOneShot(audio.clip);
            }

            StartCoroutine(FadeSpriteRenderer(obj, true, 1.0f)); // Fade images in over 1 second
            yield return new WaitForSeconds(section.delayBetweenObjects);
        }

        yield return new WaitForSeconds(section.displayTime);

        // Deactivate the images
        foreach (GameObject obj in section.objectsToShow)
        {
            StartCoroutine(FadeSpriteRenderer(obj, false, 1.0f)); // Fade images out over 1 second
        }
    }

    // Function to change the alpha of sprite renderers within gameObject simultaneously
    private IEnumerator FadeSpriteRenderer(GameObject obj, bool fadeIn, float duration)
    {
        // Create a list to hold all fade coroutines for each SpriteRenderer
        List<IEnumerator> fadeCoroutines = new List<IEnumerator>();

        // Loop through each SpriteRenderer in the GameObject and its children
        foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            IEnumerator fadeCoroutine = FadeSingleSprite(renderer, fadeIn, duration); // Create a coroutine for fading particular SpriteRenderer
            fadeCoroutines.Add(fadeCoroutine);

            // Start coroutines to fade topgether
            StartCoroutine(fadeCoroutine);
        }

        // Wait for all SpriteRenderer fade coroutines to complete
        foreach (IEnumerator coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }
    }

    // Coroutine to fade a single sprite renderer
    private IEnumerator FadeSingleSprite(SpriteRenderer renderer, bool fadeIn, float duration)
    {
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
}
