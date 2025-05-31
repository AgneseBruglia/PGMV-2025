using UnityEngine;

public class CabinetDoorController : MonoBehaviour
{
    public Transform door; // Reference 

    public float speed;
    public bool open; //is opened
    public bool active; //action active 

    private Vector3 open_pos;
    private Vector3 close_pos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        active = true;
        open = false;
        speed = 2.0f;
        close_pos = new Vector3(door.position.x, door.position.y, door.position.z);
        open_pos = new Vector3(door.position.x, door.position.y, door.position.z + 0.9f);


        /*
        open_close = false;
        top_open = new Vector3(top_door.transform.position.x, 42, top_door.transform.position.z);
        top_close = new Vector3(top_door.transform.position.x, 30, top_door.transform.position.z);

        bottom_open = new Vector3(bottom_door.transform.position.x, -15, top_door.transform.position.z);
        bottom_close = new Vector3(bottom_door.transform.position.x, 0, top_door.transform.position.z);
        //CloseDoors();
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E)) // "E" to open drowers 
        {
            open = !open;
        }

        if (open)
        {
            door.position = Vector3.Lerp(door.position, open_pos, Time.deltaTime * speed); // Linearly interpolates between two points.
        }
        else
        {
            door.position = Vector3.Lerp(door.position, close_pos, Time.deltaTime * speed); //Linearly interpolates between two points.
        }
    }

    /*

    public bool open_close;
    public GameObject top_door;
    public GameObject bottom_door;
    public float speed = 1f;

    public Vector3 top_open;
    public Vector3 top_close;
    public Vector3 bottom_open;
    public Vector3 bottom_close;

    */
    public void OpenDoors()
    {
        /*
        open_close = true;
        top_door.transform.position = Vector3.Lerp(top_close, top_open, Time.deltaTime * speed);
        bottom_door.transform.position = Vector3.Lerp(bottom_close, bottom_open, Time.deltaTime * speed);
        */
    }

    public void CloseDoors()
    {
        /*
        open_close = false;
        top_door.transform.position = Vector3.Lerp(top_open, top_close, Time.deltaTime * speed);
        bottom_door.transform.position = Vector3.Lerp(bottom_open, bottom_close, Time.deltaTime * speed);
        */
    }

    public void OnPlayerInteract()
    {
        open = !open;
    }

}
