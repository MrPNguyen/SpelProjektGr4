using System.Collections;
using UnityEngine;

public class MusicFadeOut : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    public IEnumerator AudioFadeOutcoroutine(float duration)
    {
        float startVolume = audioSource.volume;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
