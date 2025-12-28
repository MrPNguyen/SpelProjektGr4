using System;
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
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            audioSource.PlayOneShot(HoverClip);
            Debug.Log("Mouse Hovering");
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSource.PlayOneShot(ClickClip);
    }
}
