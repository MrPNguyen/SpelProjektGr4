using UnityEngine;

public class TypingSpeed : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;

    public void ChangeTypingSpeed(int index)
    {
        switch (index)
        {
            case 0:
                dialogueManager.typingSpeed = 0.2f; 
                break;
            case 1:
                dialogueManager.typingSpeed = 0.1f;
                break;
            case 2:
                dialogueManager.typingSpeed = 0.05f;
                break;
            default:
                dialogueManager.typingSpeed = 0.1f; 
                break;
        }
    }
}
