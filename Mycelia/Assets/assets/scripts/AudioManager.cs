using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Start()
    {
        if (instance == null)
        {
            instance = this;

        }
    }

  

        public void SwapTrack(AudioClip clip)
    {
        
    }
    void Update()
    {
        
    }
}
