using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    public AudioSource narrationSource; // Where the main narration will come form

    [SerializeField]
    private AudioSource sfxSource; // Where environment or interaction specific audio will come form

    [SerializeField]
    public List<AudioClip> narrationClips; // all the narration specific clips (maybe order this as necessary)

    [SerializeField]
    private List<AudioClip> sfxClips; // All other audio clips

    private Queue<AudioClip> narrationQueue = new Queue<AudioClip>(); // queue to hold what needs to be played

    private void Awake()
    {
        // makes AudioManager a singleton, so only one instance is active
        if (Instance == null)
        {
            Instance = this;
            gameObject.tag = "PersistentObject"; // tag for easy deletion in end
            DontDestroyOnLoad(gameObject); // Keep the AudioManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        //Find audio components
        if(narrationClips == null)
            narrationSource = GameObject.FindGameObjectWithTag("NarrationSource").GetComponent<AudioSource>();
        if(sfxClips == null)
            sfxSource = GameObject.FindGameObjectWithTag("SFXSource").GetComponent<AudioSource>();
    }

    // Used to play a specific narration clip.
    public void PlayNarration(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= narrationClips.Count)
        {
            Debug.LogError("Clip index out of range");
            return;
        }

        narrationQueue.Enqueue(narrationClips[clipIndex]);

        // If nothing is playing, play it now
        if (narrationSource != null && !narrationSource.isPlaying)
        {
            StartCoroutine(PlayNarrationQueue());
        }
    }

    // A coroutine that manages the queue of narrations.
    private IEnumerator PlayNarrationQueue()
    {
        while (narrationQueue.Count > 0)
        {
            narrationSource.clip = narrationQueue.Dequeue();
            narrationSource.Play();

            // Wait until the clip is finished playing
            yield return new WaitWhile(() => narrationSource.isPlaying);

            //// Optionally, increment the index here if this is the only place that affects it
            //ExperienceManager.Instance.currentNarrationIndex++;
        }
    }

    public void PauseNarration()
    {
        narrationSource.Pause();
    }

    //private void skipClip()
    //{
    //    narrationSource.Stop();
    //
    //}

    public void ResumeNarration()
    {
        narrationSource.UnPause();
    }

    public void StopSFX()
    {
        if(sfxSource != null)
            sfxSource.Stop();
    }

    public void StopNarration()
    {
        if (narrationSource != null)
        {
            narrationSource.Stop();
            narrationQueue.Clear();
        }
    }

    public bool IsNarrationPlaying()
    {
        return narrationSource != null && narrationSource.isPlaying;
    }

    public void PlaySFX(int clipIndex, float volume, float pitch)
    {
        if (clipIndex < 0 || clipIndex >= sfxClips.Count)
            return;

        sfxSource.pitch = pitch;

        sfxSource.PlayOneShot(sfxClips[clipIndex], volume);
    }
}
