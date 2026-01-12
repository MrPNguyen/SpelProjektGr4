using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalSystem : MonoBehaviour
{
    [SerializeField] private List<string> pages;
    [SerializeField] private TMP_Text pageLeft;
    [SerializeField] private TMP_Text pageRight;
    [SerializeField] private Animator pageFlipper;
    private int currentPage = 0;
    private bool isFlipping = false;
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
        pageLeft.text = pages[0];
        pageRight.text = pages[1];
    }

    void Update()
    {
        if (currentPage == 0)
        {
            buttonPrevious.SetActive(false);
            PreviousHighlight.SetActive(false);
        }
        else
        {
            buttonPrevious.SetActive(true);
        }

        if (currentPage == pages.Count - 1)
        {
            buttonNext.SetActive(false);
            NextHighlight.SetActive(false);
        }
        else
        {
            buttonNext.SetActive(true);
        }
    }
    public void AddPage(string content)
    {
        pages.Add(content);
    }

    public void FlipPageRight()
    {
        if (!isFlipping)
        {
            StartCoroutine(PageRightFlipcoroutine());
        }
    }
    
    public void FlipPageLeft()
    {
        
        if (!isFlipping)
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
        if (pages.Count < 2) return;
        currentPage = Mathf.Clamp(currentPage, 0, pages.Count - 2);

        pageLeft.text = pages[currentPage];
        pageRight.text = pages[currentPage + 1];
       
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
}
