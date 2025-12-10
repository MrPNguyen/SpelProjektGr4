using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<DialogueLine> lines;


    public float typingSpeed = 2f;

    public Animator animator;

    [SerializeField] public PlayerMovement playerMovement;

    [Header("Audio")]

    [SerializeField] private AudioClip dialogueTypingSoundClip;
    [SerializeField] private int frequencyLevel = 4;
    [Range(-5,5)]
    [SerializeField] private bool stopAudioSource;

    private AudioSource audioSource;
    public bool DialogueEnd = false;

    public bool skipExitAnimation = false;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("DialogueManager Instance created. Instance ID: " + this.GetInstanceID());
        }
        else
        {
            Debug.LogWarning("Another DialogueManager instance already exists. Destroying this one.");
            Destroy(gameObject);
        }

        lines = new Queue<DialogueLine>();

        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting new dialogue. Resetting skipExitAnimation to false.");
        animator.SetBool("started", true);

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
        if (playerMovement != null)
        {
            playerMovement.canMove = false;

        }
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();


        characterIcon.sprite = currentLine.character.icon;
        nameText.text = currentLine.character.name; 

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
        
        if (lines.Count == 0)
        {
            FindObjectOfType<DialogueEnd>().StartTransitionEarly();
        }
        
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueText.text = "";
        int charCount = 0;
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            PlayDialogueSound(charCount);
            dialogueText.text += letter;
            charCount++;
            yield return new WaitForSeconds(typingSpeed);
        }

        audioSource.Stop();
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
            //Debug.Log("Playing sound");
            audioSource.PlayOneShot(dialogueTypingSoundClip);
        }
    }
    public void EndDialogue()
    {
        Debug.Log("EndDialogue called. skipExitAnimation: " + skipExitAnimation);
        if (skipExitAnimation)
        {
            Debug.Log("Skipping exit animation.");
            animator.enabled = false;
        }
        else
        {
            Debug.Log("Playing exit animation.");
            animator.SetBool("started", false);
        }
        
        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }
        DialogueEnd = true;
    }

    public void SkipExitAnimation()
    {
        Debug.Log("SkipExitAnimation called. Setting skipExitAnimation to true.");
        Debug.Log("DialogueManager Instance ID: " + this.GetInstanceID());
        skipExitAnimation = true;
    }
}
