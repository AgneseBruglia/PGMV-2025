using UnityEngine;

public class DoorController : MonoBehaviour
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
}
