using UnityEngine;

public class LightToggle : MonoBehaviour
{
    public Light targetLight;
    public Camera playerCamera; // La camera da cui lanci il raycast
    
    [SerializeField] private GameObject lightSource;

    private void OnPlayerInteract(){
        lightSource.SetActive(!lightSource.activeSelf);
    }

    
}
