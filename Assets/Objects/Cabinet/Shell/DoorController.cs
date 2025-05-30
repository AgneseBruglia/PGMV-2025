using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;                            // Riferimento alla porta
    public Vector3 openOffset; // Spostamento della porta per l'apertura - can't be fixed position
    public float speed = 2f;                          // Velocità movimento porta

    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private Vector3 openPosition;
    private bool isOpen = false;
    private bool isMoving = false;
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
            //door.position = Vector3.MoveTowards(door.position, target, speed * Time.deltaTime);
            door.localPosition = Vector3.MoveTowards(door.localPosition, target, speed * Time.deltaTime);


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
            playerInRange= true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name=="collider"){
            playerInRange= false;
        }
    }
    
}
