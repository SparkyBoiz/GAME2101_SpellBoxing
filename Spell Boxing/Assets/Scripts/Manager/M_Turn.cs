using UnityEngine;

public class M_Turn : MonoBehaviour
{
    public static M_Turn Instance { get; private set; }

    [Header("Player References")]
    [SerializeField] private P1_Controller player1;
    [SerializeField] private P2_Controller player2;
    [SerializeField] private P_Health player1Health;
    [SerializeField] private P_Health player2Health;

    [Header("Turn Settings")]
    [Tooltip("Time in seconds for each player's turn.")]
    [SerializeField] private float turnDuration = 5f;
    [SerializeField] private int damageAmount = 20;

    private float currentTurnTimer;
    private bool isPlayer1Turn;

    private bool player1IsAttacker = true;
    public bool Player1IsAttacker => player1IsAttacker;
    public event System.Action<bool> OnAttackerChanged;
    private bool isSecondInput = false;
    private bool waitingForResolution = false;
    private bool collisionProcessed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        player1IsAttacker = true;
        StartRound();

        if (player1 != null) player1.OnSpellCast += SwitchTurn;
        if (player2 != null) player2.OnSpellCast += SwitchTurn;
    }

    private void Update()
    {
        if (waitingForResolution) return;

        currentTurnTimer -= Time.deltaTime;

        if (currentTurnTimer <= 0f)
        {
            Debug.Log("Turn Timer Expired!");
            SwitchTurn();
        }
    }

    private void SwitchTurn()
    {
        if (waitingForResolution) return;

        if (!isSecondInput)
        {
            isSecondInput = true;
            isPlayer1Turn = !isPlayer1Turn;
            currentTurnTimer = turnDuration;
            UpdatePlayerControl();
        }
        else
        {
            waitingForResolution = true;
            if (player1 != null) player1.enabled = false;
            if (player2 != null) player2.enabled = false;

            bool p1HasSpell = player1 != null && player1.HasQueuedSpell;
            bool p2HasSpell = player2 != null && player2.HasQueuedSpell;

            if (p1HasSpell && p2HasSpell)
            {
                if (player1 != null) player1.ExecuteQueuedSpell();
                if (player2 != null) player2.ExecuteQueuedSpell();
                return;
            }

            if (p1HasSpell)
            {
                Debug.Log("Player 2 timed out! Player 1's spell is discarded.");
                if (player1 != null) player1.DiscardQueuedSpell();
            }
            else if (p2HasSpell)
            {
                Debug.Log("Player 1 timed out! Player 2's spell is discarded.");
                if (player2 != null) player2.DiscardQueuedSpell();
            }
            else
            {
                Debug.Log("Both players timed out! No damage dealt.");
            }

            player1IsAttacker = !player1IsAttacker;
            StartRound();
        }
    }

    private void StartRound()
    {
        waitingForResolution = false;
        collisionProcessed = false;
        isSecondInput = false;
        
        isPlayer1Turn = player1IsAttacker;
        
        currentTurnTimer = turnDuration;
        UpdatePlayerControl();
        Debug.Log($"New Round! Attacker: {(player1IsAttacker ? "Player 1" : "Player 2")}");
        OnAttackerChanged?.Invoke(player1IsAttacker);
    }

    private void UpdatePlayerControl()
    {
        if (player1 != null) player1.enabled = isPlayer1Turn;
        if (player2 != null) player2.enabled = !isPlayer1Turn;
    }

    private void OnDestroy()
    {
        if (player1 != null) player1.OnSpellCast -= SwitchTurn;
        if (player2 != null) player2.OnSpellCast -= SwitchTurn;
    }

    public void OnSpellCollision(bool sameType)
    {
        if (collisionProcessed) return;
        collisionProcessed = true;

        if (sameType)
        {
            Debug.Log($"Spells matched! Damage dealt to {(player1IsAttacker ? "Player 2" : "Player 1")}.");
            if (player1IsAttacker)
            {
                if (player2Health != null) player2Health.TakeDamage(damageAmount);
            }
            else
            {
                if (player1Health != null) player1Health.TakeDamage(damageAmount);
            }
        }
        else
        {
            Debug.Log("Spells fizzled! Attacker priority swaps.");
            player1IsAttacker = !player1IsAttacker;
        }

        StartRound();
    }
}