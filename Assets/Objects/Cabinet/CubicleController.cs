using UnityEngine;

public class CubicleController : MonoBehaviour
{
    public Transform door; // Reference 

    public float speed;
    public bool open; //is opened
    public bool active; //action active 

    private float open_angle;
    private Quaternion open_rotation;
    private Quaternion close_rotation;
    private float rot_yy;
    private float rot_zz;

    void Awake() //Or OnEnable()
    {
        if (door != null)
        {
            // Access transform variables here before Start()
            Debug.Log("AWAKE Intanttiated id(" + transform.GetInstanceID() + ")");
        }
        else
        {
            // Access transform variables here before Start()
            Debug.Log("AWAKE not Intanttiated id(" + transform.GetInstanceID() + ")");
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start method executed");

        active = true;
        open = false;
        speed = 2.0f;
        open_angle = 130f;

        if (door != null)
        {
            Debug.Log("YUPI door: id(" + transform.GetInstanceID() + ")");

            open_rotation = Quaternion.Euler(new Vector3(door.rotation.x, door.rotation.y + open_angle, door.rotation.z));
            close_rotation = door.rotation;           
        }
        else {
            Debug.Log("NUll reference: id(" + transform.GetInstanceID() + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (door == null)
        {
            //Debug.LogError("Door is not assigned");
            //Debug.Log("Door is not assigned");
            return;
        }

            if (!active)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E)) // "E" to open drowers 
        {
            open = !open;

            if (open)
            {
                // Dampen towards the target rotation
                door.rotation = Quaternion.Slerp(door.rotation, open_rotation, Time.deltaTime * speed);
            }
            else
            {
                // Dampen towards the target rotation
                door.rotation = Quaternion.Slerp(door.rotation, close_rotation, Time.deltaTime * speed);
            }
        }
    }
}
