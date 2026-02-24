using UnityEngine;
using UnityEngine.InputSystem;

public class Player1Controller : MonoBehaviour
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

    private InputAction castWAction;
    private InputAction castAAction;
    private InputAction castSAction;
    private InputAction castDAction;

    private void Awake()
    {
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
        OnSpellCast?.Invoke();
    }

    public void ExecuteQueuedSpell()
    {
        if (queuedSpell == null) return;

        Vector3 spawnPos = castPoint != null ? castPoint.position : transform.position;
        Quaternion spawnRot = castPoint != null ? castPoint.rotation : transform.rotation;

        Instantiate(queuedSpell, spawnPos, spawnRot);
        queuedSpell = null;
    }

    public void DiscardQueuedSpell()
    {
        queuedSpell = null;
    }
}
