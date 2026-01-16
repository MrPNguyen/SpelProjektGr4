using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;

//AudioClip from: "Talking Synthesizer" by tcarisland via OpenArtGame.Org, CC-BY 40 license
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<DialogueLine> lines;


    public float typingSpeed;
    [SerializeField] private float autoAdvanceDelay = 1.5f;

    public Animator animator;

    [SerializeField] public PlayerMovement playerMovement;

    [Header("Audio")]

    [SerializeField] private AudioClip dialogueTypingSoundClip;
    [SerializeField] private int frequencyLevel = 4;
    [Range(-5, 5)]
    [SerializeField] private bool stopAudioSource;

    private AudioSource audioSource;
    public bool DialogueEnd = false;

    private bool currentAutoAdvance;
    private float currentAutoAdvanceDelay;
    private bool movementWasLocked;
    private bool isTyping;
    private string currentLineText;

    private void Start()
    {
        audioSource.volume = 0.2f;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Debug.Log("DialogueManager Instance created. Instance ID: " + this.GetInstanceID());
        }
        else
        {
            //Debug.LogWarning("Another DialogueManager instance already exists. Destroying this one.");
            Destroy(gameObject);
        }

        lines = new Queue<DialogueLine>();

        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public void StartDialogue(Dialogue dialogue, bool ableToWalkDuringDialogue)
    {
        //Debug.Log("Starting new dialogue. Resetting skipExitAnimation to false.");
        animator.SetBool("started", true);
        DialogueEnd = false;

        currentAutoAdvance = dialogue.automaticAdvance;
        currentAutoAdvanceDelay = dialogue.autoAdvanceDelay;

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        movementWasLocked = playerMovement != null && !ableToWalkDuringDialogue;
        if (movementWasLocked)
        {
            playerMovement.canMove = false;
            playerMovement.velocity = Vector2.zero;
            playerMovement.rb.linearVelocity = Vector2.zero;
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            audioSource.Stop();
            isTyping = false;
            dialogueText.text = currentLineText;
            return;
        }

        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();
        currentLineText = currentLine.line;


        characterIcon.sprite = currentLine.character.icon;
        nameText.text = currentLine.character.name;

        StartCoroutine(TypeSentence(currentLine));
    }

    public void OnAdvanceDialogue(InputAction.CallbackContext context)
    {
        if (context.performed && !DialogueEnd)
        {
            DisplayNextDialogueLine();
        }
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueText.text = "";
        int charCount = 0;
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            if (isTyping == false)
                break;

            PlayDialogueSound(charCount);
            dialogueText.text += letter;
            charCount++;
            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueText.text = currentLineText;
        isTyping = false;
        audioSource.Stop();

        if (currentAutoAdvance)
        {
            yield return new WaitForSeconds(currentAutoAdvanceDelay);
            DisplayNextDialogueLine();
        }
    }
    private void PlayDialogueSound(int currentDisplayedCharacterCount)
    {
        //Debug.Log("PlayDialogueSound called, character count:" + currentDisplayedCharacterCount + "");
        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }

            if (audioSource != null)
            {
                audioSource.PlayOneShot(dialogueTypingSoundClip);
            }
        }
    }
    public void EndDialogue()
    {
        StopAllCoroutines();
        audioSource.Stop();
        isTyping = false;
        animator.SetBool("started", false);
        DialogueEnd = true;
        
        if (movementWasLocked)
        {
            playerMovement.horizontalMovement = 0f;
            playerMovement.velocity = Vector2.zero;
            playerMovement.rb.linearVelocity = Vector2.zero;
            playerMovement.canMove = true;
        }
    }
}
