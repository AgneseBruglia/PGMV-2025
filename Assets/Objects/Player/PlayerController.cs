using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float max_speed = 10f;

    public float jump_force = 0.5f;
    public float fall_force = 4f;
    
    public float turn_rate = 5f;
    [SerializeField] bool use_physics = true;
    private Rigidbody rb;
	
   [SerializeField] GameObject jet_bottom;
   [SerializeField] GameObject jet_back_left;
   [SerializeField] GameObject jet_back_right;

    public float gravity_force = -20f;

    public float mouse_sensitivity_x = 200f;
    public float mouse_sensitivity_y = 200f;

    private float x_rotate = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view

        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0; // config for falling , drag
	
        jet_bottom     = GameObject.FindWithTag("JET_BOTTOM");
        jet_back_left  = GameObject.FindWithTag("JET_BACK_LEFT");
        jet_back_right = GameObject.FindWithTag("JET_BACK_RIGHT");

        jet_bottom.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
        jet_back_left.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f); 
        jet_back_right.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f); 
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
            //Debug.Log("Applying forces to go forward and backweards");
            //rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
            //rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);
            rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
            rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);

            //get mouse input x,y axis, control mouse sensitivity, control frame rate
            float x_input = Input.GetAxis("Mouse X") * mouse_sensitivity_x * 10 * Time.deltaTime;
            float y_input = Input.GetAxis("Mouse Y") * mouse_sensitivity_y * 2 * Time.deltaTime;
            
            //Debug.Log("mouse x:" + x_input);
            //Debug.Log("mouse y:" + y_input);
            
            // rotate XX
            x_rotate -= y_input;
            x_rotate = Mathf.Clamp(x_rotate, -30f, 30f); // restric 90�

            transform.localRotation = Quaternion.Euler(x_rotate, transform.localRotation.eulerAngles.y, 0f); 
            transform.Rotate(Vector3.up * x_input); // rotate YY
        }

        //Limit velocity
        //Debug.Log(rb.linearVelocity.magnitude);
        if (rb.linearVelocity.magnitude > max_speed)
        {
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, max_speed);
        }
		
		
        //jets
        if (Input.GetAxis("Vertical") <= 0) // stopped or backwards
        {
            //Debug.Log("(" + Input.GetAxis("Horizontal") + ", " + Input.GetAxis("Vertical") + ")");
            jet_back_left.transform.localScale = new Vector3(0, 0, 0);
            jet_back_right.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            //Debug.Log("(" + Input.GetAxis("Horizontal") + ", " + Input.GetAxis("Vertical") + ")");
            //[0.0, 0.3][-1, 1]
            if (Input.GetAxis("Horizontal") > 0) // turn right
            {
                jet_back_left.transform.localScale = new Vector3(0.3f, Input.GetAxis("Vertical") * 0.3f, 0.3f);
                jet_back_right.transform.localScale = new Vector3(0.3f, 0.0f, 0.3f);
            }
            else if (Input.GetAxis("Horizontal") < 0) // turn left
            {
                jet_back_left.transform.localScale = new Vector3(0.3f, 0.0f, 0.3f);
                jet_back_right.transform.localScale = new Vector3(0.3f, Input.GetAxis("Vertical") * 0.3f, 0.3f);
            }
            else //forward
            {
                jet_back_left.transform.localScale = new Vector3(0.3f, Input.GetAxis("Vertical") * 0.3f, 0.3f);
                jet_back_right.transform.localScale = new Vector3(0.3f, Input.GetAxis("Vertical") * 0.3f, 0.3f);
            }

            // full size = new Vector3(0.3f, 0.3f, 0.3f) (max)
            jet_back_left.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // if negative right engine
            jet_back_right.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
		

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 50)
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * jump_force * 9.81f * rb.mass, ForceMode.Impulse);
			jet_bottom.transform.localScale = new Vector3(0.3f, 0.9f, 0.3f);
        }
        else if (Input.GetKey(KeyCode.Space)) // In Air Acelaration
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * speed * 9.81f * rb.mass, ForceMode.Acceleration);
			jet_bottom.transform.localScale = new Vector3(0.3f, 0.6f, 0.3f);
        }       
        else if (Input.GetKeyUp(KeyCode.Space) && transform.position.y < 50) // In Air decelaration
        {
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.down * fall_force, ForceMode.Impulse);
			jet_bottom.transform.localScale = new Vector3(0.3f, 0, 0.3f);
        }
        else 
        {
            jet_bottom.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        
        if (transform.position.y < -5) //Fix position if falls belloiw ground
        {
            transform.position = Vector3.zero;
        }

    }

}
