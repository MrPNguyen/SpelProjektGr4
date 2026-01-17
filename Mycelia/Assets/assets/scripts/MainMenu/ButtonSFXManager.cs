using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonSFXManager : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ClickClip;
    [SerializeField] private AudioClip HoverClip;

    private bool isHovering = false;
    
    [Header("Highlights")]
    [SerializeField] private TMP_Text highlight;
    private Coroutine highlightCoroutine;
    [SerializeField] private float duration;

    private void Start()
    {
        if (highlight != null)
        {
            highlight.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            UIAudio.Instance.Play(HoverClip);

            if (highlightCoroutine != null  && highlight != null)
            {
                StopCoroutine(highlightCoroutine);
            }
            highlightCoroutine = StartCoroutine(HighlightFadeIncoroutine(duration));
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (highlightCoroutine != null && highlight != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(HighlightFadeOutcoroutine(duration));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClickClip != null)
        {
            UIAudio.Instance.Play(ClickClip);
        }
    }
    
    private void OnEnable()
    {
        isHovering = false;

        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);

        if (highlight != null)
        {
            highlight.alpha = 0f;
            highlight.gameObject.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(null);

        }
    }
    
    public IEnumerator HighlightFadeOutcoroutine(float duration)
    {
        if (highlight != null)
        {
            float startOpacity = highlight.alpha;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                highlight.alpha = Mathf.Lerp(startOpacity, 0f, time / duration);
                yield return null;
            }
        
            highlight.alpha = 0;
            highlight.gameObject.SetActive(false);
        }
    }
    public IEnumerator HighlightFadeIncoroutine(float duration)
    {
        if (highlight != null)
        {
            highlight.gameObject.SetActive(true);
            highlight.alpha = 0f;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                highlight.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }
        
            highlight.alpha = 1f;
        }
    }
}
