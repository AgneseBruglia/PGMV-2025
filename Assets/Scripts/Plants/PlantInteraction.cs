using UnityEngine;

public class PlantInteraction : MonoBehaviour
{
    private Camera cam;
    public PlantUIController uiController;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform) // Se la pianta è stata cliccata
            {
                uiController.CreatePlantUI(); // Crea la UI per questa pianta
            }
        }
    }
}
