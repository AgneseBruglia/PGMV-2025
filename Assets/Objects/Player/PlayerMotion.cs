using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 10f;
    
    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Direction to move
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime); // move, speed, framerate
    }
}
