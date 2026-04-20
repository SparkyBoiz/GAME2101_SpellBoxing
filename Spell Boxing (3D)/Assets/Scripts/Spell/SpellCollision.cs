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

    [SerializeField] public int spellDamage = 20;

    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private float effectDestroyDelay = 2f;

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
            if (destructionEffect != null)
            {
                GameObject effect = Instantiate(destructionEffect, transform.position, transform.rotation);
                Destroy(effect, effectDestroyDelay);
            }

            if (M_Turn.Instance != null)
            {
                M_Turn.Instance.OnSpellCollision();
            }

            Destroy(gameObject);
        }
    }
}