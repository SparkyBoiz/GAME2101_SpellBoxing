using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} has been defeated!");
            Destroy(gameObject);
        }
    }
}