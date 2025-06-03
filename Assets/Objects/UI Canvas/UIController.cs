using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UIController handles the logic for displaying and interacting with the plant customization UI.
/// It enables user interaction, applies settings to plants, and integrates with the simulation logic.
/// </summary>
public class UIController : MonoBehaviour
{
    // References to UI and controller components
    public GameObject uiCanvas;
    public PlayerView playerView;
    public PlayerController playerController;
    public PlantSimulationController plantController;
    public RulesDisplayController rulesController;
    public Crossair crosshairScript;

    // Currently selected plant for customization
    private GameObject hit_plant;

    // UI elements for input and control
    public TMP_InputField iterationsInput;
    public TMP_Dropdown ruleDropdown;
    public TMP_InputField scaleInput;
    public TMP_InputField branchLenghtInput;
    public TMP_InputField angleInput;
    public Slider flowersSlider;
    public Slider singlePlantSlider;
    public Slider multiplePlantSlider;
    public Button toggleWindButton;
    public Button showRulesButton;
    public TextMeshProUGUI textOn;
    public TextMeshProUGUI textOff;

    // Wind toggle state
    private bool isOn = true;

    /// <summary>
    /// Initializes component references and UI state on startup.
    /// </summary>
    void Start()
    {
        // Fallback assignment of PlayerView and Crossair from main camera
        if (playerView == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                playerView = mainCam.GetComponent<PlayerView>();
                crosshairScript = mainCam.GetComponent<Crossair>();
            }
        }
        CloseUI(); // Start with UI closed
        toggleWindButton.onClick.AddListener(ToggleText); // Setup wind toggle UI
        UpdateText(); // Update toggle button label
    }

    /// <summary>
    /// Updates wind effect corresponding UI text.
    /// </summary>
    void ToggleText()
    {
        isOn = !isOn;
        UpdateText();
    }

    /// <summary>
    /// Updates text labels based on wind state.
    /// </summary>
    void UpdateText()
    {
        textOn.gameObject.SetActive(isOn);
        textOff.gameObject.SetActive(!isOn);
    }

    /// <summary>
    /// Opens the customization UI for a given plant and populates UI fields.
    /// </summary>
    public void OpenUI(GameObject plant)
    {
        uiCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Disable player controls for UI interaction
        if (playerView != null) playerView.enabled = false;
        if (playerController != null) playerController.enabled = false;
        if (crosshairScript != null) crosshairScript.enabled = false;
        hit_plant = plant;
        // Retrieve and display plant configuration
        Plant data = plant.GetComponent<PlantGenerator>().GetValues();
        iterationsInput.text = data.iterations.ToString();
        angleInput.text = data.delta.ToString();
        scaleInput.text = data.scale.ToString();
        branchLenghtInput.text = data.branchLength.ToString();
        // Set rule dropdown value
        string ruleName = data.ruleFileName ?? "";
        int index = ruleDropdown.options.FindIndex(opt => opt.text == ruleName);
        ruleDropdown.value = index >= 0 ? index : 0;
        // Set slider values
        flowersSlider.value = data.flowerSpawnProbability;
        singlePlantSlider.value = data.iterations;
    }

    /// <summary>
    /// Closes the customization UI and re-enables player control.
    /// </summary>
    public void CloseUI()
    {
        uiCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        if (playerView != null) playerView.enabled = true;
        if (playerController != null) playerController.enabled = true;
        if (crosshairScript != null) crosshairScript.enabled = true;
    }

    /// <summary>
    /// Sets the number of iterations in the selected plant.
    /// </summary>
    public void setIterations()
    {
        int parsedValue;
        if (int.TryParse(iterationsInput.text, out parsedValue))
        {
            hit_plant.GetComponent<PlantGenerator>().SetIterations(parsedValue);
        }
        else
        {
            Debug.LogWarning("Invalid Iterations input");
        }
    }

    /// <summary>
    /// Sets the rule configuration for the selected plant.
    /// </summary>
    public void setRules()
    {
        string selectedRuleName = ruleDropdown.options[ruleDropdown.value].text;
        var plantGenerator = hit_plant.GetComponent<PlantGenerator>();
        if (plantGenerator != null)
        {
            plantGenerator.SetRuleConfigFile(selectedRuleName + ".json");
        }
        else
        {
            Debug.LogWarning("PlantGenerator component not found!");
        }
    }

    /// <summary>
    /// Sets the plant scale value.
    /// </summary>
    public void setScale()
    {
        float scaleValue;
        if (float.TryParse(scaleInput.text, out scaleValue))
        {
            hit_plant.GetComponent<PlantGenerator>().SetScale(scaleValue);
        }
        else
        {
            Debug.LogWarning("Invalid Scale input");
        }
    }

    /// <summary>
    /// Sets the branch scale.
    /// </summary>
    public void setBranchLenght()
    {
        float branchValue;
        if (float.TryParse(branchLenghtInput.text, out branchValue))
        {
            hit_plant.GetComponent<PlantGenerator>().SetBranchLenght(branchValue);
        }
        else
        {
            Debug.LogWarning("Invalid Branch length input");
        }
    }

    /// <summary>
    /// Sets the growth angle (delta) of the plant.
    /// </summary>
    public void setAngle()
    {
        float parsedAngle;
        if (float.TryParse(angleInput.text, out parsedAngle))
        {
            hit_plant.GetComponent<PlantGenerator>().SetDelta(parsedAngle);
        }
        else
        {
            Debug.LogWarning("Invalid Angle input");
        }
    }

    /// <summary>
    /// Sets the flower spawn probability for the plant.
    /// </summary>
    public void setFlowerProbability()
    {
        float probability = flowersSlider.value;
        hit_plant.GetComponent<PlantGenerator>().SetFlowerSpawnProbability(probability);
    }

    /// <summary>
    /// Applies all changes to the selected plant and regenerates it.
    /// </summary>
    public void ApplyChanges()
    {
        if (hit_plant == null) return;
        var plantGenerator = hit_plant.GetComponent<PlantGenerator>();
        if (plantGenerator == null) return;
        StartCoroutine(ApplyChangesRoutine(plantGenerator));
    }

    /// <summary>
    /// Coroutine to reset and regenerate the plant after applying new settings.
    /// </summary>
    private IEnumerator ApplyChangesRoutine(PlantGenerator plantGenerator)
    {
        yield return StartCoroutine(plantGenerator.ResetPlant());
        plantGenerator.GeneratePlant();
        hit_plant = plantGenerator.gameObject;
    }

    /// <summary>
    /// Toggles wind simulation effect in the plant simulation controller.
    /// </summary>
    public void ToggleWindEffect()
    {
        plantController.GetComponent<PlantSimulationController>().ToggleWind();
    }

    /// <summary>
    /// Starts/resumes the plant generation simulation.
    /// </summary>
    public void PlayGeneration()
    {
        plantController.GetComponent<PlantSimulationController>().PlaySimulation();
    }

    /// <summary>
    /// Pauses the plant generation simulation.
    /// </summary>
    public void PauseGeneration()
    {
        plantController.GetComponent<PlantSimulationController>().PauseSimulation();
    }

    /// <summary>
    /// Restarts the plant generation simulation from the beginning.
    /// </summary>
    public void RestartGeneration()
    {
        StartCoroutine(plantController.GetComponent<PlantSimulationController>().RestartSimulation());
    }

    /// <summary>
    /// Grows a single plant with the iteration value from the singlePlantSlider.
    /// </summary>
    public void SinglePlantGrowth()
    {
        if (hit_plant == null) return;
        var plantGenerator = hit_plant.GetComponent<PlantGenerator>();
        int iterationsNumber = (int)singlePlantSlider.value;
        plantGenerator.SetIterations(iterationsNumber);
        plantGenerator.GeneratePlant();
        hit_plant = plantGenerator.gameObject;
    }

    /// <summary>
    /// Triggers growth of multiple plants with the iteration value from the multiplePlantSlider using the simulation controller.
    /// </summary>
    public void MultiplePlantsGrowth()
    {
        int iterationsNumber = (int)multiplePlantSlider.value;
        StartCoroutine(plantController.GetComponent<PlantSimulationController>().MultiplePlantsGrowthRoutine(iterationsNumber));
    }

    /// <summary>
    /// Opens the rules display UI.
    /// </summary>
    public void DisplayRules()
    {
        Debug.Log("Rules button clicked");
        rulesController.OpenRulesCanvas();
    }
}
