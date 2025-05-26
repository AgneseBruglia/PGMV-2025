using UnityEngine;

public class CubicleController : MonoBehaviour
{
    public Transform door;        // Assign this in the Inspector
    public float speed = 2.0f;    // Rotation speed
    public float openAngle = 130f;

    private bool open = false;             // Desired state
    private Quaternion openRotation;       // Target open rotation
    private Quaternion closeRotation;      // Target closed rotation

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door is not assigned on: " + gameObject.name);
            return;
        }

        // Save closed rotation
        closeRotation = door.localRotation;

        // Define open rotation as Y-axis rotation from closed position
        openRotation = closeRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        if (door == null) return;

        Quaternion targetRotation = open ? openRotation : closeRotation;
        door.localRotation = Quaternion.Slerp(door.localRotation, targetRotation, Time.deltaTime * speed);
        
    }
    public void OnPlayerInteract(){
        Debug.Log("Cubicle clicked!");
        open=!open;
    }
}
