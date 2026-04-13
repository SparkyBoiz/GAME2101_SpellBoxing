using UnityEngine;
using TMPro;
using System.Text;

public class SpellSequenceUI : MonoBehaviour
{
    [SerializeField] private P_Controller playerController;
    [SerializeField] private TextMeshProUGUI sequenceText;

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
        if (playerController == null || sequenceText == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (var spell in playerController.SpellSequences)
        {
            sb.AppendLine($"<b>{spell.name}</b>");
            sb.AppendLine(string.Join(" -> ", spell.sequence));
            sb.AppendLine(); // Add an empty line for better spacing between spells
        }

        sequenceText.text = sb.ToString();
    }
}