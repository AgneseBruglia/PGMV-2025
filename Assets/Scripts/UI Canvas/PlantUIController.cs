using UnityEngine;
using UnityEngine.Events;

public class PlantUIController : MonoBehaviour
{
    private GameObject canvasUIInstance;

    [Header("Data and prefab")]
    public GameObject canvasUIPrefab;

    public void TogglePlantUI()
    {
        if (canvasUIInstance == null)
        {
            OpenPlantUI();
        }
        else
        {
            CloseCanvas();
        }
    }

    void OpenPlantUI()
    {
        canvasUIInstance = Instantiate(canvasUIPrefab, transform.parent);

    }

    void CloseCanvas()
    {
        if (canvasUIInstance != null)
        {
            Destroy(canvasUIInstance);
            canvasUIInstance = null;
        }
    }
}
