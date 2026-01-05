using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(AudioClip clip)
    {
        if (clip == null || source == null) return;
        source.PlayOneShot(clip);
    }
}
