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
    [Header("Input Bindings")]
    [Tooltip("Input binding for the Up direction")]
    [SerializeField] private string upBinding = "<Keyboard>/w";

    [Tooltip("Input binding for the Left direction")]
    [SerializeField] private string leftBinding = "<Keyboard>/a";

    [Tooltip("Input binding for the Down direction")]
    [SerializeField] private string downBinding = "<Keyboard>/s";

    [Tooltip("Input binding for the Right direction")]
    [SerializeField] private string rightBinding = "<Keyboard>/d";

    [Header("Spell Sequences")]
    [Tooltip("Define combinations of directions to cast specific spells.")]
    [SerializeField] private List<SpellSequence> spellSequences = new List<SpellSequence>();

    [Header("Cast Settings")]
    [Tooltip("Optional: The transform where spells will spawn. If null, uses the player's position.")]
    [SerializeField] private Transform castPoint;

    [Header("Feedback Settings")]
    [Tooltip("The transform to move for feedback. If null, uses this player's transform.")]
    [SerializeField] private Transform feedbackTarget;
    [Tooltip("How far the player moves to provide input feedback.")]
    [SerializeField] private float inputFeedbackDistance = 0.5f;
    [Tooltip("How fast the player moves for input feedback.")]
    [SerializeField] private float inputFeedbackSpeed = 15f;

    public event System.Action OnSpellCast;

    private GameObject queuedSpell;
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

    private void CastSpell(GameObject spellPrefab)
    {
        if (spellPrefab == null)
        {

            return;
        }

        queuedSpell = spellPrefab;
        animator.SetBool(IsAttackingHash, true);

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
            animator.SetInteger(SpellTypeHash, spellData.SpellAnimationId);
        }

        Instantiate(queuedSpell, spawnPos, spawnRot);
        queuedSpell = null;

        animator.SetBool(IsAttackingHash, false);
    }

    public void DiscardQueuedSpell()
    {
        queuedSpell = null;
        animator.SetBool(IsAttackingHash, false);
    }
}
