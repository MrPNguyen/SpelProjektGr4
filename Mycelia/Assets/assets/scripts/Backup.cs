using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Backup : MonoBehaviour
{
    public void SlutLevel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadSceneAsync("SlutLevel");
        }
    }
}
