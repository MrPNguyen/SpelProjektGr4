using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDialogue : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private List<BoxCollider2D> bcs;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}
