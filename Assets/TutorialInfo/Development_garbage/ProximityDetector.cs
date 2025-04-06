using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    public GameObject player; // Reference to the player or object to detect
    public DrawerController drawer_controller; // Reference to the DrawerController

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Player entered the trigger area
            drawer_controller.active = true; // Open the drawer
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            // Player exited the trigger area
            drawer_controller.active = false; // Open the drawer
        }
    }
}