using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    // Singleton instance
    public static DialogueManager Instance { get; private set; }

    public bool isTyping = false;
    private bool playerCanvasFound = false;
    public Canvas playerCanvas;
    public bool dontSearchForPlayerCanvas;

    // Active DialogueController reference
    private DialogueController activeDialogueController = null;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //if(!dontSearchForPlayerCanvas)
        //{
        //    playerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
        //    playerCanvasFound = true;
        //}
    }

    void Update()
    {
        //Need a way to chekc if object is in gamescene, 
        //if it is, check if player cnavs is null
        // if is set the player canvas
        if (SceneManager.GetActiveScene().name == "Game_Scene" && !playerCanvasFound)
        {
            if(playerCanvas == null)
            {
                playerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
                playerCanvasFound = true;
            }
        }

        if (playerCanvas != null)
        {
            if (isTyping)
                playerCanvas.enabled = true;
            else
                playerCanvas.enabled = false;
        }

        //// Handle skipping logic
        //if (activeDialogueController != null && Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if(activeDialogueController.isDialogueComplete)
        //    {
        //        DialogueManager.Instance.EndDialogue();
        //    }
        //    else
        //    {
        //        activeDialogueController.SkipLine();
        //    }
        //}
    }

    public void StartDialogue(DialogueController dialogueController)
    {
        // If another dialogue is active, return
        if (activeDialogueController != null) return;

        activeDialogueController = dialogueController;
        isTyping = true;
        dialogueController.StartDialogue();
    }

    public void EndDialogue(/*DialogueController dialogueController*/)
    {
        isTyping = false;
        //playerCanvas.enabled = false;
        activeDialogueController = null;
    }
}
