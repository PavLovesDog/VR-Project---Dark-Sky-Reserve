using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistGameObject : MonoBehaviour
{
    private static HashSet<string> createdObjects = new HashSet<string>();
    private static bool created = false;
    public string typeId;

    public bool BG_music;

    void Awake()
    {
        typeId = gameObject.name; // or some other unique identifier for the type of object. juyst use name for now

        if (BG_music)
        {
            if (!created)
            {
                DontDestroyOnLoad(gameObject); // Keep the ExperienceManager across scenes
                created = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (!createdObjects.Contains(typeId))
            {
                gameObject.tag = "PersistentObject"; // tag for easy deletion in end
                DontDestroyOnLoad(gameObject);
                createdObjects.Add(typeId); // add it the hash set so its not replicated
                LogCreatedObjects(); // display whats in hashset
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    // Public static method to clear the HashSet
    public static void ClearCreatedObjects()
    {
        createdObjects.Clear();
    }

    // Method to log the contents of the HashSet
    public void LogCreatedObjects()
    {
        Debug.Log(typeId + ": Objects in HashSet:");
        int counter = 1;
        foreach (string obj in createdObjects)
        {
            Debug.Log(counter + ": " + obj);
        }
    }
}
