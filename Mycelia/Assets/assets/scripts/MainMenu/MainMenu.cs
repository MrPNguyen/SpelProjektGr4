using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject transitionWall;
    [SerializeField] private float duration;
    [SerializeField] private MusicFadeOut musicFadeOut;
    [SerializeField] private int levelIndex;

    void Start()
    {
        musicFadeOut = GetComponent<MusicFadeOut>();
    }
    public void PlayGame()
    {
        StartCoroutine(StartGameCoroutine(duration));
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine(duration));
    }
    
    IEnumerator StartGameCoroutine(float duration)
    {
        StartCoroutine(musicFadeOut.AudioFadeOutcoroutine(duration));

        if (transitionWall != null && animator != null)
        {
            transitionWall.SetActive(true);
            animator.SetTrigger("play");
        }
        
        yield return new WaitForSecondsRealtime(2f);
        
        Debug.Log("Transitioning");
        SceneManager.LoadSceneAsync(levelIndex);
    }
    IEnumerator QuitGameCoroutine(float duration)
    {
        StartCoroutine(musicFadeOut.AudioFadeOutcoroutine(duration));

        if (transitionWall != null && animator != null)
        {
            transitionWall.SetActive(true);
            animator.SetTrigger("play");
        }
        
        yield return new WaitForSecondsRealtime(2f);
        
        Debug.Log("Transitioning");
        Application.Quit();
    }

    
}

