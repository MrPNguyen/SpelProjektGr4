using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public void Fullscreen(bool is_fullscreen)
    {
        Screen.fullScreen = is_fullscreen;
    }
}
