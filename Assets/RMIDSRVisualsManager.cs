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
        public GameObject[] objectsToShow;
        public float startDelay;
        public float displayTime;
        public float delayBetweenObjects;
        public float timeBetweenSections; // Time to wait after a section before starting the next one
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
                SetSpriteRenderersEnabled(obj, false);
            }
        }
    }

    //Function to fire off sequence of events
    public IEnumerator VisualsSequence()
    {
        //roll through each section
        foreach (Section section in sections)
        {
            yield return new WaitForSeconds(section.startDelay); // use this to timne with narration beginnings

            // display each object in order
            foreach (GameObject obj in section.objectsToShow)
            {
                //Find its audio source
                AudioSource audio = obj.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.volume = 1.0f;
                    audio.pitch = Random.Range(0.4f, 0.9f);
                    audio.PlayOneShot(audio.clip); //play clip thats loaded into it one shot
                }

                SetSpriteRenderersEnabled(obj, true);
                yield return new WaitForSeconds(section.delayBetweenObjects);
            }

            yield return new WaitForSeconds(section.displayTime);

            //deactivate the images
            foreach (GameObject obj in section.objectsToShow)
            {
                SetSpriteRenderersEnabled(obj, false);
            }

            // Wait for the specified time before starting the next section
            yield return new WaitForSeconds(section.timeBetweenSections);
        }
    }
    // function to change state of sprite renderers
    private void SetSpriteRenderersEnabled(GameObject obj, bool enabled)
    {
        foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            //enable their sprite
            renderer.enabled = enabled;
        }
    }
}
