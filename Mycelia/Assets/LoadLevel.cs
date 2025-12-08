using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void LoadGameLevel()
    {
        SceneManager.LoadSceneAsync("Level1");
    }
}