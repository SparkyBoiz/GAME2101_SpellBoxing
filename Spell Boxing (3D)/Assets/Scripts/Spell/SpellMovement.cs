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
            targetPoint.y = transform.position.y;
            direction = (targetPoint - transform.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}