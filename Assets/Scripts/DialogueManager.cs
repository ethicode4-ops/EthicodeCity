using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public Image specialImage; // Image to display for specific dialogue
    public string dialogueSpeech;
    public SpeechManager speechmanager;

    public float textSpeed;
    public Character[] characters;  // Array of 4 characters (Alex, Elena, Sam, John)

    private int currentLineIndex = 0;
    private int currentCharacterIndex = 0;

    private bool isRunning = false;

    void Start()
    {
        specialImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // dialogueText.text = characters[currentCharacterIndex].dialogueLines[currentLineIndex];
            if (!isRunning) { transform.parent.gameObject.SetActive(false); }
        }
    }

    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
    //     {
    //         if (!isRunning) 
    //         { 
    //             transform.parent.gameObject.SetActive(false); 
    //         }
    //     }

    //     if (Input.GetKeyDown(KeyCode.B))
    //     {
    //         if (isRunning)
    //         {
    //             SkipDialogue();
    //         }
    //     }
    // }

    void SkipDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = characters[currentCharacterIndex].dialogueLines[currentLineIndex];

        currentLineIndex++;

        if (currentLineIndex < characters[currentCharacterIndex].dialogueLines.Length)
        {
            StartCoroutine(TypeLine(currentLineIndex, currentLineIndex + 1));
        }
        isRunning = false;
        transform.parent.gameObject.SetActive(false);
    }


    private void Init()
    {
        if (isRunning) return;
        transform.parent.gameObject.SetActive(true);
        currentCharacterIndex = 0;
        isRunning = true;
        nameText.text = characters[currentCharacterIndex].name;
        characterImage.sprite = characters[currentCharacterIndex].image;
        dialogueText.text = string.Empty;
    }

    public void StartDialogue()
    {
        Init();
        // SpeakLine(0, 5);
        StartCoroutine(TypeLine(0, 5));
        // StartCoroutine(GetLine(0, 5));
    }

    public IEnumerator FactoryDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(17, 18));
    }

    public IEnumerator BankBuildDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(7, 10));
    }

    public IEnumerator BankDestroyDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(10, 11));
    }

    public IEnumerator EarthQuakeDialogue(int i = 0)
    {
        Init();
        if (i == 0) yield return StartCoroutine(TypeLine(11, 13));
        if (i == 1) yield return StartCoroutine(TypeLine(13, 15));
    }

    public void HappinessDialogue()
    {
        Init();
        StartCoroutine(TypeLine(15, 16));
    }

    public IEnumerator DocumentationDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(16, 17));
    }

    public IEnumerator CongratulationsDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(18, 19));
    }

    public IEnumerator AIBuildingDialogue()
    {
        Init();
        yield return StartCoroutine(TypeLine(19, 20));
    }

    public IEnumerator BuildingPlacementDialogue(int i)
    {
        Init();
        if (i == 0) { yield return StartCoroutine(TypeLine(20, 21)); }
        else { yield return StartCoroutine(TypeLine(21, 22)); }
    }

    private IEnumerator TypeLine(int i, int j)
    {
        for (currentLineIndex = i; currentLineIndex < j; currentLineIndex++)
        {
            dialogueSpeech = characters[currentCharacterIndex].dialogueLines[currentLineIndex];
            Debug.Log(dialogueSpeech);
            dialogueText.text = string.Empty;
            yield return StartCoroutine(TypeLine());
            speechmanager.SpeakLine(dialogueSpeech);
            // yield return new WaitForSeconds(1.2f);
        }
        isRunning = false;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in characters[currentCharacterIndex].dialogueLines[currentLineIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

    }

    // void SpeakLine(int i, int j)
    // {
    //     for (currentLineIndex = i; currentLineIndex < j; currentLineIndex++)
    //     {
    //         dialogueSpeech = characters[currentCharacterIndex].dialogueLines[currentLineIndex];
    //         Debug.Log(dialogueSpeech);
    //         speechmanager.Speak(dialogueSpeech);
    //     }
    // }

    // IEnumerator GetLine()
    // {
    //     foreach (char c in characters[currentCharacterIndex].dialogueLines[currentLineIndex].ToCharArray())
    //     {
    //         dialogueSpeech += c;
    //         Debug.Log(dialogueSpeech);
    //         yield return new WaitForSeconds(textSpeed);
    //     }
    // }


    void CheckSpecialDialogue()
    {
        // Check if the current line is the special dialogue
        string currentDialogue = characters[currentCharacterIndex].dialogueLines[currentLineIndex];
        if (currentDialogue == "You are the new Mayor of this city. and must rebuild it after unforeseen circumstances caused its collapse.")
        {
            Debug.Log("Special image activated!");
            specialImage.gameObject.SetActive(true); // Show the special image
        }
        else
        {
            Debug.Log("Special image deactivated!");
            specialImage.gameObject.SetActive(false); // Hide the special image if it's not the special line
        }
    }
}
