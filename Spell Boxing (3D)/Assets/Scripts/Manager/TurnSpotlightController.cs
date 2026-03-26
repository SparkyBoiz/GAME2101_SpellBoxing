using UnityEngine;

public class TurnSpotlightController : MonoBehaviour
{
    [SerializeField] private Light player1Light;

    [SerializeField] private Light player2Light;

    private void Update()
    {
        if (player1Light != null)
        {
            player1Light.enabled = true;
        }

        if (player2Light != null)
        {
            player2Light.enabled = true;
        }
    }
}