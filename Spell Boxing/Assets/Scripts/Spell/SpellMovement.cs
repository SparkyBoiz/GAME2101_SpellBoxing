using UnityEngine;

public class SpellMovement : MonoBehaviour
{
    [Tooltip("Speed at which the spell travels forward.")]
    [SerializeField] private float speed = 10f;

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}