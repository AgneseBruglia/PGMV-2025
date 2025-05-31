using UnityEngine;
using System.Collections;


public class DoorController : MonoBehaviour
{
    public Transform door;     // Riferimento alla porta
    public Vector3 openOffset; // Spostamento della porta per l'apertura - can't be fixed position
    public float speed = 5f;   // Velocità movimento porta

    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool is_moving = false;
    private bool playerInRange = false;

    void Start()
    {
        if (door != null)
        {
            openOffset       = new Vector3(0, 0, transform.localScale.z);
            originalPosition = door.localPosition;
            openPosition     = originalPosition + openOffset;
        }
        else
        {
            Debug.LogError("Door non assegnata nel DoorController!");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && door != null)
        {
            isOpen = !isOpen;
            Debug.Log("playerInRange pressed E");
            if (!isOpen)
            {
                Transform parent = transform.parent; //Important to be the parent (this script needs to be in a first child)
                if (parent != null)
                {
                    foreach (Transform child in parent)
                    {
                        // Close all doors inside the cabinet
                        if (child.CompareTag("Cubicle") || child.CompareTag("Drawer"))
                        {
                            child.gameObject.SendMessage("OnCabinetDoorClose", SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
            Debug.Log("playerInRange after close other doors");
            is_moving = true;

            // Disattiva collider se la porta si apre
            Collider doorCollider = door.GetComponent<Collider>();
            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen;
            }

            

            Vector3 target = isOpen ? openPosition : originalPosition;
            Debug.Log("Start routine");
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("is playerInRange=true; enter");
        if(other.name=="collider"){
            Debug.Log("yes");
            playerInRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("is playerInRange=true; exit");
        if (other.name=="collider"){
            Debug.Log("yes");
            playerInRange = false;
        }
    }

    
    IEnumerator moveDoor(Transform door, Vector3 target, float speed, bool isOpen)
    {
        Debug.Log("in moove door");
        if (!isOpen)
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();

        //yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

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
