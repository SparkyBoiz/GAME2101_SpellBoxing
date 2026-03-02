using UnityEngine;
using UnityEngine.InputSystem;

public class P1_Controller : MonoBehaviour
{
    [Header("Spell Configuration")]
    [Tooltip("Spell cast when pressing W")]
    [SerializeField] private GameObject spellPrefabW;

    [Tooltip("Spell cast when pressing A")]
    [SerializeField] private GameObject spellPrefabA;

    [Tooltip("Spell cast when pressing S")]
    [SerializeField] private GameObject spellPrefabS;

    [Tooltip("Spell cast when pressing D")]
    [SerializeField] private GameObject spellPrefabD;

    [Header("Cast Settings")]
    [Tooltip("Optional: The transform where spells will spawn. If null, uses the player's position.")]
    [SerializeField] private Transform castPoint;

    public event System.Action OnSpellCast;

    private GameObject queuedSpell;
    public bool HasQueuedSpell => queuedSpell != null;

    private Animator animator;
    private InputAction castWAction;
    private InputAction castAAction;
    private InputAction castSAction;
    private InputAction castDAction;

    // Hashed parameter IDs for performance
    private static readonly int IsAttackingHash = Animator.StringToHash("isAttacking");
    private static readonly int SpellTypeHash = Animator.StringToHash("spellType");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        castWAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/w");
        castAAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/a");
        castSAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/s");
        castDAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/d");

        castWAction.performed += ctx => CastSpell(spellPrefabW);
        castAAction.performed += ctx => CastSpell(spellPrefabA);
        castSAction.performed += ctx => CastSpell(spellPrefabS);
        castDAction.performed += ctx => CastSpell(spellPrefabD);
    }

    private void OnEnable()
    {
        castWAction.Enable();
        castAAction.Enable();
        castSAction.Enable();
        castDAction.Enable();
    }

    private void OnDisable()
    {
        castWAction.Disable();
        castAAction.Disable();
        castSAction.Disable();
        castDAction.Disable();
    }

    private void CastSpell(GameObject spellPrefab)
    {
        if (spellPrefab == null)
        {
            Debug.LogWarning($"A spell prefab is missing on {gameObject.name}!");
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

        // Set spell type for animation
        var spellData = queuedSpell.GetComponent<SpellData>();
        if (spellData != null)
        {
            animator.SetInteger(SpellTypeHash, spellData.SpellAnimationId);
        }

        Instantiate(queuedSpell, spawnPos, spawnRot);
        queuedSpell = null;

        // It's often better to reset this at the end of the attack animation
        // using an Animation Event.
        animator.SetBool(IsAttackingHash, false);
    }

    public void DiscardQueuedSpell()
    {
        queuedSpell = null;
        animator.SetBool(IsAttackingHash, false);
    }
}
