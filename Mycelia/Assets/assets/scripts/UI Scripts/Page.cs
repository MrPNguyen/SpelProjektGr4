using UnityEngine;

public class Page : MonoBehaviour
{
    [SerializeField] private int originalSortOrder;
    [SerializeField] private int newSortOrder;
    private Canvas page;
    private Animator animator;
    void Awake()
    {
        page = GetComponent<Canvas>();
        animator = GetComponent<Animator>();
        page.overrideSorting = true;
    }

    public void BringToFront()
    {
       page.sortingOrder = newSortOrder;
    }

    public void ReturnBack()
    {
        page.sortingOrder = originalSortOrder;
    }
    
    public void OnBookOpened()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}
