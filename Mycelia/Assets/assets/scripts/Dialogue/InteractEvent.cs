using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractEvent : MonoBehaviour
{
    public GameObject interactText;
    public bool isInRange;
    public UnityEvent interactAction;

    public void Interact(InputAction.CallbackContext context)
    {
        if (isInRange)
        {
            if (context.performed)
            {
                interactAction.Invoke();
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            interactText.SetActive(false);
        }
    }
}
