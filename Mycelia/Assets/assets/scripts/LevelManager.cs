using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int LevelIndex;
    
    public void StartGame()
    {
        SceneManager.LoadScene(LevelIndex);
    }

    public void EndGame()
    {
        Environment.Exit(0); //Alternativt Application.Quit();
    }
}
