using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologReturn : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
