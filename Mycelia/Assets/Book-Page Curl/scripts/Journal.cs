using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    [Header("Animated Pages")]
    [SerializeField] private TMP_Text leftFlipText;
    [SerializeField] private TMP_Text rightFlipText;
    
    [Header("Static Pages")]
    [SerializeField] private TMP_Text leftStaticText;  
    [SerializeField] private TMP_Text rightStaticText;

    [SerializeField] private List<string> pages;

    private void Awake()
    {
        if (pages == null)
        {
            pages = new List<string>();
        }
        leftFlipText.gameObject.SetActive(false);
        rightFlipText.gameObject.SetActive(false);
    }
    public void AddPaper(string content)
    {
        pages.Add(content);
    }

    public void UpdateVisiblePage(int rightPageIndex)
    {
        if (rightPageIndex == 0)
        {
            SetText(leftFlipText, -1);
            SetText(rightFlipText, 0);
            
            SetText(leftStaticText, -1);
            SetText(rightStaticText, 0);
            
            rightFlipText.alignment = TextAlignmentOptions.TopLeft;
            rightStaticText.alignment = TextAlignmentOptions.Center;
            return;
        }

        if (rightPageIndex == pages.Count)
        {
            SetText(leftFlipText, -1);
            SetText(rightFlipText, -1);
            SetText(leftStaticText, -1);
            SetText(rightStaticText, -1);
            return;
        }
        
        rightFlipText.alignment = TextAlignmentOptions.TopLeft;
        rightStaticText.alignment = TextAlignmentOptions.TopLeft;
        
        int leftTextIndex = rightPageIndex - 1;
        int rightTextIndex = rightPageIndex;
        Debug.Log(
            $"LEFT parent: {leftFlipText.transform.parent.name}, " +
            $"RIGHT parent: {rightFlipText.transform.parent.name}"
        );
        SetText(leftStaticText, leftTextIndex);
        SetText(rightStaticText, rightTextIndex);

        // These are for the pages underneath
        /*SetText(leftFlipText, leftTextIndex);
        SetText(rightFlipText, rightTextIndex);*/
        SetText(leftFlipText, leftTextIndex + 1);
        SetText(rightFlipText, rightTextIndex + 2);
        
    }

    private void SetText(TMP_Text text, int index)
    {
        if (index >= 0 && index < pages.Count)
            text.text = pages[index];
        else
            text.text = "";
    }

    public void ActivateRightFlipText(int rightPageIndex)
    {
        if (rightPageIndex == 0)
        {
            if (leftStaticText != null) leftStaticText.gameObject.SetActive(false);
            if (rightStaticText != null) rightStaticText.gameObject.SetActive(false);
        }
        
        int leftIndex = Mathf.Clamp(rightPageIndex + 1, 0, pages.Count - 1);
        int rightIndex = Mathf.Clamp(rightPageIndex + 2, 0, pages.Count - 1);
        
        SetText(leftFlipText, leftIndex);
        SetText(rightFlipText, rightIndex);
        
        if (leftFlipText != null) leftFlipText.gameObject.SetActive(true);
        if (rightFlipText != null) rightFlipText.gameObject.SetActive(true);
    }
    
    public void ActivateLeftFlipText(int rightPageIndex)
    {
        if (rightPageIndex == 0)
        {
            if (leftStaticText != null) leftStaticText.gameObject.SetActive(false);
            if (rightStaticText != null) rightStaticText.gameObject.SetActive(false);
        }

        int leftIndex = Mathf.Clamp(rightPageIndex - 2, 0, pages.Count - 1);
        int rightIndex = Mathf.Clamp(rightPageIndex - 1, 0 , pages.Count - 1);
        
        SetText(leftFlipText, leftIndex);
        SetText(rightFlipText, rightIndex);
        
        if (leftFlipText != null) leftFlipText.gameObject.SetActive(true);
        if (rightFlipText != null) rightFlipText.gameObject.SetActive(true);

        if (leftFlipText != null)
        {
            leftFlipText.text = "";
            leftStaticText.text = "";
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

        if (leftStaticText != null)
        {
            leftStaticText.gameObject.SetActive(true);
        }

        if (rightStaticText != null)
        {
            rightStaticText.gameObject.SetActive(true);
        }
    }
}
