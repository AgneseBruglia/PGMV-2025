using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public Transform door_2_right;
    public Transform door_2_left;

    public Vector3 rightOpenOffset;
    public Vector3 leftOpenOffset;
    public float openSpeed = 2f;

    private Vector3 rightClosedPosition;
    private Vector3 rightOpenPosition;
    private Vector3 leftClosedPosition;
    private Vector3 leftOpenPosition;

    private bool isOpen = false;

    void Start()
    {
        // Save the starting (closed) positions
        rightClosedPosition = door_2_right.localPosition;
        leftClosedPosition = door_2_left.localPosition;

        // Calculate open positions
        rightOpenPosition = rightClosedPosition + rightOpenOffset;
        leftOpenPosition = leftClosedPosition + leftOpenOffset;
    }

    void Update()
    {
        // Toggle door open/close with E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        // Move doors toward their target positions
        Vector3 rightTarget = isOpen ? rightOpenPosition : rightClosedPosition;
        Vector3 leftTarget = isOpen ? leftOpenPosition : leftClosedPosition;

        door_2_right.localPosition = Vector3.Lerp(door_2_right.localPosition, rightTarget, Time.deltaTime * openSpeed);
        door_2_left.localPosition = Vector3.Lerp(door_2_left.localPosition, leftTarget, Time.deltaTime * openSpeed);
    }
}
