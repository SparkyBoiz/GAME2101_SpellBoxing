using UnityEngine;

public class SpellDamageMultiplier : MonoBehaviour
{
    [SerializeField] private float maxMultiplier = 2.0f;

    [SerializeField] private float minMultiplier = 1.0f;

    public float GetMultiplier(float timeRemaining, float turnDuration)
    {
        if (turnDuration <= 0f) return minMultiplier;

        float timeRatio = Mathf.Clamp01(timeRemaining / turnDuration);

        return Mathf.Lerp(minMultiplier, maxMultiplier, timeRatio);
    }
}