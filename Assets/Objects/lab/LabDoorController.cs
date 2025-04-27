using UnityEngine;

public class LabDoorController : MonoBehaviour
{
    public bool open_close;
    public GameObject top_door;
    public GameObject bottom_door;
    public float speed = 1f;

    public Vector3 top_open;
    public Vector3 top_close;
    public Vector3 bottom_open;
    public Vector3 bottom_close;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        open_close = false;
        top_open = new Vector3(top_door.transform.position.x, 42, top_door.transform.position.z);
        top_close = new Vector3(top_door.transform.position.x, 30, top_door.transform.position.z);

        bottom_open = new Vector3(bottom_door.transform.position.x, -15, top_door.transform.position.z);
        bottom_close = new Vector3(bottom_door.transform.position.x, 0, top_door.transform.position.z);
        //CloseDoors();
    }

    public void OpenDoors()
    {
        open_close = true;
        top_door.transform.position = Vector3.Lerp(top_close, top_open, Time.deltaTime * speed);
        bottom_door.transform.position = Vector3.Lerp(bottom_close, bottom_open, Time.deltaTime * speed);
    }

    public void CloseDoors()
    {
        open_close = false;
        top_door.transform.position = Vector3.Lerp(top_open, top_close, Time.deltaTime * speed);
        bottom_door.transform.position = Vector3.Lerp(bottom_open, bottom_close, Time.deltaTime * speed);
    }
}
