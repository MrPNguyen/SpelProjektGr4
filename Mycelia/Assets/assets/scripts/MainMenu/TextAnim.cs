using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class TextAnim : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;

    public string[] stringArray;

    [SerializeField] private float timeBtwnChars;
    [SerializeField] private float timeBtwnWords;

    int i = 0;

    [SerializeField] private bool AutomaticStart;
    
    [Header("NextButton")]
    [SerializeField] private TMP_Text nextButton;
    [SerializeField] private bool AutomaticAdvance;
    [SerializeField] private float startDelay;
    private bool isLineFinished;
    private Coroutine typingCoroutine;


    void Start()
    {
        if (AutomaticStart)
        {
            EndCheck();
        }
    }

    void Update()
    {
        if (i < stringArray.Length - 1)
        {
            if (nextButton != null)
            {
                nextButton.text = "Next>>";
            }
        }
        else
        {
            if (nextButton != null)
            {
                nextButton.text = "Venture Into The Forest>>";
            }
        }
    }

    void EndCheck()
    {
        if (i <= stringArray.Length - 1)
        {
            StopAllCoroutines();
            _textMeshPro.text = stringArray[i];
            _textMeshPro.maxVisibleCharacters = 0;
            typingCoroutine = StartCoroutine(TextVisible());
        }
    }
    
    private IEnumerator TextVisible()
    {
        isLineFinished = false;

        if (i == 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                isLineFinished = true;

                if (AutomaticAdvance)
                {
                    i++; 
                    Invoke("EndCheck", timeBtwnWords);
                }
                
                break;
            }
            
            counter++;
            yield return new WaitForSeconds(timeBtwnChars);
        }
    }

    public void BeginAnimation()
    {
        EndCheck();
    }

    public void NextLine()
    {
        CancelInvoke();
        if (!isLineFinished)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            _textMeshPro.maxVisibleCharacters = _textMeshPro.textInfo.characterCount;
            isLineFinished = true;
            return;
        }

        if (i < stringArray.Length - 1)
        {
            i++;
            EndCheck();
        }
        else
        {
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        }
    }

    public void RestartAnimation()
    {
        CancelInvoke();
        StopAllCoroutines();

        i = 0;
        _textMeshPro.text = "";
        _textMeshPro.maxVisibleCharacters = 0;

        EndCheck();    
    }
}


    