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
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (animator != null)
        {
            animator.SetBool(TookDamageHash, true);
            StartCoroutine(ResetTakeDamage());
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

    private IEnumerator ResetTakeDamage()
    {
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
        {
            animator.SetBool(TookDamageHash, false);
        }
    }

    public void OnTakeDamageAnimationFinished()
    {
        if (animator != null)
        {
            animator.SetBool(TookDamageHash, false);
        }
    }

    public void OnDeathAnimationFinished()
    {
        Destroy(gameObject);
    }
}