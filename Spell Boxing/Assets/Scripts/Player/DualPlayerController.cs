using UnityEngine;
using UnityEngine.InputSystem;

public class DualPlayerController : MonoBehaviour
{
    [Header("Player 1 Configuration")]
    [Tooltip("Assign the Transform for Player 1 here.")]
    public Transform player1Transform;
    public float player1Speed = 5.0f;
    private InputAction m_Player1Move;

    [Header("Player 2 Configuration")]
    [Tooltip("Assign the Transform for Player 2 here.")]
    public Transform player2Transform;
    public float player2Speed = 5.0f;
    private InputAction m_Player2Move;

    private void Awake()
    {
        // Setup Player 1 Actions (WASD)
        m_Player1Move = new InputAction("Player1Move");
        m_Player1Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Setup Player 2 Actions (Arrow Keys)
        m_Player2Move = new InputAction("Player2Move");
        m_Player2Move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    private void OnEnable()
    {
        m_Player1Move.Enable();
        m_Player2Move.Enable();
    }

    private void OnDisable()
    {
        m_Player1Move.Disable();
        m_Player2Move.Disable();
    }

    private void Update()
    {
        // Move Player 1
        if (player1Transform != null)
        {
            var move = m_Player1Move.ReadValue<Vector2>();
            player1Transform.Translate(new Vector3(move.x, 0, move.y) * player1Speed * Time.deltaTime);
        }

        // Move Player 2
        if (player2Transform != null)
        {
            var move = m_Player2Move.ReadValue<Vector2>();
            player2Transform.Translate(new Vector3(move.x, 0, move.y) * player2Speed * Time.deltaTime);
        }
    }
}