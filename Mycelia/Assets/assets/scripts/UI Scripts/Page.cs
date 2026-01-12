using UnityEngine;

public class Page : MonoBehaviour
{
    [SerializeField] private int originalSortOrder;
    [SerializeField] private int newSortOrder;
    private Canvas page;
    void Start()
    {
        page = GetComponent<Canvas>();
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
    
}
