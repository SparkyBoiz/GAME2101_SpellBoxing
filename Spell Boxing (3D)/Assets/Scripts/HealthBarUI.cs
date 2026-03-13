using UnityEngine;
using UnityEngine.UI;

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

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
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
