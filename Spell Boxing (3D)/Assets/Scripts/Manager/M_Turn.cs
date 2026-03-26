using UnityEngine;

public class M_Turn : MonoBehaviour
{
    public static M_Turn Instance { get; private set; }

    [SerializeField] private P_Controller player1;
    [SerializeField] private P_Controller player2;
    [SerializeField] private P_Health player1Health;
    [SerializeField] private P_Health player2Health;

    [SerializeField] private float turnDuration = 5f;
    [SerializeField] private int damageAmount = 20;

    private float currentTurnTimer;

    public float CurrentTurnTimer => currentTurnTimer;
    public float TurnDuration => turnDuration;
    public SpellDamageMultiplier DamageMultiplierCalc => damageMultiplierCalc;

    [SerializeField] private SpellDamageMultiplier damageMultiplierCalc;
    private float p1DamageMultiplier = 1f;
    private float p2DamageMultiplier = 1f;

    private bool player1IsAttacker = true;
    public Vector3 SpellTargetPosition { get; private set; }
    public bool Player1IsAttacker => player1IsAttacker;
    public event System.Action<bool> OnAttackerChanged;
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

        if (player1 != null) player1.OnSpellCast += CheckBothSpellsCast;
        if (player2 != null) player2.OnSpellCast += CheckBothSpellsCast;

        if (player1 != null) player1.OnSpellCast += CalculateP1Multiplier;
        if (player2 != null) player2.OnSpellCast += CalculateP2Multiplier;
    }

    private void CalculateP1Multiplier()
    {
        if (damageMultiplierCalc != null)
            p1DamageMultiplier = damageMultiplierCalc.GetMultiplier(currentTurnTimer, turnDuration);
    }

    private void CalculateP2Multiplier()
    {
        if (damageMultiplierCalc != null)
            p2DamageMultiplier = damageMultiplierCalc.GetMultiplier(currentTurnTimer, turnDuration);
    }

    private void Update()
    {
        if (waitingForResolution) return;

        if (player1 != null && player2 != null)
        {
            SpellTargetPosition = (player1.transform.position + player2.transform.position) / 2f;
        }

        currentTurnTimer -= Time.deltaTime;

        if (currentTurnTimer <= 0f)
        {
            HandleTimeOut();
        }
    }

    private void HandleTimeOut()
    {
        if (waitingForResolution) return;

        waitingForResolution = true;
        if (player1 != null) player1.enabled = false;
        if (player2 != null) player2.enabled = false;

        if (player1IsAttacker)
        {
            if (player1Health != null) player1Health.TakeDamage(10);
        }
        else
        {
            if (player2Health != null) player2Health.TakeDamage(10);
        }

        ResolveSpells();
    }

    private void CheckBothSpellsCast()
    {
        if (waitingForResolution) return;

        bool p1HasSpell = player1 != null && player1.HasQueuedSpell;
        bool p2HasSpell = player2 != null && player2.HasQueuedSpell;

        if (p1HasSpell && p2HasSpell)
        {
            waitingForResolution = true;
            if (player1 != null) player1.enabled = false;
            if (player2 != null) player2.enabled = false;
            ResolveSpells();
        }
    }

    private void ResolveSpells()
    {
        bool p1HasSpell = player1 != null && player1.HasQueuedSpell;
        bool p2HasSpell = player2 != null && player2.HasQueuedSpell;

        if (p1HasSpell && p2HasSpell)
        {
            collisionProcessed = false;
            if (player1 != null) player1.ExecuteQueuedSpell();
            if (player2 != null) player2.ExecuteQueuedSpell();
            return;
        }

        if (p1HasSpell)
        {
            if (player1 != null) player1.DiscardQueuedSpell();
        }
        else if (p2HasSpell)
        {
            if (player2 != null) player2.DiscardQueuedSpell();
        }

        player1IsAttacker = !player1IsAttacker;
        StartRound();
    }

    private void StartRound()
    {
        waitingForResolution = false;
        
        currentTurnTimer = turnDuration;
        if (player1 != null) player1.enabled = true;
        if (player2 != null) player2.enabled = true;
        
        p1DamageMultiplier = 1f;
        p2DamageMultiplier = 1f;

        OnAttackerChanged?.Invoke(player1IsAttacker);
    }

    private void OnDestroy()
    {
        if (player1 != null) player1.OnSpellCast -= CheckBothSpellsCast;
        if (player2 != null) player2.OnSpellCast -= CheckBothSpellsCast;

        if (player1 != null) player1.OnSpellCast -= CalculateP1Multiplier;
        if (player2 != null) player2.OnSpellCast -= CalculateP2Multiplier;
    }

    public void OnSpellCollision(bool sameType, SpellType spellType)
    {
        if (collisionProcessed) return;
        collisionProcessed = true;

        if (sameType)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySpellMatchSFX(spellType);
            }

            if (player1IsAttacker)
            {
                // Player 2 successfully defended; apply Player 2's casting speed multiplier to the counterattack damage
                int finalDamage = damageMultiplierCalc != null ? Mathf.RoundToInt(damageAmount * p2DamageMultiplier) : damageAmount;
                if (player1Health != null) player1Health.TakeDamage(finalDamage);
            }
            else
            {
                // Player 1 successfully defended; apply Player 1's casting speed multiplier to the counterattack damage
                int finalDamage = damageMultiplierCalc != null ? Mathf.RoundToInt(damageAmount * p1DamageMultiplier) : damageAmount;
                if (player2Health != null) player2Health.TakeDamage(finalDamage);
            }
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFizzleSFX();
            }

            player1IsAttacker = !player1IsAttacker;
        }

        StartRound();
    }
}