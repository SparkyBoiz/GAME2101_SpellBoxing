using UnityEngine;
using TMPro;

public class DamageMultiplierUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplierText;

    [SerializeField] private P_Controller targetController;

    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color lockedColor = Color.yellow;

    private M_Turn turnManager;
    private SpellDamageMultiplier damageMultiplierCalc;
    private bool spellCasted = false;

    void Awake()
    {
        if (targetController != null)
        {
            targetController.OnSpellCast += OnSpellCasted;
        }
    }

    void Start()
    {
        turnManager = M_Turn.Instance;
        if (turnManager != null)
        {
            damageMultiplierCalc = turnManager.DamageMultiplierCalc;
            turnManager.OnAttackerChanged += OnNewRound;
        }
        else
        {
            Debug.LogWarning("M_Turn instance is null! Is the Turn Manager in the scene?", this);
        }

        if (multiplierText != null)
        {
            OnNewRound(true);
        }
        else
        {
            Debug.LogWarning("Multiplier Text is missing! Please assign it in the Inspector.", this);
        }

        if (damageMultiplierCalc == null)
        {
            Debug.LogWarning("Damage Multiplier Calc is missing! Did you assign the SpellDamageMultiplier script to the Turn Manager?", this);
        }
    }

    void OnDestroy()
    {
        if (turnManager != null)
        {
            turnManager.OnAttackerChanged -= OnNewRound;
        }
        if (targetController != null)
        {
            targetController.OnSpellCast -= OnSpellCasted;
        }
    }

    private void OnNewRound(bool isP1Attacker)
    {
        spellCasted = false;
        if (multiplierText != null)
        {
            multiplierText.gameObject.SetActive(true);
            multiplierText.color = activeColor;
        }
    }

    private void OnSpellCasted()
    {
        if (spellCasted) return;
        spellCasted = true;

        if (damageMultiplierCalc != null && turnManager != null && multiplierText != null)
        {
            float lockedMultiplier = damageMultiplierCalc.GetMultiplier(turnManager.CurrentTurnTimer, turnManager.TurnDuration);
            multiplierText.text = $"x{lockedMultiplier:F2}";
            multiplierText.color = lockedColor;
        }
    }

    void Update()
    {
        if (spellCasted || turnManager == null || damageMultiplierCalc == null || multiplierText == null || !multiplierText.gameObject.activeInHierarchy)
        {
            return;
        }

        float currentMultiplier = damageMultiplierCalc.GetMultiplier(turnManager.CurrentTurnTimer, turnManager.TurnDuration);
        multiplierText.text = $"x{currentMultiplier:F2}";
    }
}