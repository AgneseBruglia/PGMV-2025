using UnityEngine;

public class PlantPickup : MonoBehaviour
{
    public GameObject player; // Assegna manualmente il player con il componente PickUpPlant

    public void OnPlayerInteract()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            PickUpPlant pickupScript = player.GetComponent<PickUpPlant>();
            if (pickupScript != null)
            {
                pickupScript.grab(hit);
            }
            else
            {
                Debug.LogWarning("PickUpPlant script non trovato sul player.");
            }
        }
    }
}
