using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject transitionWall;
    [SerializeField] private AudioSource audioSource;
    public void PlayGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator StartGameCoroutine()
    {
        audioSource.Stop();
        transitionWall.SetActive(true);
        animator.SetTrigger("play");
        
        yield return new WaitForSecondsRealtime(2f);
        
        Debug.Log("Transitioning");
        SceneManager.LoadSceneAsync(1);
    }
}

