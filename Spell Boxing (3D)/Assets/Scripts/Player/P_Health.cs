using UnityEngine;
using UnityEngine.SceneManagement;

public class P_Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // Event to notify UI elements of health changes
    public event System.Action<int, int> OnHealthChanged;

    private Animator animator;

    // Hashed parameter IDs for performance
    private static readonly int TookDamageHash = Animator.StringToHash("tookDamage");
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Notify listeners of the initial health value
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger(TookDamageHash);
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
            if (animator != null)
            {
                animator.SetBool(IsDeadHash, true);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeathSFX();
            }
            
            // Disable player controls to prevent a "ghost" from moving.
            var p1 = GetComponent<P1_Controller>();
            if (p1 != null) p1.enabled = false;
            var p2 = GetComponent<P2_Controller>();
            if (p2 != null) p2.enabled = false;
            
            // To allow the death animation to play, we should not destroy the object immediately.
            // Instead, disable components that allow further interaction.
            // The object can be destroyed via an Animation Event at the end of the death animation.
            // Destroy(gameObject); // This is not needed, as LoadScene will destroy all objects.

            // NOTE: Loading the scene immediately will cut the death animation short.
            // To see the full animation, you could move this line to OnDeathAnimationFinished().
            SceneManager.LoadScene("Scene_MainMenu");
        }
    }

    // This public method can be called from an Animation Event on the last frame of the death animation.
    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}