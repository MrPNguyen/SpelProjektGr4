using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int LevelIndex;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerManager playerM;
    public void StartGame()
    {
        SceneManager.LoadScene(LevelIndex);
    }

    public void EndGame()
    {
       Application.Quit();
    }

    public void PauseGame()
    {
        if(player == null) Debug.LogError("Player reference not set in LevelManager!");
        else
        {
            player.canMove = false;
            Debug.Log("Game paused, canMove = " + player.canMove);
        }
    }

    public void ResumeGame()
    {
        player.canMove = true;
    }

    public void UnstuckButton()
    {
        player.transform.position = playerM.originalPosition;
        player.canMove = true;
    }
}
