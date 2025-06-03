using UnityEngine;

/// <summary>
/// DoorOpener controls the animated opening and closing of a two-part door
/// using local position interpolation. Triggered by player interaction.
/// </summary>
public class DoorOpener : MonoBehaviour
{
    // References to the left and right parts of the door
    public Transform door_2_right;
    public Transform door_2_left;

    // Local position offsets used to calculate open positions from the closed ones
    public Vector3 rightOpenOffset;
    public Vector3 leftOpenOffset;

    // Speed at which the door opens/closes
    public float openSpeed = 2f;

    // Internal positions for open/closed states
    private Vector3 rightClosedPosition;
    private Vector3 rightOpenPosition;
    private Vector3 leftClosedPosition;
    private Vector3 leftOpenPosition;

    // Door state flag
    private bool isOpen = false;

    /// <summary>
    /// Initializes door positions and calculates open target positions.
    /// </summary>
    void Start()
    {
        Debug.Log("Door opener initialized");
        // Save the initial (closed) local positions
        rightClosedPosition = door_2_right.localPosition;
        leftClosedPosition = door_2_left.localPosition;
        // Compute open target positions based on offsets
        rightOpenPosition = rightClosedPosition + rightOpenOffset;
        leftOpenPosition = leftClosedPosition + leftOpenOffset;
    }

    /// <summary>
    /// Smoothly interpolates door parts toward open or closed positions each frame.
    /// </summary>
    void Update()
    {
        // Determine target positions based on current state
        Vector3 rightTarget = isOpen ? rightOpenPosition : rightClosedPosition;
        Vector3 leftTarget = isOpen ? leftOpenPosition : leftClosedPosition;
        // Smooth transition using linear interpolation
        door_2_right.localPosition = Vector3.Lerp(door_2_right.localPosition, rightTarget, Time.deltaTime * openSpeed);
        door_2_left.localPosition = Vector3.Lerp(door_2_left.localPosition, leftTarget, Time.deltaTime * openSpeed);
    }

    /// <summary>
    /// Called externally when the player interacts with the door.
    /// Toggles the open/closed state and plays an audio cue.
    /// </summary>
    public void OnPlayerInteract()
    {
        Debug.Log("OnPlayerInteract called for: " + gameObject.name);
        // Toggle door state
        isOpen = !isOpen;
        // Play door sound (AudioSource must be attached to this GameObject)
        GetComponent<AudioSource>().Play();
    }
}
