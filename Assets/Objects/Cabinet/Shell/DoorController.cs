using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;                            // Riferimento alla porta
    public Vector3 openOffset = new Vector3(0, 0, 2); // Spostamento della porta per l'apertura
    public float speed = 2f;                          // Velocità movimento porta

    private Vector3 originalPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
    private bool playerInRange = false;

    void Start()
    {
        if (door != null)
        {
            originalPosition = door.position;
            openPosition = originalPosition + openOffset;
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
        isMoving = true;

        // Disattiva collider se la porta si apre
        Collider doorCollider = door.GetComponent<Collider>();
        if (doorCollider != null)
        {
            doorCollider.enabled = !isOpen;
        }
    }

    if (isMoving)
    {
        Vector3 target = isOpen ? openPosition : originalPosition;
        door.position = Vector3.MoveTowards(door.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(door.position, target) < 0.01f)
        {
            door.position = target;
            isMoving = false;
        }
    }
}


    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="collider"){
            Debug.Log("ENTER COLLIDER: " + other.name);
            playerInRange= true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name=="collider"){
            Debug.Log("EXITED COLLIDER: " + other.name);
            playerInRange= false;
        }
    }
    
}
