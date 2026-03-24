using UnityEngine;
using UnityEngine.UI;

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

        if (duration > 0)
        {
            float ratio = Mathf.Clamp01(current / duration);

            Vector3 newScale = initialScale;
            newScale.x *= ratio;
            timerImage.rectTransform.localScale = newScale;
        }
    }
}