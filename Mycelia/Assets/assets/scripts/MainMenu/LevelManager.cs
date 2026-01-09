using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int LevelIndex;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerManager playerM;
    [SerializeField] private GameObject PauseMenu;
    private bool isPauseActive = false;
    public void StartGame()
    {
        Debug.Log(LevelIndex);
        SceneManager.LoadScene(LevelIndex, LoadSceneMode.Single);
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync("SlutLevel", LoadSceneMode.Single);

    }

    public void EndGame()
    {
       Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        player.canMove = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        player.canMove = true;
    }

    public void UnstuckButton()
    {
        player.transform.position = playerM.originalPosition;
        player.canMove = true;
        Time.timeScale = 1;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPauseActive = !isPauseActive;
            
            if (!isPauseActive)
            {
                PauseGame();
                if (PauseMenu != null) PauseMenu.SetActive(true);
            }
            else
            {
                ResumeGame();
                if (PauseMenu != null) PauseMenu.SetActive(false);
            }
        }
    }
}
