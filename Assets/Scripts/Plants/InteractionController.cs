using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("Plant UI Controller")]
    public UIController plantUIController;

    public void OpenInterface(GameObject plant)
    {
        plantUIController.GetComponent<UIController>().OpenUI(plant);
    }
}
