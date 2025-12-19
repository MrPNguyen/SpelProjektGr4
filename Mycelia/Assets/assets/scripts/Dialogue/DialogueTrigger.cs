using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
    
    [Header("Automatically Display Next DialogueLine")]
    public bool automaticAdvance;
    public float autoAdvanceDelay = 1.5f;
}
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public bool ableToWalkDuringDialogue;
    
    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, ableToWalkDuringDialogue);
    }
}
