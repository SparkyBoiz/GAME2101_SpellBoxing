using UnityEngine;
using UnityEngine.InputSystem;

public class P2_Controller : MonoBehaviour
{
    [Header("Spell Configuration")]
    [Tooltip("Spell cast when pressing Up Arrow")]
    [SerializeField] private GameObject spellPrefabUp;

    [Tooltip("Spell cast when pressing Left Arrow")]
    [SerializeField] private GameObject spellPrefabLeft;

    [Tooltip("Spell cast when pressing Down Arrow")]
    [SerializeField] private GameObject spellPrefabDown;

    [Tooltip("Spell cast when pressing Right Arrow")]
    [SerializeField] private GameObject spellPrefabRight;

    [Header("Cast Settings")]
    [Tooltip("Optional: The transform where spells will spawn. If null, uses the player's position.")]
    [SerializeField] private Transform castPoint;

    public event System.Action OnSpellCast;

    private GameObject queuedSpell;
    public bool HasQueuedSpell => queuedSpell != null;

    private InputAction castUpAction;
    private InputAction castLeftAction;
    private InputAction castDownAction;
    private InputAction castRightAction;

    private void Awake()
    {
        castUpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/upArrow");
        castLeftAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftArrow");
        castDownAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/downArrow");
        castRightAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/rightArrow");

        castUpAction.performed += ctx => CastSpell(spellPrefabUp);
        castLeftAction.performed += ctx => CastSpell(spellPrefabLeft);
        castDownAction.performed += ctx => CastSpell(spellPrefabDown);
        castRightAction.performed += ctx => CastSpell(spellPrefabRight);
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
    }

    private void CastSpell(GameObject spellPrefab)
    {
        if (spellPrefab == null)
        {
            Debug.LogWarning($"A spell prefab is missing on {gameObject.name}!");
            return;
        }

        queuedSpell = spellPrefab;
        OnSpellCast?.Invoke();
    }

    public void ExecuteQueuedSpell()
    {
        if (queuedSpell == null) return;

        Vector3 spawnPos = castPoint != null ? castPoint.position : transform.position;
        Quaternion spawnRot = castPoint != null ? castPoint.rotation : transform.rotation;

        spawnRot *= Quaternion.Euler(0, 180f, 0);

        Instantiate(queuedSpell, spawnPos, spawnRot);
        queuedSpell = null;
    }

    public void DiscardQueuedSpell()
    {
        queuedSpell = null;
    }
}