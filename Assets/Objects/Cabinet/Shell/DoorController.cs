using UnityEngine;
using System.Collections;


public class DoorController : MonoBehaviour
{
    public Transform door;     // Riferimento alla porta
    public Vector3 openOffset; // Spostamento della porta per l'apertura - can't be fixed position
    public float speed = 5f;   // Velocità movimento porta

    private Vector3 originalPosition; // (close position)
    private Vector3 openPosition;
    private bool isOpen = false; // flag if it's open
    private bool is_moving = false; // flag to check if door is moving
    private bool playerInRange = false; // Verify if it is in close range

    void Start()
    {
        // If door, sets the open/close positions
        if (door != null)
        {
            openOffset       = new Vector3(0, 0, transform.localScale.z);
            originalPosition = door.localPosition;
            openPosition     = originalPosition + openOffset;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && door != null)
        {
            isOpen = !isOpen;

            // Close all doors inside the cabinet
            // if we want to close the cabinet
            if (!isOpen)
            {
                Transform parent = transform.parent; //Important to be the parent (this script needs to be in a first child)
                if (parent != null)
                {
                    foreach (Transform child in parent)
                    {
                        if (child.CompareTag("Cubicle") || child.CompareTag("Drawer"))
                        {
                            child.gameObject.SendMessage("OnCabinetDoorClose", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }

            is_moving = true;

            // Disable collider if door opens
            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen;
            }

            // Togles door to open or close
            Vector3 target = isOpen ? openPosition : originalPosition;

            // Start routine to close/open the door
            StartCoroutine(
                moveDoor(
                    door,
                    target,
                    speed,
                    isOpen
                )
            );
        }
    }

    /*
     * If Player its the collider,
     * set flag playerInRange = true
     * so that it is able to open/close the door
     */
    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="collider"){
            playerInRange = true;
        }
        
    }

    /*
     * If Player get's out of the collider,
     * set flag playerInRange = false
     * so that the player is not able to open/close the door if is away
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.name=="collider"){
            playerInRange = false;
        }
    }

    /*
     * Couroutine to move the door
     * Add sound 
     * Applies the translation
     * Waits to be able to close all the doors first
     */
    IEnumerator moveDoor(Transform door, Vector3 target, float speed, bool isOpen)
    {
        // Waits only if we want to close the cabinet
        if (!isOpen)
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // cabinet sounds
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();

        while (is_moving)
        {
            // Move the door toward the target position
            door.localPosition = Vector3.MoveTowards(door.localPosition, target, speed * Time.deltaTime);

            // Check if the door has reached the target
            if (Vector3.Distance(door.localPosition, target) < 0.01f)
            {
                door.localPosition = target;
                is_moving = false;
                
                audio.loop = false;
                audio.Stop();
            }
            yield return null; // Wait for the next frame
        }
    }
}
