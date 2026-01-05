using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Journal : MonoBehaviour
{
    [SerializeField] private TMP_Text leftPageText;
    [SerializeField] private TMP_Text rightPageText;
    
    [SerializeField] private List<string> pages = new List<string>();
    public int currentPageIndex = 0;

    public void AddPaper(string content)
    {
        pages.Add(content);
        UpdateVisiblePage();
    }

    public void NextPage()
    {
        if (currentPageIndex + 2 < pages.Count)
        {
            currentPageIndex =+ 2;
            UpdateVisiblePage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex - 2 >= 0)
        {
            currentPageIndex -= 2;
            UpdateVisiblePage();
        }
    }

    public void UpdateVisiblePage()
    {
        leftPageText.text = currentPageIndex < pages.Count ? pages[currentPageIndex] : "";
        rightPageText.text = currentPageIndex + 1 < pages.Count ? pages[currentPageIndex + 1] : "";
    }
}
