using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawerController : MonoBehaviour
{
    public Transform drawer; // Referência à gaveta
    public Vector3 open_pos; // Opened position
    public Vector3 closed_pos; //Closed position
    public float speed; //Velocity

    public bool open; //Is Open
    public bool active = false;

    void Start()
    {
        open = false;
        speed = 2.0f;
        open_pos = new Vector3(0, 0, 0.857f);
        closed_pos = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Tecla para abrir/fechar
        {
            open = !open;
        }

        if (open)
        {
            drawer.position = Vector3.Lerp(drawer.position, open_pos, Time.deltaTime * speed);
        }
        else
        {
            drawer.position = Vector3.Lerp(drawer.position, closed_pos, Time.deltaTime * speed);
        }
    }
}