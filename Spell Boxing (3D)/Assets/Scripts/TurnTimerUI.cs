using UnityEngine;
using TMPro;

/// <summary>
/// Displays the turn timer on a TextMeshProUGUI component.
/// It polls the M_Turn singleton for the current time.
/// </summary>
public class TurnTimerUI : MonoBehaviour
{
    [Tooltip("The TextMeshProUGUI component to display the timer.")]
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (M_Turn.Instance != null && timerText != null)
        {
            // Get the current time left from the M_Turn manager
            float timeLeft = M_Turn.Instance.CurrentTurnTimer;

            // Ensure the timer doesn't display negative values
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            // Update the text display, formatted to one decimal place
            timerText.text = timeLeft.ToString("F1");
        }
        else if (timerText != null)
        {
            // If M_Turn is not available, you might want to hide the timer or show a default message
            timerText.text = ""; 
        }
    }
}