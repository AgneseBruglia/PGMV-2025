using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    public GameObject uiCanvas;
    public PlayerView playerView;
    public PlayerController playerController;
    public PlantSimulationController plantController;
    private GameObject hit_plant;
    public List<TextAsset> ruleConfigFiles;
    public TMP_InputField iterationsInput;
    public TMP_Dropdown ruleDropdown;
    public TMP_InputField scaleInput;
    public TMP_InputField angleInput;
    public Slider flowersSlider;
    public Button toggleWindButton;
    public TextMeshProUGUI textOn;
    public TextMeshProUGUI textOff;

    private bool isUIOpen = false;
    private bool isOn = true;

    void Start()
    {
        if (playerView == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                playerView = mainCam.GetComponent<PlayerView>();
        }
        CloseUI();
        toggleWindButton.onClick.AddListener(ToggleText);
        UpdateText();
    }

     void ToggleText()
    {
        isOn = !isOn;
        UpdateText();
    }

    void UpdateText()
    {
        textOn.gameObject.SetActive(isOn);
        textOff.gameObject.SetActive(!isOn);
    }

    public void OpenUI(GameObject plant)
    {
        uiCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isUIOpen = true;

        // Disables the player movement and interactions
        if (playerView != null)
            playerView.enabled = false;

        if (playerController != null)
            playerController.enabled = false;

        hit_plant = plant;

        Plant data = plant.GetComponent<PlantGenerator>().GetValues();

        // Setting of the default values
        iterationsInput.text = data.iterations.ToString();
        angleInput.text = data.delta.ToString();
        scaleInput.text = data.scale.ToString();
        string ruleName = data.ruleConfigFile != null ? data.ruleConfigFile.name : "";
        int index = ruleDropdown.options.FindIndex(opt => opt.text == ruleName);
        if (index >= 0)
        {
            ruleDropdown.value = index;
        }
        flowersSlider.value = data.flowerSpawnProbability;
    }

    public void CloseUI()
    {
        uiCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isUIOpen = false;

        // Enables the player movement and interactions
        if (playerView != null)
            playerView.enabled = true;

        if (playerController != null)
            playerController.enabled = true;
    }

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

    public void setRules()
    {
        string selectedRuleName = ruleDropdown.options[ruleDropdown.value].text;
        TextAsset selectedFile = ruleConfigFiles.Find(file => file.name == selectedRuleName);
        if (selectedFile != null)
        {
            hit_plant.GetComponent<PlantGenerator>().SetRuleConfigFile(selectedFile);
        }
        else
        {
            Debug.LogWarning($"No TextAsset found with the name {selectedRuleName}");
        }
    }

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

    public void setFlowerProbability()
    {
        float probability = flowersSlider.value;
        hit_plant.GetComponent<PlantGenerator>().SetFlowerSpawnProbability(probability);
    }

    public void ApplyChanges()
    {
        hit_plant.GetComponent<PlantGenerator>().RegeneratePlant();
    }

    public void ToggleWindEffect()
    {
        plantController.GetComponent<PlantSimulationController>().ToggleWind();
    }

    public void PlayGeneration()
    {
        plantController.GetComponent<PlantSimulationController>().PlaySimulation();
    }

    public void PauseGeneration()
    {
        plantController.GetComponent<PlantSimulationController>().PauseSimulation();
    }

    public void RestartGeneration()
    {
        plantController.GetComponent<PlantSimulationController>().RestartSimulation();
    }

    public void SinglePlantGrowth()
    {

    }

    public void MultiplePlantsGrowth()
    {
        
    }

}
