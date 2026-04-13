using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputDirection
{
    Up,
    Down,
    Left,
    Right
}

[System.Serializable]
public struct SpellSequence
{
    public string name;
    public List<InputDirection> sequence;
    public GameObject spellPrefab;
}

public class P_Controller : MonoBehaviour
{
    [SerializeField] private string upBinding = "<Keyboard>/w";
    [SerializeField] private string leftBinding = "<Keyboard>/a";

    [SerializeField] private string downBinding = "<Keyboard>/s";

    [SerializeField] private string rightBinding = "<Keyboard>/d";

    [SerializeField] private List<SpellSequence> spellSequences = new List<SpellSequence>();

    public IReadOnlyList<SpellSequence> SpellSequences => spellSequences;

    [SerializeField] private Transform castPoint;

    [SerializeField] private Transform feedbackTarget;
    [SerializeField] private float inputFeedbackDistance = 0.5f;
    [SerializeField] private float inputFeedbackSpeed = 15f;

    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private Color errorFlashColor = Color.red;
    [SerializeField] private float errorFlashDuration = 0.2f;

    public event System.Action OnSpellCast;
    public event System.Action OnSpellSequencesUpdated;

    private GameObject queuedSpell;
    public GameObject QueuedSpell => queuedSpell;
    public bool HasQueuedSpell => queuedSpell != null;
    private List<InputDirection> currentInputSequence = new List<InputDirection>();

    private Animator animator;
    private InputAction castUpAction;
    private InputAction castLeftAction;
    private InputAction castDownAction;
    private InputAction castRightAction;

    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int SpellTypeHash = Animator.StringToHash("spellType");

    private Vector3 initialPosition;
    private Coroutine feedbackCoroutine;
    private Coroutine errorFlashCoroutine;
    private Color originalColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        castUpAction = new InputAction(type: InputActionType.Button, binding: upBinding);
        castLeftAction = new InputAction(type: InputActionType.Button, binding: leftBinding);
        castDownAction = new InputAction(type: InputActionType.Button, binding: downBinding);
        castRightAction = new InputAction(type: InputActionType.Button, binding: rightBinding);

        castUpAction.performed += ctx => OnInputDirection(InputDirection.Up);
        castLeftAction.performed += ctx => OnInputDirection(InputDirection.Left);
        castDownAction.performed += ctx => OnInputDirection(InputDirection.Down);
        castRightAction.performed += ctx => OnInputDirection(InputDirection.Right);
    }

    private void Start()
    {
        if (feedbackTarget == null)
        {
            feedbackTarget = transform;
        }
        initialPosition = feedbackTarget.position;

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }
        if (playerRenderer != null && playerRenderer.material != null)
        {
            originalColor = playerRenderer.material.color;
        }
        // Spell sequences are now set by M_Turn at the start of each round.
    }

    private void OnEnable()
    {
        castUpAction.Enable();
        castLeftAction.Enable();
        castDownAction.Enable();
        castRightAction.Enable();
    }

    private void OnDisable()
    {
        castUpAction.Disable();
        castLeftAction.Disable();
        castDownAction.Disable();
        castRightAction.Disable();
        currentInputSequence.Clear();
    }

    private void OnDestroy()
    {
        // The OnAttackerChanged subscription is no longer needed here.
    }

    public void SetSpellSequences(IReadOnlyList<SpellSequence> newSequences)
    {
        spellSequences.Clear();
        spellSequences.AddRange(newSequences);
        OnSpellSequencesUpdated?.Invoke();
    }

    private void OnInputDirection(InputDirection dir)
    {
        if (HasQueuedSpell) return;

        currentInputSequence.Add(dir);

        foreach (var spellSeq in spellSequences)
        {
            if (IsExactMatch(currentInputSequence, spellSeq.sequence))
            {
                TriggerFeedback(dir);
                CastSpell(spellSeq.spellPrefab);
                currentInputSequence.Clear();
                return;
            }
        }

        bool isValidPrefix = false;
        foreach (var spellSeq in spellSequences)
        {
            if (IsPrefix(currentInputSequence, spellSeq.sequence))
            {
                isValidPrefix = true;
                break;
            }
        }

        if (isValidPrefix)
        {
            TriggerFeedback(dir);
        }
        else
        {
            TriggerErrorFeedback();
            currentInputSequence.Clear();
            
            if (AudioManager.Instance != null)
            {
                
            }
        }
    }

    private bool IsExactMatch(List<InputDirection> input, List<InputDirection> target)
    {
        if (target == null || input.Count != target.Count) return false;
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != target[i]) return false;
        }
        return true;
    }

    private bool IsPrefix(List<InputDirection> input, List<InputDirection> target)
    {
        if (target == null || input.Count > target.Count) return false;
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] != target[i]) return false;
        }
        return true;
    }

    private void TriggerFeedback(InputDirection dir)
    {
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(InputFeedbackRoutine(dir));
    }

    private IEnumerator InputFeedbackRoutine(InputDirection dir)
    {
        Vector3 targetOffset = Vector3.zero;
        switch (dir)
        {
            case InputDirection.Up: targetOffset = feedbackTarget.up; break;
            case InputDirection.Down: targetOffset = -feedbackTarget.up; break;
            case InputDirection.Left: targetOffset = -feedbackTarget.right; break;
            case InputDirection.Right: targetOffset = feedbackTarget.right; break;
        }

        Vector3 targetPos = initialPosition + (targetOffset * inputFeedbackDistance);
        Vector3 startPos = feedbackTarget.position;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * inputFeedbackSpeed;
            feedbackTarget.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        t = 0;
        startPos = feedbackTarget.position;
        while (t < 1f)
        {
            t += Time.deltaTime * inputFeedbackSpeed;
            feedbackTarget.position = Vector3.Lerp(startPos, initialPosition, t);
            yield return null;
        }

        feedbackTarget.position = initialPosition;
    }

    private void TriggerErrorFeedback()
    {
        if (errorFlashCoroutine != null) StopCoroutine(errorFlashCoroutine);
        if (playerRenderer != null)
        {
            errorFlashCoroutine = StartCoroutine(ErrorFlashRoutine());
        }
    }

    private IEnumerator ErrorFlashRoutine()
    {
        if (playerRenderer == null || playerRenderer.material == null) yield break;

        playerRenderer.material.color = errorFlashColor;
        yield return new WaitForSeconds(errorFlashDuration);
        playerRenderer.material.color = originalColor;
    }

    private void CastSpell(GameObject spellPrefab)
    {
        if (spellPrefab == null)
        {

            return;
        }

        queuedSpell = spellPrefab;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySpellQueuedSFX();
        }
        OnSpellCast?.Invoke();
    }

    public void ExecuteQueuedSpell()
    {
        if (queuedSpell == null) return;

        Vector3 spawnPos = castPoint != null ? castPoint.position : transform.position;
        Quaternion spawnRot = castPoint != null ? castPoint.rotation : transform.rotation;

        var spellData = queuedSpell.GetComponent<SpellData>();
        if (spellData != null)
        {
            animator.SetFloat(SpellTypeHash, spellData.SpellAnimationId);
            Debug.Log($"[P_Controller] Triggering attack animation for spellType (Blend Tree threshold): {spellData.SpellAnimationId}");
        }

        animator.SetTrigger(IsAttackingHash);
        Instantiate(queuedSpell, spawnPos, spawnRot);
        queuedSpell = null;
    }

    public void DiscardQueuedSpell()
    {
        queuedSpell = null;
        animator.ResetTrigger(IsAttackingHash);
    }
}
