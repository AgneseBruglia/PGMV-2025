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
        open = false;
        speed = 2.0f;
        close_pos = new Vector3(drawer.position.x, drawer.position.y, drawer.position.z);
        open_pos = new Vector3(drawer.position.x + 1f, drawer.position.y, drawer.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = open ? open_pos : close_pos;
        drawer.position = Vector3.Lerp(drawer.position, targetPos, Time.deltaTime * speed);
    }

    public void OnPlayerInteract(){
        Debug.Log("Drawer clicked!");
        open = !open;
    }
}
