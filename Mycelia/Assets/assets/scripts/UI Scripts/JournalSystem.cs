using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    [SerializeField] private List<string> pages;
    [SerializeField] private TMP_Text pageLeft;
    [SerializeField] private TMP_Text pageRight;
    [SerializeField] private Animator pageFlipper;
    
    public void AddPage(string content)
    {
        pages.Add(content);
    }

    public void FlipPageRight()
    {
        Debug.Log("FlipPageRight");
        pageFlipper.SetTrigger("FlipRight");
    }
    
    public void FlipPageLeft()
    {
        Debug.Log("FlipPageLeft");
        pageFlipper.SetTrigger("FlipLeft");
    }
    
    private void SetText(TMP_Text text, int index)
    {
        if (index >= 0 && index < pages.Count)
            text.text = pages[index];
        else
            text.text = "";
    }
}
