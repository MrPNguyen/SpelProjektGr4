using UnityEngine;

public class Page : MonoBehaviour
{
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }
    
    public void TriggerAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Move"); //0B1D1C
    }
}
