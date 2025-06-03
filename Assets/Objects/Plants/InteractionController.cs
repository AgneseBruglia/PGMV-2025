using UnityEngine;

/// <summary>
/// Manages interactions with plants in the scene,
/// particularly opening the UI interface associated with a specific plant.
/// </summary>
public class InteractionController : MonoBehaviour
{
    [Header("Plant UI Controller")]
    [Tooltip("Reference to the UIController responsible for displaying plant information.")]
    public UIController plantUIController;

    /// <summary>
    /// Opens the plant-specific UI interface using the provided plant GameObject.
    /// </summary>
    /// <param name="plant">The plant GameObject to display information for.</param>
    public void OpenInterface(GameObject plant)
    {
        // Delegates UI handling to the attached UIController
        plantUIController.GetComponent<UIController>().OpenUI(plant);
    }
}
