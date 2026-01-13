using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scrolls : MonoBehaviour
{
    private bool inRange = false;
    [SerializeField] private GameObject interactText;
    [SerializeField] private TMP_Text scrollText;
    [SerializeField] private GameObject scrollCanvas;
    [SerializeField] private JournalSystem journal;
    [SerializeField] private GameObject Alert;
    private bool read = false;

    private void Start()
    {
        interactText.SetActive(false);
    }

    void Update()
    {
        Debug.Log(inRange);
        if (inRange)
        {
            if (interactText != null)
            {
                interactText.SetActive(true);
            }
        }
        else
        {
            if (interactText != null)
            {
                interactText.SetActive(false);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player") inRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
       if(other.tag == "Player") inRange = false;
    }

    public void PickupScroll(InputAction.CallbackContext context)
    {
        if(!inRange) return;
        
        if (context.performed && !scrollCanvas.activeSelf)
        {
            scrollCanvas.SetActive(true);
            journal.AddPage(scrollText.text);
            Alert.SetActive(true);
            Time.timeScale = 0;
        }
    }
    
    public void DestroyScroll()
    {
        if (read) return;
        read = true;
        
        scrollCanvas.SetActive(false);
        interactText.SetActive(false);
        Time.timeScale = 1;
        Destroy(gameObject);
    }
    
    private IEnumerator HideAlertAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Alert.SetActive(false);
    }
}
