using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Controls the UI for displaying, editing, and saving L-System rules files.
/// Manages showing rules from JSON files, editing them in a text input, and saving changes.
/// </summary>
public class RulesDisplayController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Canvas GameObject containing the rules display UI.")]
    public GameObject rulesCanvas;

    [Tooltip("Panel that contains the rules text and editing UI.")]
    public GameObject rulesDisplayPanel;

    [Tooltip("Parent UI Canvas whose interactions will be disabled when rules UI is active.")]
    public GameObject parentUICanvas;

    [Tooltip("Text component displaying formatted rules.")]
    public TMP_Text rulesText;

    private CanvasGroup parentCanvasGroup;
    private CanvasGroup thisCanvasGroup;

    [Tooltip("UI panel for editing rules.")]
    public GameObject rulesChange;

    [Tooltip("Input field for editing rule text.")]
    public TMP_InputField ruleChangesInput;

    [Header("Data")]
    [Tooltip("Dropdown to select which rule file to load.")]
    public TMP_Dropdown ruleDropdown;

    /// <summary>
    /// Initialize UI state, get CanvasGroup components for interaction toggling.
    /// </summary>
    void Start()
    {
        // Hide rules and editing UI at start
        rulesCanvas.SetActive(false);
        rulesChange.SetActive(false);
        // Get this panel's CanvasGroup for interaction control
        thisCanvasGroup = rulesDisplayPanel.GetComponent<CanvasGroup>();
        // Get CanvasGroup from parent UI for disabling interactions when rules UI active
        if (parentUICanvas != null)
        {
            parentCanvasGroup = parentUICanvas.GetComponent<CanvasGroup>();
            if (parentCanvasGroup == null)
            {
                Debug.LogWarning("CanvasGroup not found on parentUICanvas. Add one to enable interaction toggling.");
            }
        }
    }

    /// <summary>
    /// Shows the rules UI.
    /// When opening, disables parent UI interactions and loads rules from the selected file.
    /// </summary>
    public void OpenRulesCanvas()
    {
        // Show rules UI
        rulesCanvas.SetActive(true);
        // Disable parent UI interactions
        if (parentCanvasGroup != null)
        {
            parentCanvasGroup.interactable = false;
            parentCanvasGroup.blocksRaycasts = false;
        }
        // Load and display rules from file
        DisplayRulesFromSelectedFile();
    }

    /// <summary>
    /// Loads the rules from the JSON file selected in the dropdown,
    /// then displays the rules in both formatted text and editable plain text.
    /// </summary>
    private void DisplayRulesFromSelectedFile()
    {
        string selectedRuleName = ruleDropdown.options[ruleDropdown.value].text;
        string filePath = RuleFileManager.GetRuleFilePath(selectedRuleName + ".json");
        // Check if the file exists
        if (!File.Exists(filePath))
        {
            rulesText.text = "File not found.";
            ruleChangesInput.text = "File not found.";
            return;
        }
        string jsonText = File.ReadAllText(filePath);
        try
        {
            // Deserialize JSON into array of LSystemRuleSet objects
            PlantGenerator.LSystemRuleSet[] ruleSets = JsonHelper.FromJson<PlantGenerator.LSystemRuleSet>(jsonText);
            string formattedText = "";
            string plainText = "";
            // Format rules for display and plain editing text
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
            // Set the UI texts accordingly
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

    /// <summary>
    /// Closes the rules UI and re-enables interactions on the parent UI canvas.
    /// </summary>
    public void CloseUI()
    {
        rulesCanvas.SetActive(false);
        if (parentCanvasGroup != null)
        {
            parentCanvasGroup.interactable = true;
            parentCanvasGroup.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// Opens the child UI (rules editing panel), disabling interaction with the main rules display.
    /// </summary>
    public void OpenChildUI()
    {
        if (thisCanvasGroup != null)
        {
            thisCanvasGroup.interactable = false;
            thisCanvasGroup.blocksRaycasts = false;
        }
        rulesChange.SetActive(true);
    }

    /// <summary>
    /// Parses the edited text input to reconstruct rule sets,
    /// converts them back to JSON and saves to file,
    /// then reloads and displays the updated rules.
    /// </summary>
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
                // Detect new predecessor rule
                if (line.StartsWith("Predecessor:"))
                {
                    // Save previous rule if any
                    if (currentRule != null && currentSuccessors != null)
                    {
                        currentRule.successors = currentSuccessors.ToArray();
                        ruleSets.Add(currentRule);
                    }
                    // Create new rule set with the predecessor extracted
                    currentRule = new PlantGenerator.LSystemRuleSet
                    {
                        predecessor = line.Substring("Predecessor:".Length).Trim()
                    };
                    currentSuccessors = new List<PlantGenerator.LSystemRuleSet.LSystemSuccessor>();
                }
                // Parse successors line
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
            // Add last parsed rule set if any
            if (currentRule != null && currentSuccessors != null)
            {
                currentRule.successors = currentSuccessors.ToArray();
                ruleSets.Add(currentRule);
            }
            // Convert updated rules back to JSON with formatting
            string newJson = JsonHelper.ToJson(ruleSets.ToArray(), prettyPrint: true);
            Debug.Log("Generated JSON:\n" + newJson);
            // Save the JSON back to file
            File.WriteAllText(filePath, newJson);
            Debug.Log("Saving rules file at: " + filePath);
            Debug.Log("Rules correctly saved.");
            // Refresh displayed rules after save
            DisplayRulesFromSelectedFile();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during rules parsing or saving: " + ex.Message);
        }
    }

    /// <summary>
    /// Closes the rules editing child UI and re-enables interaction on the main rules display panel.
    /// </summary>
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
