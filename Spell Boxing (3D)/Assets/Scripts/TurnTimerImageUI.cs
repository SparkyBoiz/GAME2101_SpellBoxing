using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates a UI Image's scale to represent the remaining turn time,
/// shrinking from both sides towards the middle.
/// </summary>
public class TurnTimerImageUI : MonoBehaviour
{
    [Tooltip("The UI Image to use for the timer. Ensure the RectTransform Pivot X is set to 0.5 for center shrinking.")]
    [SerializeField] private Image timerImage;

    private Vector3 initialScale;

    private void Start()
    {
        if (timerImage != null)
        {
            initialScale = timerImage.rectTransform.localScale;
        }
    }

    private void Update()
    {
        if (M_Turn.Instance == null || timerImage == null) return;

        float duration = M_Turn.Instance.TurnDuration;
        float current = M_Turn.Instance.CurrentTurnTimer;

        // Avoid division by zero
        if (duration > 0)
        {
            float ratio = Mathf.Clamp01(current / duration);

            // Scale the X axis based on the time remaining to shrink towards the center (if pivot is 0.5)
            Vector3 newScale = initialScale;
            newScale.x *= ratio;
            timerImage.rectTransform.localScale = newScale;
        }
    }
}