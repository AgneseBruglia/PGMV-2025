using UnityEngine;

public class DrawersControilleer : MonoBehaviour
{
    public Transform drawer; // Reference 
    
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
        close_pos = new Vector3(drawer.position.x, drawer.position.y, drawer.position.z);
        open_pos = new Vector3(drawer.position.x + 0.781f, drawer.position.y, drawer.position.z);
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
            drawer.position = Vector3.Lerp(drawer.position, open_pos, Time.deltaTime * speed); // Linearly interpolates between two points.
        }
        else
        {
            drawer.position = Vector3.Lerp(drawer.position, close_pos, Time.deltaTime * speed); //Linearly interpolates between two points.
        }
    }
}
