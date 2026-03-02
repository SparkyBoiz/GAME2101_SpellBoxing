using UnityEngine;

public class SpellData : MonoBehaviour
{
    [Tooltip("The ID used by the Animator to determine which cast animation to play.")]
    [SerializeField] private int spellAnimationId;

    public int SpellAnimationId => spellAnimationId;
}
