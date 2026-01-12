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

    [TextArea(3, 10)]
    [SerializeField] private List<string> pages;

    private void Awake()
    {
        if (pages == null)
        {
            pages = new List<string>();
        }
        //leftFlipText.gameObject.SetActive(false);
        //rightFlipText.gameObject.SetActive(false);
    }

    void Update()
    {
        Debug.Log($"rightStaticText: {rightStaticText.text}");
        Debug.Log($"rightFlipText: {rightFlipText.text}");
        Debug.Log($"leftStaticText: {leftStaticText.text}");
        Debug.Log($"leftFlipText: {leftFlipText.text}");
    }
    public void AddPaper(string content)
    {
        pages.Add(content);
    }

    public void UpdateVisiblePage(int rightPageIndex)
    {
        if (rightPageIndex == 0)
        {
            SetText(leftStaticText, -1);
            SetText(rightStaticText, 0);
            
            rightStaticText.alignment = TextAlignmentOptions.Center;
            rightStaticText.fontSize = 60;
            return;
        }

        if (rightPageIndex == pages.Count)
        {
            SetText(leftStaticText, -1);
            SetText(rightStaticText, -1);
            return;
        }
        
        rightFlipText.alignment = TextAlignmentOptions.TopLeft;
        rightStaticText.alignment = TextAlignmentOptions.TopLeft;
        
        rightStaticText.fontSize = 45;
        leftStaticText.fontSize = 45;
        rightFlipText.fontSize = 45;
        leftFlipText.fontSize = 45;

        
        int leftTextIndex = rightPageIndex - 1;
        int rightTextIndex = rightPageIndex;
        Debug.Log(
            $"LEFT parent: {leftFlipText.transform.parent.name}, " +
            $"RIGHT parent: {rightFlipText.transform.parent.name}"
        );
        SetText(leftStaticText, leftTextIndex);
        SetText(rightStaticText, rightTextIndex);
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
        /*if (rightPageIndex == 0)
        {
            if (leftStaticText != null) leftStaticText.gameObject.SetActive(false);
            if (rightStaticText != null) rightStaticText.gameObject.SetActive(false);
        }
        
        int leftIndex = Mathf.Clamp(rightPageIndex + 1, 0, pages.Count - 1);
        int rightIndex = Mathf.Clamp(rightPageIndex + 2, 0, pages.Count - 1);
        
        SetText(leftFlipText, leftIndex);
        SetText(rightFlipText, rightIndex);
        
        if (leftFlipText != null) leftFlipText.gameObject.SetActive(true);
        if (rightFlipText != null) rightFlipText.gameObject.SetActive(true);*/
        
        /*if (rightFlipText != null)
        {
            rightFlipText.text = "";
            rightStaticText.text = "";
        }*/
        leftFlipText.gameObject.SetActive(true);
        rightFlipText.gameObject.SetActive(true);
        
        int leftIndex = rightPageIndex + 1;
        int rightIndex = rightPageIndex + 2;
        
        SetText(leftFlipText, leftIndex);
        SetText(rightFlipText, rightIndex);
        
        //leftStaticText.gameObject.SetActive(false);
        //rightStaticText.gameObject.SetActive(false);
    }
    
    public void ActivateLeftFlipText(int rightPageIndex)
    {
        /*if (rightPageIndex == 0)
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
        
        /*if (leftFlipText != null)
        {
            leftFlipText.text = "";
            leftStaticText.text = "";
        }*/
        
        leftFlipText.gameObject.SetActive(true);
        rightFlipText.gameObject.SetActive(true);
        
        int leftIndex = rightPageIndex - 2;
        int rightIndex = rightPageIndex - 1;
        
        SetText(leftFlipText, leftIndex);
        SetText(rightFlipText, rightIndex);
        
        leftStaticText.gameObject.SetActive(false);
        //rightStaticText.gameObject.SetActive(false);
    }

    public void DeactivateFlipText()
    {
        rightFlipText.gameObject.SetActive(false);

        leftFlipText.gameObject.SetActive(false);

        leftStaticText.gameObject.SetActive(true);

        rightStaticText.gameObject.SetActive(true);
    }
    
    public void CommitPageChange(int newRightPageIndex)
    {
        UpdateVisiblePage(newRightPageIndex);
    }
}
