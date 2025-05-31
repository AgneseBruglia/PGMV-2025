using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class RulesDisplayController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject rulesCanvas;
    public GameObject rulesDisplayPanel;
    public GameObject parentUICanvas;
    public TMP_Text rulesText;
    private CanvasGroup parentCanvasGroup;
    private CanvasGroup thisCanvasGroup;
    public GameObject rulesChange;
    public TMP_InputField ruleChangesInput;

    [Header("Data")]
    public TMP_Dropdown ruleDropdown;

    void Start()
    {
        rulesCanvas.SetActive(false);
        rulesChange.SetActive(false);

        thisCanvasGroup = rulesDisplayPanel.GetComponent<CanvasGroup>();

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
        string filePath = RuleFileManager.GetRuleFilePath(selectedRuleName + ".json");
        if (!File.Exists(filePath))
        {
            rulesText.text = "File not found.";
            ruleChangesInput.text = "File not found.";
            return;
        }
        string jsonText = File.ReadAllText(filePath);
        try
        {
            PlantGenerator.LSystemRuleSet[] ruleSets = JsonHelper.FromJson<PlantGenerator.LSystemRuleSet>(jsonText);
            string formattedText = "";
            string plainText = "";
            foreach (var ruleSet in ruleSets)
            {
                formattedText += $"<b>Predecessor:</b> {ruleSet.predecessor}\n";
                plainText += $"Predecessor: {ruleSet.predecessor}\n";
                foreach (var succ in ruleSet.successors)
                {
                    formattedText += $"\t→ {succ.successor} (p={succ.probability})\n";
                    plainText += $"\t-> {succ.successor} (p={succ.probability})\n";
                }
                formattedText += "\n";
                plainText += "\n";
            }
            rulesText.text = formattedText;
            ruleChangesInput.text = plainText;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during JSON file reading: {e.Message}");
            rulesText.text = "Error during rules reading.";
            ruleChangesInput.text = "Error during rules reading.";
        }
    }

    public void CloseUI()
    {
        rulesCanvas.SetActive(false);
        parentCanvasGroup.interactable = true;
        parentCanvasGroup.blocksRaycasts = true;
    }

    public void OpenChildUI()
    {
        if (thisCanvasGroup != null)
        {
            thisCanvasGroup.interactable = false;
            thisCanvasGroup.blocksRaycasts = false;
        }
        rulesChange.SetActive(true);
    }
    
    public void ChangeRules()
    {
        string selectedRuleName = ruleDropdown.options[ruleDropdown.value].text;
        string filePath = RuleFileManager.GetRuleFilePath(selectedRuleName + ".json");

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found.");
            return;
        }

        string inputText = ruleChangesInput.text;
        var ruleSets = new List<PlantGenerator.LSystemRuleSet>();

        try
        {
            string[] lines = inputText.Split('\n');
            PlantGenerator.LSystemRuleSet currentRule = null;
            List<PlantGenerator.LSystemRuleSet.LSystemSuccessor> currentSuccessors = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("Predecessor:"))
                {
                    if (currentRule != null && currentSuccessors != null)
                    {
                        currentRule.successors = currentSuccessors.ToArray();
                        ruleSets.Add(currentRule);
                    }

                    currentRule = new PlantGenerator.LSystemRuleSet
                    {
                        predecessor = line.Substring("Predecessor:".Length).Trim()
                    };
                    currentSuccessors = new List<PlantGenerator.LSystemRuleSet.LSystemSuccessor>();
                }
                else if (line.StartsWith("->") && currentRule != null && currentSuccessors != null)
                {
                    string succLine = line.Substring(2).Trim(); // remove '->'
                    string[] parts = succLine.Split(new string[] { "(p=" }, System.StringSplitOptions.None);

                    if (parts.Length == 2)
                    {
                        string successor = parts[0].Trim();
                        string probStr = parts[1].Trim(')', ' ');

                        if (float.TryParse(probStr, out float probability))
                        {
                            currentSuccessors.Add(new PlantGenerator.LSystemRuleSet.LSystemSuccessor
                            {
                                successor = successor,
                                probability = probability
                            });
                        }
                    }
                }
            }

            // Add last rule if exists
            if (currentRule != null && currentSuccessors != null)
            {
                currentRule.successors = currentSuccessors.ToArray();
                ruleSets.Add(currentRule);
            }

            string newJson = JsonHelper.ToJson(ruleSets.ToArray(), prettyPrint: true);
            Debug.Log("Generated JSON:\n" + newJson);
            File.WriteAllText(filePath, newJson);
            Debug.Log("Saving rules file at: " + filePath);
            Debug.Log("Rules correctly saved.");
            DisplayRulesFromSelectedFile(); // update UI
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during rules parsing or saving: " + ex.Message);
        }
    }

    public void CloseChildUI()
    {
        if (thisCanvasGroup != null)
        {
            thisCanvasGroup.interactable = true;
            thisCanvasGroup.blocksRaycasts = true;
        }
        rulesChange.SetActive(false);
    }
}
