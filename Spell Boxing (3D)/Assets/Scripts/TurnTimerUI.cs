using UnityEngine;
using TMPro;

public class TurnTimerUI : MonoBehaviour
{
    [Tooltip("The TextMeshProUGUI component to display the timer.")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (M_Turn.Instance != null && timerText != null)
        {
            float timeLeft = M_Turn.Instance.CurrentTurnTimer;

            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            timerText.text = timeLeft.ToString("F1");
        }
        else if (timerText != null)
        {
            timerText.text = ""; 
        }
    }
}