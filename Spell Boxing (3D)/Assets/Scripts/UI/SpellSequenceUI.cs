using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class SpellSequenceUI : MonoBehaviour
{
    [SerializeField] private P_Controller playerController;
    [SerializeField] private TextMeshProUGUI sequenceText;

    [Header("Arrow Visuals")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform[] spellContainers; // Assign one UI container per spell

    private List<GameObject> spawnedArrows = new List<GameObject>();

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnSpellSequencesUpdated += UpdateSequenceUI;
            
            // Update immediately in case the event fired before this UI was enabled
            UpdateSequenceUI();
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnSpellSequencesUpdated -= UpdateSequenceUI;
        }
    }

    private void UpdateSequenceUI()
    {
        if (playerController == null) return;

        // Clear previously spawned arrows
        foreach (var arrow in spawnedArrows)
        {
            Destroy(arrow);
        }
        spawnedArrows.Clear();

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < playerController.SpellSequences.Count; i++)
        {
            var spell = playerController.SpellSequences[i];

            if (sequenceText != null)
            {
            sb.AppendLine($"<b>{spell.name}</b>");
                // Optional: Comment the line below out if you no longer want the text version of the sequence
                sb.AppendLine(string.Join(" -> ", spell.sequence));
                sb.AppendLine(); // Add an empty line for better spacing between spells
            }

            // Spawn arrows if prefab and containers are assigned
            if (arrowPrefab != null && spellContainers != null && i < spellContainers.Length)
            {
                Transform container = spellContainers[i];
                if (container != null)
                {
                    foreach (var dir in spell.sequence)
                    {
                        GameObject arrow = Instantiate(arrowPrefab, container);
                        arrow.transform.localScale = Vector3.one; // Prevents scaling glitches in World Space Canvases
                        arrow.transform.localRotation = Quaternion.Euler(0, 0, GetRotationAngle(dir));
                        spawnedArrows.Add(arrow);
                    }
                }
            }
        }

        if (sequenceText != null) sequenceText.text = sb.ToString();
    }

    private float GetRotationAngle(InputDirection dir)
    {
        // Assuming your base arrow prefab points UP (0 degrees). 
        // Adjust these angles if your sprite faces a different default direction.
        return dir switch
        {
            InputDirection.Up => 0f,
            InputDirection.Right => -90f,
            InputDirection.Down => 180f,
            InputDirection.Left => 90f,
            _ => 0f
        };
    }
}