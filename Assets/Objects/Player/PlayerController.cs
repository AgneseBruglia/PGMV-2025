using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float max_speed = 10f;

    public float jump_force = 2f;
    public float fall_force = 2f;
    
    public float turn_rate = 10f;
    [SerializeField] bool use_physics = true;
    private Rigidbody rb;

    public float gravity_force = -10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0; // config for falling , drag
    }

    void Update()
    {
        rb.isKinematic = !use_physics;
        rb.useGravity = use_physics;

        //Move 
        if (!use_physics)
        {
            transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime, Space.Self);
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * turn_rate  * Time.deltaTime, Space.Self);
        }
        else
        {
            Debug.Log("Applying forces to go forward and backweards");
            //rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
            //rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);
            rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
            rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);
        }

        //Limit velocity
        //Debug.Log(rb.linearVelocity.magnitude);
        if (rb.linearVelocity.magnitude > max_speed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, max_speed);
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 50)
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * jump_force * 9.81f * rb.mass, ForceMode.Impulse);
        }
        else if (Input.GetKey(KeyCode.Space)) // In Air Acelaration
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * speed * 9.81f * rb.mass, ForceMode.Acceleration);
        }       
        else if (Input.GetKeyUp(KeyCode.Space) && transform.position.y < 50) // In Air decelaration
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.down * fall_force, ForceMode.Impulse);
        }

        
        if (transform.position.y < -5) //Fix position if falls belloiw ground
        {
            transform.position = Vector3.zero;
        }

    }

}
