using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeRemaining;
    public bool timerRunning = true;

    public void UpdateTimerDisplay()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();
    }

    public void ResetTimer(float newTime)
    {
        timeRemaining = newTime;
        timerRunning = true;
        UpdateTimerDisplay();
    }

}
