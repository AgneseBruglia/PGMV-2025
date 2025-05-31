using UnityEngine;
using System.Collections;

public class CubicleController : MonoBehaviour
{
    public GameObject content;

    public Transform door;         // Cubicle door
    public float speed = 10f;      // Rotation speed
    public float openAngle = 130f;

    private bool open = false;             // Desired state
    private Quaternion open_rotation;       // Target open rotation
    private Quaternion close_rotation;      // Target closed rotation

    private bool is_moving = false;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door is not assigned on: " + gameObject.name);
            return;
        }

        // Save closed rotation
        close_rotation = door.localRotation;

        // Define open rotation as Y-axis rotation from closed position
        open_rotation = close_rotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
 
    }
    
    public void OnPlayerInteract(){
        Debug.Log("Cubicle clicked!");
        open = !open;
        OnCubicleDoorOpenClose();
    }

    void OnTriggerEnter(Collider other)
    {
        if (open && other.CompareTag("Plant"))
        {
            content = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (open && other.CompareTag("Plant"))
        {
            Debug.Log("On enter cubicle colider:" + other.name);
            content = null;
        }
    }

    void OnCabinetDoorClose() 
    {
        open = false;
        OnCubicleDoorOpenClose();
    }

    void OnCubicleDoorOpenClose() 
    {
        is_moving = true;

        Quaternion target_rotation = open ? open_rotation : close_rotation;
        StartCoroutine(
            moveDoor(
                door,
                target_rotation,
                speed
            )
        );

        if (content != null && open && !content.activeInHierarchy)
        {
            content.SetActive(true);
        }
        else if (content != null && !open && content.activeInHierarchy)
        {
            content.SetActive(false);
        }
    }

    public void getPlant() 
    {
        Debug.Log("GetPlany before content");
        if (open && content != null) 
        {
            Debug.Log("GetPlany");
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PickUpPlant>().grabFromCabinet(content);
        }
    }

    public void addPlant()
    {
        // TODO
    }

    IEnumerator moveDoor(Transform door, Quaternion target, float speed)
    {
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if (is_moving == true) 
        {
            audio.loop = false;
            audio.Play();
        }

        while (is_moving)
        {
            // Move the door toward the target position
            door.localRotation = Quaternion.Slerp(door.localRotation, target, Time.deltaTime * speed);

            // Check if the door has reached the target
            if (door.localRotation == target)
            {
                is_moving = false;

                audio.loop = false;
                audio.Stop();
            }
            yield return null; // Wait for the next frame
        }
    }

}
