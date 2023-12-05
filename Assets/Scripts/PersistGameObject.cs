using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistGameObject : MonoBehaviour
{
    void Awake()
    {
        gameObject.tag = "PersistentObject"; // tag for easy deletion in end
        DontDestroyOnLoad(gameObject);
    }
}
