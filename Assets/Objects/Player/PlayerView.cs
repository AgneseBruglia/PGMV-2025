using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public float mouse_sensitivity_x = 200f;
    public float mouse_sensitivity_y = 200f;
    public Transform player_body;

    private float x_rotate = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view
    }

    // Update is called once per frame
    void Update()
    {
        //get mouse input x,y axis, control mouse sensitivity, control frame rate
        float x_input = Input.GetAxis("Mouse X") * mouse_sensitivity_x * 2 * Time.deltaTime;
        float y_input = Input.GetAxis("Mouse Y") * mouse_sensitivity_y * 2 * Time.deltaTime;

        // rotate XX
        x_rotate -= y_input;

        Debug.Log("x_rotate:" + x_rotate);

        //x_rotate = Mathf.Clamp(x_rotate, -30f, 30f); // restric 90º

        Debug.Log("x_rotate clamped:" + x_rotate);


        transform.localRotation = Quaternion.Euler(x_rotate, 0f, 0f);

        player_body.Rotate(Vector3.up * x_input); // rotate YY
    }
}
