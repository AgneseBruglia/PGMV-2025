using UnityEngine;
using System.Collections;

public class DrawerController : MonoBehaviour
{
    public GameObject content;

    public Transform door; // drawer Reference 

    public float speed = 10f;
    public bool open = false; //is opened
    public bool active; //action active

    private Vector3 open_position;
    private Vector3 close_position;

    private bool is_moving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door is not assigned on: " + gameObject.name);
            return;
        }

        Vector3 open_offset = new Vector3(transform.localScale.x, 0, 0);
        close_position = door.localPosition;
        open_position = close_position + open_offset;

        GetComponent<Collider>().enabled = open;
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

    public void initPlant(GameObject plant)
    {
        content = plant;
        content.SetActive(false);
    }

    public void OnPlayerInteract()
    {

        open = !open;
        OnDrawerDoorOpenClose();
    }

    void OnTriggerEnter(Collider other)
    {
        if (open && other.CompareTag("Plant"))
        {
            Debug.Log("On enter Drawer colider:" + other.name);
            content = other.gameObject;
        }
    }

    //remove the reference to contents
    void OnTriggerExit(Collider other)
    {
        if (open && other.CompareTag("Plant"))
        {
            Debug.Log("On enter Drawer colider:" + other.name);
            content = null;
        }
    }

    // Flags closed and do couroutione to close
    void OnCabinetDoorClose()
    {
        open = false;
        OnDrawerDoorOpenClose();
    }

    void OnDrawerDoorOpenClose()
    {
        is_moving = true;

        if (content != null && open && !content.activeInHierarchy)
        {
            content.SetActive(true);
        }
        else if (content != null && !open && content.activeInHierarchy)
        {
            content.SetActive(false);
        }

        GetComponent<Collider>().enabled = open;


        Vector3 target_position = open ? open_position : close_position;
        StartCoroutine(
            moveDoor(
                door,
                target_position,
                speed
            )
        );
    }

    IEnumerator moveDoor(Transform door, Vector3 target, float speed)
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
            door.localPosition = Vector3.MoveTowards(door.localPosition, target, Time.deltaTime * speed);

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
