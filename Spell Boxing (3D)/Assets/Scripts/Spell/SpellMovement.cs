using UnityEngine;

public class SpellMovement : MonoBehaviour
{
    [Tooltip("Speed at which the spell travels.")]
    [SerializeField] private float speed = 10f;

    private Vector3 direction;

    private void Start()
    {
        if (M_Turn.Instance != null)
        {
            Vector3 targetPoint = M_Turn.Instance.SpellTargetPosition;
            // Make the spell travel horizontally towards the midpoint between players
            targetPoint.y = transform.position.y;
            direction = (targetPoint - transform.position).normalized;
        }
        else
        {
            // Fallback in case M_Turn is not present in the scene (e.g., for testing)
            Debug.LogWarning("M_Turn instance not found. Spell will move based on its initial rotation.");
            direction = transform.forward;
        }
    }

    private void Update()
    {
        // Move in the calculated direction at a constant speed
        transform.position += direction * speed * Time.deltaTime;
    }
}