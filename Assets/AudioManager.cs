using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource narrationSource; // Where the main narration will come form

    [SerializeField]
    private AudioSource sfxSource; // Where environment or interaction specific audio will come form

    [SerializeField]
    private List<AudioClip> narrationClips; // all the narration specific clips (maybe order this as necessary)

    [SerializeField]
    private List<AudioClip> sfxClips; // All other audio clips

    private Queue<AudioClip> narrationQueue = new Queue<AudioClip>(); // queue to hold what needs to be played

    private void Awake()
    {
        // makes AudioManager a singleton, so only one instance is active
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the AudioManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // used to play a specific narration clip.
    public void PlayNarration(int clipIndex)
    {
        //catch for bad clip index
        if (clipIndex < 0 || clipIndex >= narrationClips.Count)
            return;

        // add it to the queue
        narrationQueue.Enqueue(narrationClips[clipIndex]);
        // if nothing playing, play it now
        if (!narrationSource.isPlaying)
        {
            StartCoroutine(PlayNarrationQueue());
        }
    }

    //a coroutine that manages the queue of narrations.
    private IEnumerator PlayNarrationQueue()
    {
        while (narrationQueue.Count > 0)
        {
            narrationSource.clip = narrationQueue.Dequeue();
            narrationSource.Play();
            while (narrationSource.isPlaying)
            {
                yield return null;
            }
        }
    }

    public void PauseNarration()
    {
        narrationSource.Pause();
    }

    public void ResumeNarration()
    {
        narrationSource.UnPause();
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void StopNarration()
    {
        narrationSource.Stop();
        narrationQueue.Clear();
    }

    public void PlaySFX(int clipIndex, float volume, float pitch)
    {
        if (clipIndex < 0 || clipIndex >= sfxClips.Count)
            return;

        sfxSource.pitch = pitch;

        sfxSource.PlayOneShot(sfxClips[clipIndex], volume);
    }
}
