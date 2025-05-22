using UnityEngine;

public class PlantInteraction : MonoBehaviour
{
    public PlantUIController uiController;

    public event System.Action OnPlantClicked;

    void OnMouseDown()
    {
        if (uiController != null)
        {
            uiController.TogglePlantUI();
        }

        OnPlantClicked?.Invoke();
    }
}
