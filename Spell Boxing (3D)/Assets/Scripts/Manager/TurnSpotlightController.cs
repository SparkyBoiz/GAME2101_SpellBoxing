using UnityEngine;

public class TurnSpotlightController : MonoBehaviour
{
    [SerializeField] private Light player1Light;

    [SerializeField] private Light player2Light;

    private void OnEnable()
    {
        if (M_Turn.Instance != null)
        {
            M_Turn.Instance.OnAttackerChanged += HandleAttackerChanged;
            // Set initial state
            HandleAttackerChanged(M_Turn.Instance.Player1IsAttacker);
        }
    }

    private void OnDisable()
    {
        if (M_Turn.Instance != null)
        {
            M_Turn.Instance.OnAttackerChanged -= HandleAttackerChanged;
        }
    }

    private void HandleAttackerChanged(bool isPlayer1Attacker)
    {
        if (player1Light != null)
        {
            player1Light.enabled = isPlayer1Attacker;
        }
        if (player2Light != null)
        {
            player2Light.enabled = !isPlayer1Attacker;
        }
    }
}