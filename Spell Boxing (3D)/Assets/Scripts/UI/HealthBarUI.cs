using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private P_Health playerHealth;

    [SerializeField] private Image healthBarFill;

    [SerializeField] private Slider healthBarSlider;

    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 5f;

    private int previousHealth;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            previousHealth = playerHealth.CurrentHealth;
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
        bool tookDamage = currentHealth < previousHealth;
        previousHealth = currentHealth;

        if (tookDamage)
        {
            TriggerShake();
        }

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

    private void TriggerShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            if (rectTransform != null) rectTransform.anchoredPosition = originalPosition;
        }
        else
        {
            if (rectTransform != null) originalPosition = rectTransform.anchoredPosition;
        }

        if (gameObject.activeInHierarchy && rectTransform != null)
        {
            shakeCoroutine = StartCoroutine(ShakeRoutine());
        }
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float strength = (1f - (elapsed / shakeDuration)) * shakeIntensity;
            rectTransform.anchoredPosition = originalPosition + (Vector3)(Random.insideUnitCircle * strength);
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
        shakeCoroutine = null;
    }
}
