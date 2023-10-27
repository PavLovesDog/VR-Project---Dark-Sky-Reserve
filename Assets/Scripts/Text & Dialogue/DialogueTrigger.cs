using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Canvas canvas;
    public DialogueController dialogue;
    public bool collidedWithTrigger = false;

    void Start()
    {
        dialogue = GetComponent<DialogueController>();
        canvas = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !collidedWithTrigger)
        {
            collidedWithTrigger = true; // only trip once
            //canvas.enabled = true;
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
