using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class JournalSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text pageLeft;
    [SerializeField] private TMP_Text pageRight;
    [SerializeField] private Animator pageFlipper;
    [SerializeField] PlayableDirector director;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject Alert;
    
    private int currentPage = 0;
    private bool isFlipping = false;
    [SerializeField] private List<string> pages;
    
    [Header("Speed Toggle")]
    [SerializeField] private float delay;
    [SerializeField] private float duration;
    [SerializeField] private float flipSpeed;
    
    [Header("Buttons")]
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject buttonPrevious;
    [SerializeField] private GameObject NextHighlight;
    [SerializeField] private GameObject PreviousHighlight;

    void Start()
    {
        if (pages.Count > 0)
        {
            pageLeft.text = pages[0];
            if (pages.Count > 1)
            {
                pageRight.text = pages[1];
            }
            else
            {
                pageRight.text = "";
            }
        }
        else
        {
            pageLeft.text = "Dear winged one... scour the forest for the hidden past of Mycelia";
            pageRight.text = "";
        }
        director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
    }

    void Update()
    {
        bool canFlip = pages.Count > 1 && !isFlipping;
        
        buttonPrevious.SetActive(canFlip && currentPage > 0);
        PreviousHighlight.SetActive(canFlip && currentPage > 0);
        
        buttonNext.SetActive(canFlip && currentPage < pages.Count - 2);
        NextHighlight.SetActive(canFlip && currentPage < pages.Count - 2);
    }
    public void AddPage(string content)
    {
        pages.Add(content);
        UpdatePages();
    }

    public void FlipPageRight()
    {
        if (!isFlipping && pages.Count > 1)
        {
            StartCoroutine(PageRightFlipcoroutine());
        }
    }
    
    public void FlipPageLeft()
    {
        
        if (!isFlipping && pages.Count > 1)
        {
            StartCoroutine(PageLeftFlipcoroutine());
        }
    }

    private IEnumerator PageLeftFlipcoroutine()
    {
        Debug.Log("FlipPageRight");
        isFlipping = true;
        
        pageLeft.gameObject.SetActive(false);
        pageRight.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(duration);

        pageFlipper.SetTrigger("FlipRight");
        pageFlipper.SetFloat("speed", flipSpeed);
        
        yield return new WaitForSeconds(delay);
        
        currentPage -= 2;
        UpdatePages();
        
        yield return new WaitForSeconds(delay  + 0.5f);

        pageRight.gameObject.SetActive(true);
        pageLeft.gameObject.SetActive(true);

        isFlipping = false;
    }
    
    private IEnumerator PageRightFlipcoroutine()
    {
        Debug.Log("FlipPageLeft");
        isFlipping = true;
        
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageRight));
        
        yield return new WaitForSeconds(duration);

        pageFlipper.SetTrigger("FlipLeft");
        pageFlipper.SetFloat("speed", flipSpeed);
        
        yield return new WaitForSeconds(delay);
        
        currentPage += 2;
        UpdatePages();
        
        yield return new WaitForSeconds(delay + 0.5f);

        StartCoroutine(HighlightFadeIncoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeIncoroutine(duration, pageRight));

        isFlipping = false;
    }

    private void UpdatePages()
    {
        if (pages.Count == 0)
        {
            pageLeft.text = "";
            pageRight.text = "";
            return;
        }
    
        pageLeft.text = pages[currentPage];

        if (currentPage + 1 < pages.Count)
            pageRight.text = pages[currentPage + 1];
        else
            pageRight.text = "";
       
    }
    
    public IEnumerator HighlightFadeOutcoroutine(float duration, TMP_Text text)
    {
        if (text != null)
        {
            float startOpacity = text.alpha;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                text.alpha = Mathf.Lerp(startOpacity, 0f, time / duration);
                yield return null;
            }
        
            text.alpha = 0;
            text.gameObject.SetActive(false);
        }
    }
    public IEnumerator HighlightFadeIncoroutine(float duration, TMP_Text text)
    {
        if (text != null)
        {
            text.gameObject.SetActive(true);
            text.alpha = 0f;

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                text.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }
        
            text.alpha = 1f;
        }
    }

    public void ActivateBook()
    {
        director.Play();
        Time.timeScale = 0;
        animator.enabled = true;
        ResetCanvasOrder();
        Alert.SetActive(false);
    }

    public void DeactivateBook()
    {
        Time.timeScale = 1;
    }
    
    public void ResetCanvasOrder()
    {
        Page[] pages = GetComponentsInChildren<Page>(true);
        foreach (Page page in pages)
        {
            page.ReturnBack();
        }
    }
}
