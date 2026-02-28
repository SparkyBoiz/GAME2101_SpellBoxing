using UnityEngine;

public enum SpellType
{
    Fire,
    Earth,
    Water,
    Lightning
}

public class SpellCollision : MonoBehaviour
{
    [SerializeField] public SpellType spellType;

    [Tooltip("Optional: Particle effect to spawn when the spell is destroyed.")]
    [SerializeField] private GameObject destructionEffect;

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.TryGetComponent<SpellCollision>(out SpellCollision otherSpell))
        {
            bool sameType = spellType == otherSpell.spellType;

            if (sameType)
            {
                Debug.Log("Spells collided with SAME type! Dealing Damage.");
                if (destructionEffect != null)
                {
                    Instantiate(destructionEffect, transform.position, transform.rotation);
                }
            }
            else
            {
                Debug.Log("Spells collided with DIFFERENT type! Fizzling out.");
            }

            if (M_Turn.Instance != null)
            {
                M_Turn.Instance.OnSpellCollision(sameType);
            }

            Destroy(gameObject);
        }
    }
}