using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a health bar UI element, updating it based on events from a P_Health component.
/// Can use either a UI Image with Fill Amount or a UI Slider.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("The health component of the player to track.")]
    [SerializeField] private P_Health playerHealth;

    [Header("UI Elements (Use one or both)")]
    [Tooltip("(Optional) The UI Image component to use as the health bar fill. It should have its Image Type set to 'Filled'.")]
    [SerializeField] private Image healthBarFill;

    [Tooltip("(Optional) The UI Slider component to use as the health bar.")]
    [SerializeField] private Slider healthBarSlider;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
            // Also update the bar immediately on enable to reflect current state
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }

    /// <summary>
    /// Updates the health bar's visual representation based on current and max health.
    /// </summary>
    /// <param name="currentHealth">The player's current health.</param>
    /// <param name="maxHealth">The player's maximum health.</param>
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        // Handle Image fill
        if (healthBarFill != null)
        {
            if (maxHealth <= 0)
            {
                healthBarFill.fillAmount = 0;
            }
            else
            {
                healthBarFill.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
            }
        }

        // Handle Slider value
        if (healthBarSlider != null)
        {
            if (maxHealth <= 0)
            {
                healthBarSlider.value = 0;
            }
            else
            {
                healthBarSlider.maxValue = maxHealth;
                healthBarSlider.value = currentHealth;
            }
        }
    }
}
