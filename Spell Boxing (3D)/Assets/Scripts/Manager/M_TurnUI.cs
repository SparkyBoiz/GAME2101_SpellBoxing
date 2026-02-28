using UnityEngine;

public class M_TurnUI : MonoBehaviour
{
    [Header("Icons")]
    [Tooltip("The UI GameObject (e.g. Image) representing the attacker (Sword).")]
    [SerializeField] private GameObject attackerIcon;
    [Tooltip("The UI GameObject (e.g. Image) representing the defender (Shield).")]
    [SerializeField] private GameObject defenderIcon;

    [Header("Player Tracking")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [Tooltip("Height offset above the player in world units.")]
    [SerializeField] private float heightOffset = 2.0f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (M_Turn.Instance == null) return;
        if (attackerIcon == null || defenderIcon == null) return;
        if (player1 == null || player2 == null) return;

        if (mainCamera == null) mainCamera = Camera.main;

        bool p1IsAttacker = M_Turn.Instance.Player1IsAttacker;

        UpdateIconPosition(attackerIcon, p1IsAttacker ? player1 : player2);
        UpdateIconPosition(defenderIcon, p1IsAttacker ? player2 : player1);
    }

    private void UpdateIconPosition(GameObject icon, Transform target)
    {
        Vector3 worldPos = target.position + Vector3.up * heightOffset;
        icon.transform.position = mainCamera.WorldToScreenPoint(worldPos);
    }
}