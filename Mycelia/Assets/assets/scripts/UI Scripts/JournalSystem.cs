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
    [SerializeField] private GameObject Alert;
    
    private int currentPage = 0;
    private bool isFlipping = false;
    
    [Header("Speed Toggle")]
    [SerializeField] private float delay;
    [SerializeField] private float duration;
    [SerializeField] private float flipSpeed;
    
    [Header("Buttons")]
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject buttonPrevious;
    [SerializeField] private GameObject NextHighlight;
    [SerializeField] private GameObject PreviousHighlight;
    
    [Header("Sounds")]
    [SerializeField] private List<AudioClip> pageFlipSounds;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
        UpdatePage();
    }

    void Update()
    {
        bool canFlip = PageList.instance.pages.Count > 1 && !isFlipping;
        
        buttonPrevious.SetActive(canFlip && currentPage > 0);
        PreviousHighlight.SetActive(canFlip && currentPage > 0);
        
        buttonNext.SetActive(canFlip && currentPage < PageList.instance.pages.Count - 2);
        NextHighlight.SetActive(canFlip && currentPage < PageList.instance.pages.Count - 2);
    }
    public void AddPage(string content)
    {
        PageList.instance.pages.Add(content);
        
        currentPage = Mathf.Max(0, PageList.instance.pages.Count - 2);

        UpdatePage();
    }

    public void FlipPageRight()
    {
        if (!isFlipping && PageList.instance.pages.Count > 1)
        {
            StartCoroutine(PageRightFlipcoroutine());
        }
    }
    
    public void FlipPageLeft()
    {
        
        if (!isFlipping && PageList.instance.pages.Count > 1)
        {
            StartCoroutine(PageLeftFlipcoroutine());
        }
    }

    private IEnumerator PageLeftFlipcoroutine()
    {
        Debug.Log("FlipPageRight");
        isFlipping = true;
        
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageRight));
        
        yield return new WaitForSecondsRealtime(duration);

        if (pageFlipSounds != null && pageFlipSounds.Count > 0)
        {
            PlayOneShot(pageFlipSounds[Random.Range(0, pageFlipSounds.Count)]);
        }
        pageFlipper.SetFloat("speed", flipSpeed);
        pageFlipper.SetTrigger("FlipRight");
        
        yield return new WaitForSecondsRealtime(delay);
        
        currentPage -= 2;
        UpdatePage();
        
        yield return new WaitForSecondsRealtime(delay  + 0.5f);

        StartCoroutine(HighlightFadeIncoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeIncoroutine(duration, pageRight));

        isFlipping = false;
    }
    
    private IEnumerator PageRightFlipcoroutine()
    {
        Debug.Log("FlipPageLeft");
        isFlipping = true;
        
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeOutcoroutine(duration, pageRight));
        
        yield return new WaitForSecondsRealtime(duration);
        if (pageFlipSounds != null && pageFlipSounds.Count > 0)
        {
            PlayOneShot(pageFlipSounds[Random.Range(0, pageFlipSounds.Count)]);
        }
        pageFlipper.SetFloat("speed", flipSpeed);
        pageFlipper.SetTrigger("FlipLeft");
        
        yield return new WaitForSecondsRealtime(delay);
        
        currentPage += 2;
        UpdatePage();
        
        yield return new WaitForSecondsRealtime(delay + 0.5f);

        StartCoroutine(HighlightFadeIncoroutine(duration, pageLeft));
        StartCoroutine(HighlightFadeIncoroutine(duration, pageRight));

        isFlipping = false;
    }

    private void UpdatePage()
    {
        if (PageList.instance.pages.Count == 0)
        {
            pageLeft.text = "Dear winged one... scour the forest for the hidden past of Mycelia";
            pageRight.text = "";
            return;
        }
    
        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, PageList.instance.pages.Count - 2));

        pageLeft.text = PageList.instance.pages[currentPage];

        if (currentPage + 1 < PageList.instance.pages.Count)
            pageRight.text = PageList.instance.pages[currentPage + 1];
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
                time += Time.unscaledDeltaTime;
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
                time += Time.unscaledDeltaTime;
                text.alpha = Mathf.Lerp(0f, 1f, time / duration);
                yield return null;
            }
        
            text.alpha = 1f;
        }
    }

    public void ActivateBook()
    {
        director.Play();
        director.stopped += OnTimeLinesStopped;
        
        Time.timeScale = 0;
        foreach (Animator animator in GetComponentsInChildren<Animator>())
        {
            animator.enabled = true;
        }
        ResetCanvasOrder();
        Alert.SetActive(false);
        
        UpdatePage();
    }

    public void DeactivateBook()
    {
        Time.timeScale = 1;
    }

    private void OnTimeLinesStopped(PlayableDirector d)
    {
        director.stopped -= OnTimeLinesStopped;
        director.Stop();
    }
    
    public void ResetCanvasOrder()
    {
        Page[] pageList = GetComponentsInChildren<Page>(true);
        foreach (Page page in pageList)
        {
            page.ReturnBack();
        }
    }
    
    private void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
