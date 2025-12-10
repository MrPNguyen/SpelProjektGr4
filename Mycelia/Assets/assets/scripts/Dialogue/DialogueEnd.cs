using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueEnd : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Animator transitionAnimator;
    public float transitionTime = 1f;
    //SceneTransitionManager scenetransition;
    
    void Update()
    {
        if (dialogueManager != null && dialogueManager.DialogueEnd)
        {
            Debug.Log("Dialogue ended. Starting transition...");
            StartCoroutine(LoadGameWithTransition());
        }
    }
    /*private void Awake()
    {
        scenetransition = FindObjectOfType<SceneTransitionManager>();
    }*/

    IEnumerator LoadGameWithTransition()
    {
        Debug.Log("FadeOut trigger set.");

        if (dialogueManager != null)
        {
            Debug.Log("Calling SkipExitAnimation...");
            dialogueManager.SkipExitAnimation();
            Debug.Log("skipExitAnimation after SkipExitAnimation: " + dialogueManager.skipExitAnimation);
        }
        else
        {
            Debug.LogError("DialogueManager is null!");
        }

        transitionAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("Loading next scene...");
        SceneManager.LoadScene("Main_Game");
    }
    public void StartTransitionEarly()
    {
        Debug.Log("Starting transition early.");

        // Skip the dialogue box exit animation
        if (dialogueManager != null)
        {
            Debug.Log("Calling SkipExitAnimation...");
            dialogueManager.SkipExitAnimation();
            Debug.Log("skipExitAnimation after SkipExitAnimation: " + dialogueManager.skipExitAnimation);
        }
        else
        {
            Debug.LogError("DialogueManager is null!");
        }
    }
}
