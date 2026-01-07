using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Journal : MonoBehaviour
{
    [SerializeField] private TMP_Text leftPageText;
    [SerializeField] private TMP_Text rightPageText;
    [SerializeField] private TMP_Text leftFlipText;  
    [SerializeField] private TMP_Text rightFlipText;

    [SerializeField] private List<string> pages;

    private void Start()
    {
        pages = new List<string>();
    }

    public void AddPaper(string content)
    {
        pages.Add(content);
    }

    public void UpdateVisiblePage(int rightPageIndex)
    {
        leftPageText.gameObject.SetActive(true);
        rightPageText.gameObject.SetActive(true);
        
        if (rightPageIndex == 0)
        {
            SetText(leftPageText, -1);
            SetText(rightPageText, 0);
            rightPageText.alignment = TextAlignmentOptions.Center;
            return;
        }
        else
        {
            rightPageText.alignment = TextAlignmentOptions.TopLeft;
        }
        
        int leftTextIndex = rightPageIndex - 1;
        int rightTextIndex = rightPageIndex;
        Debug.Log(
            $"LEFT parent: {leftPageText.transform.parent.name}, " +
            $"RIGHT parent: {rightPageText.transform.parent.name}"
        );
       SetText(leftPageText, leftTextIndex);
       SetText(rightPageText, rightTextIndex);
       
       SetText(leftFlipText, leftTextIndex);
       SetText(rightFlipText, rightTextIndex);
    }

    private void SetText(TMP_Text text, int index)
    {
        if (index >= 0 && index < pages.Count)
            text.text = pages[index];
        else
            text.text = "";
    }

    public void ActivateRightFlipText()
    {
        if (rightFlipText != null)
        {
            rightFlipText.gameObject.SetActive(true);
        }
    }
    
    public void ActivateLeftFlipText()
    {
        if (leftFlipText != null)
        {
            leftFlipText.gameObject.SetActive(true);
        }
    }
    
    public void DeactivateFlipText()
    {
        if (rightFlipText != null)
        {
            rightFlipText.gameObject.SetActive(false);
        }
        
        if (leftFlipText != null)
        {
            leftFlipText.gameObject.SetActive(false);
        }
    }
}
