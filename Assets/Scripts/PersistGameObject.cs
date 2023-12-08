using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistGameObject : MonoBehaviour
{
    private static HashSet<string> createdObjects = new HashSet<string>();

    void Awake()
    {
        string typeId = gameObject.name; // or some other unique identifier for the type of object. juyst use name for now

        if (!createdObjects.Contains(typeId))
        {
            gameObject.tag = "PersistentObject"; // tag for easy deletion in end
            DontDestroyOnLoad(gameObject);
            createdObjects.Add(typeId); // add it the hash set so its not replicated
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
