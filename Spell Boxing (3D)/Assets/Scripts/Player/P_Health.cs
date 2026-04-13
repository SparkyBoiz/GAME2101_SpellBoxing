using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class P_Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event System.Action<int, int> OnHealthChanged;

    private Animator animator;

    private static readonly int TookDamageHash = Animator.StringToHash("tookDamage");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"[P_Health] The GameObject '{gameObject.name}' just took {damage} damage!");
        
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger(TookDamageHash);
        }

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetBool(IsDeadHash, true);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeathSFX();
            }
            
            var playerController = GetComponent<P_Controller>();
            if (playerController != null) playerController.enabled = false;
            
            SceneManager.LoadScene("Scene_MainMenu");
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth > 0) // Only heal if they aren't dead
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}