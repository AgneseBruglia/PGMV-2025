using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RulesDisplayController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rulesCanvas;
    public GameObject parentUICanvas;
    public TMP_Text rulesText;
    private CanvasGroup parentCanvasGroup;

    [Header("Data")]
    public List<TextAsset> ruleConfigFiles;
    public TMP_Dropdown ruleDropdown;

    void Start()
    {
        rulesCanvas.SetActive(false);

        if (parentUICanvas != null)
        {
            parentCanvasGroup = parentUICanvas.GetComponent<CanvasGroup>();
            if (parentCanvasGroup == null)
            {
                Debug.LogWarning("CanvasGroup non trovato su parentUICanvas. Aggiungine uno per abilitare/disabilitare interazioni.");
            }
        }
    }

    public void ToggleRulesCanvas()
    {
        bool isOpening = !rulesCanvas.activeSelf;

        rulesCanvas.SetActive(isOpening);

        if (parentCanvasGroup != null)
        {
            parentCanvasGroup.interactable = !isOpening;
            parentCanvasGroup.blocksRaycasts = !isOpening;
        }

        if (isOpening)
            DisplayRulesFromSelectedFile();
    }

    private void DisplayRulesFromSelectedFile()
    {
        string selectedRuleName = ruleDropdown.options[ruleDropdown.value].text;
        TextAsset selectedFile = ruleConfigFiles.Find(file => file.name == selectedRuleName);

        if (selectedFile == null)
        {
            rulesText.text = "File not found.";
            return;
        }

        try
        {
            PlantGenerator.LSystemRuleSet[] ruleSets = JsonHelper.FromJson<PlantGenerator.LSystemRuleSet>(selectedFile.text);

            string formattedText = "";
            foreach (var ruleSet in ruleSets)
            {
                formattedText += $"<b>Predecessor:</b> {ruleSet.predecessor}\n";
                foreach (var succ in ruleSet.successors)
                {
                    formattedText += $"\t→ {succ.successor} (p={succ.probability})\n";
                }
                formattedText += "\n";
            }

            rulesText.text = formattedText;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Errore nella lettura del JSON: {e.Message}");
            rulesText.text = "Errore nella lettura delle regole.";
        }
    }

    public void CloseUI()
    {
        rulesCanvas.SetActive(false);
        parentCanvasGroup.interactable = true;
        parentCanvasGroup.blocksRaycasts = true;
    }
}
