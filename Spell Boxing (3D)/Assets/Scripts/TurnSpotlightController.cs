using UnityEngine;

public class TurnSpotlightController : MonoBehaviour
{
    [Header("Player Spotlights")]
    [Tooltip("Assign the Light component for Player 1 here.")]
    [SerializeField] private Light player1Light;

    [Tooltip("Assign the Light component for Player 2 here.")]
    [SerializeField] private Light player2Light;

    private void Update()
    {
        if (M_Turn.Instance == null) return;

        bool isPlayer1 = M_Turn.Instance.IsPlayer1Turn;

        if (player1Light != null)
        {
            player1Light.enabled = isPlayer1;
        }

        if (player2Light != null)
        {
            player2Light.enabled = !isPlayer1;
        }
    }
}