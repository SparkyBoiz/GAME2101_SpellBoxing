using UnityEngine;
using System.Collections.Generic;

public class M_Turn : MonoBehaviour
{
    public static M_Turn Instance { get; private set; }

    [SerializeField] private P_Controller player1;
    [SerializeField] private P_Controller player2;
    [SerializeField] private P_Health player1Health;
    [SerializeField] private P_Health player2Health;

    [SerializeField] private float turnDuration = 5f;
    [SerializeField] private float turnDurationDecay = 0.2f;
    [SerializeField] private float minimumTurnDuration = 1.5f;
    [SerializeField] private int healAmount = 20;

    [Header("Spell Randomization")]
    [SerializeField] private List<SpellSequence> masterSpellSequences;

    private float currentTurnTimer;
    private float currentMaxTurnDuration;
    private bool isFirstRound = true;

    public float CurrentTurnTimer => currentTurnTimer;
    public float TurnDuration => currentMaxTurnDuration;
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

    private SpellType p1QueuedSpell;
    private SpellType p2QueuedSpell;
    private int p1QueuedDamage;
    private int p2QueuedDamage;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        currentMaxTurnDuration = turnDuration;
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
            p1DamageMultiplier = damageMultiplierCalc.GetMultiplier(currentTurnTimer, currentMaxTurnDuration);
    }

    private void CalculateP2Multiplier()
    {
        if (damageMultiplierCalc != null)
            p2DamageMultiplier = damageMultiplierCalc.GetMultiplier(currentTurnTimer, currentMaxTurnDuration);
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

        bool p1HasSpell = player1 != null && player1.HasQueuedSpell;
        bool p2HasSpell = player2 != null && player2.HasQueuedSpell;

        if (!p1HasSpell && player1Health != null)
        {
            player1Health.TakeDamage(10);
        }
        if (!p2HasSpell && player2Health != null)
        {
            player2Health.TakeDamage(15);
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
            var p1Collision = player1.QueuedSpell.GetComponent<SpellCollision>();
            var p2Collision = player2.QueuedSpell.GetComponent<SpellCollision>();
            if (p1Collision != null)
            {
                p1QueuedSpell = p1Collision.spellType;
                p1QueuedDamage = p1Collision.spellDamage;
            }
            if (p2Collision != null)
            {
                p2QueuedSpell = p2Collision.spellType;
                p2QueuedDamage = p2Collision.spellDamage;
            }

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
        
        if (!isFirstRound)
        {
            currentMaxTurnDuration = Mathf.Max(minimumTurnDuration, currentMaxTurnDuration - turnDurationDecay);
        }
        isFirstRound = false;

        currentTurnTimer = currentMaxTurnDuration;
        if (player1 != null) player1.enabled = true;
        if (player2 != null) player2.enabled = true;
        
        p1DamageMultiplier = 1f;
        p2DamageMultiplier = 1f;

        RandomizeAndDistributeSpellSequences();
        OnAttackerChanged?.Invoke(player1IsAttacker);
    }

    private void RandomizeAndDistributeSpellSequences()
    {
        // Randomize the master list
        for (int i = 0; i < masterSpellSequences.Count; i++)
        {
            SpellSequence seq = masterSpellSequences[i];
            int length = (seq.sequence != null && seq.sequence.Count > 0) ? seq.sequence.Count : 3;

            var newSequence = new List<InputDirection>();
            for (int j = 0; j < length; j++)
            {
                InputDirection randomDir = (InputDirection)Random.Range(0, 4);
                newSequence.Add(randomDir);
            }
            seq.sequence = newSequence;
            masterSpellSequences[i] = seq;
        }

        // Distribute to players
        if (player1 != null)
            player1.SetSpellSequences(masterSpellSequences);
        if (player2 != null)
            player2.SetSpellSequences(masterSpellSequences);
    }

    private void OnDestroy()
    {
        if (player1 != null) player1.OnSpellCast -= CheckBothSpellsCast;
        if (player2 != null) player2.OnSpellCast -= CheckBothSpellsCast;

        if (player1 != null) player1.OnSpellCast -= CalculateP1Multiplier;
        if (player2 != null) player2.OnSpellCast -= CalculateP2Multiplier;
    }

    public void OnSpellCollision()
    {
        if (collisionProcessed) return;
        collisionProcessed = true;

        bool sameType = p1QueuedSpell == p2QueuedSpell;

        if (sameType)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySpellMatchSFX(p1QueuedSpell);
            }

            // If spells are the same type, the attacker's spell hits the defender.
            if (player1IsAttacker)
            {
                Debug.Log($"[M_Turn] P1 is the attacker! Dealing {p1QueuedDamage} base damage to player2Health.");
                int finalDamage = damageMultiplierCalc != null ? Mathf.RoundToInt(p1QueuedDamage * p1DamageMultiplier) : p1QueuedDamage;
                if (player2Health != null) player2Health.TakeDamage(finalDamage);
            }
            else
            {
                Debug.Log($"[M_Turn] P2 is the attacker! Dealing {p2QueuedDamage} base damage to player1Health.");
                int finalDamage = damageMultiplierCalc != null ? Mathf.RoundToInt(p2QueuedDamage * p2DamageMultiplier) : p2QueuedDamage;
                if (player1Health != null) player1Health.TakeDamage(finalDamage);
            }
        }
        else
        {
            bool p1CountersP2 = Counters(p1QueuedSpell, p2QueuedSpell);
            bool p2CountersP1 = Counters(p2QueuedSpell, p1QueuedSpell);

            if (p1CountersP2)
            {
                if (player1Health != null) player1Health.Heal(healAmount);
            }
            else if (p2CountersP1)
            {
                if (player2Health != null) player2Health.Heal(healAmount);
            }
            else
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlayFizzleSFX();
            }

            player1IsAttacker = !player1IsAttacker;
        }

        StartRound();
    }

    private bool Counters(SpellType attacker, SpellType defender)
    {
        return (attacker == SpellType.Water && defender == SpellType.Fire) ||
               (attacker == SpellType.Fire && defender == SpellType.Earth) ||
               (attacker == SpellType.Earth && defender == SpellType.Lightning) ||
               (attacker == SpellType.Lightning && defender == SpellType.Water);
    }
}