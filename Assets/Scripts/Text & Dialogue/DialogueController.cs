using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueController : MonoBehaviour
{
    //Game variables for screen writitng
    public bool isIntroScene;
    public bool isEndScene;
    public bool inGameScene;
    public bool autoStart = true;
    public Canvas canvas; // The canvas Which the text will print to, SET IN INSPECTOR!
    public bool isDialogueComplete { get; private set; } = false;
    //public DialogueController DC;

    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.03f;
    private int index;

    public string[] lines;

    //Enable for connector rooms changing their state
    private void OnEnable()
    {
        if (inGameScene)
        {
            //// find Canvas
            //canvas = GameObject.FindGameObjectWithTag("PlayerCanvas").GetComponent<Canvas>();
            //// assign TMP text component
            //textComponent = GameObject.FindGameObjectWithTag("PlayerCanvasText").GetComponent<TextMeshProUGUI>();
            //// empty tex, if there is some
            //textComponent.text = string.Empty; // empty text

            //canvas.enabled = false; // set the canvas invisible
        }
    }

    public void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    public void SkipLine()
    {
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];

            // Check if it's the last line after immediately displaying the text
            if (index == lines.Length - 1 && inGameScene)
            {
                //StartCoroutine(DelayEndDialogue());
                isDialogueComplete = true;
            }
        }
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty; // clear previous text
        //canvas.enabled = enabled; // ensure the canvas is enabled while characters to be written
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c; // add character
            yield return new WaitForSeconds(textSpeed);
        }

        // Notify DialogueManager that dialogue has ended ??
        if(index == lines.Length - 1 && inGameScene)
            isDialogueComplete = true;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            textComponent.text = string.Empty;

        }
    }

    void LoadNextScene()
    {
        if (isIntroScene)
        {
            SceneManager.LoadScene(2);
        }
        else // is END scene
        {
            SceneManager.LoadScene(0);
        }
    }
}
