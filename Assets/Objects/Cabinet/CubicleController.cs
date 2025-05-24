using UnityEngine;

public class CubicleController : MonoBehaviour
{
    public Transform door; // Reference 

    public float speed;
    public bool open; //is opened
    public bool active; //action active 
    public bool is_action; //action active 

    private float open_angle;
    private Quaternion open_rotation;
    private Quaternion close_rotation;
    private float rot_yy;
    private float rot_zz;
    private float time = 0f;

    void Awake() //Or OnEnable()
    {
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        is_action = false;
        active = true;
        open = false;
        speed = 2.0f;
        open_angle = 130f;

        if (door != null)
        {
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


        if (time < speed)
        {
            if (open)
            {
                time += Time.deltaTime * speed;
                door.localRotation = Quaternion.Lerp(close_rotation, open_rotation, time);
            }
            else
            {
                time += Time.deltaTime * speed;
                door.localRotation = Quaternion.Lerp(open_rotation, close_rotation, time);
            }

        }

        if (time >= speed && Input.GetKeyDown(KeyCode.E)) // "E" to open drowers 
        {
            open = !open;

            time = 0;
        }
    }
}
