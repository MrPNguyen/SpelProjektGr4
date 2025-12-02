using System;
using TMPro;
using UnityEngine;

public class ScoreTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text TimerText;
    private float currentScore = 0f;

    void Start()
    {
        TimerText = GetComponent<TMP_Text>();
        UpdateScoreText();
    }
    private void Update()
    {
        Debug.Log(Time.deltaTime);
        currentScore += Time.deltaTime * 5f;
        UpdateScoreText();
    }
    
    void UpdateScoreText()
    {
        TimerText.text = ((int)currentScore).ToString("D7");
    }
}
