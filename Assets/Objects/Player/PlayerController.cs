using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float max_speed = 10f;
    public float jump_force = 0.5f;
    public float fall_force = 4f;
    private Rigidbody rb;
	
    [SerializeField] GameObject jet_bottom;
    [SerializeField] GameObject jet_back_left;
    [SerializeField] GameObject jet_back_right;
    [SerializeField] GameObject jets;

    public float mouse_sensitivity_x = 200f;
    public float mouse_sensitivity_y = 200f;

    private float x_rotate = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view

        rb = GetComponent<Rigidbody>();
        // avoid gliding
        rb.linearDamping = 0; // config for falling , drag

        // get jets object
        jet_bottom = GameObject.FindWithTag("JET_BOTTOM");
        jet_back_left = GameObject.FindWithTag("JET_BACK_LEFT");
        jet_back_right = GameObject.FindWithTag("JET_BACK_RIGHT");
        jets = GameObject.FindWithTag("JETS");

        jet_bottom.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); 
        jet_back_left.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f); 
        jet_back_right.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f); 
    }

    void Update()
    {
        //Apply continuos force based on input params (Input.GetAxis("Vertical"), W,S keys - Defined in settings, //Input.GetAxis("Horizontal") , A, D keys - Defined in settings)
        rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
        rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);
        //get mouse input x,y axis, control mouse sensitivity, control frame rate
        float x_input = Input.GetAxis("Mouse X") * mouse_sensitivity_x * 5 * Time.deltaTime;
        float y_input = Input.GetAxis("Mouse Y") * mouse_sensitivity_y * 2 * Time.deltaTime;
        // rotate XX
        x_rotate -= y_input; // Invert controll so that when mouse up it looks up
        x_rotate = Mathf.Clamp(x_rotate, -45f, 60f); // restrict minimum -45 and maximum 60f
        //apply change players rotation (cameras are rotating to)
        transform.localRotation = Quaternion.Euler(x_rotate, transform.localRotation.eulerAngles.y, 0f);
        transform.Rotate(Vector3.up * x_input); // rotate YY axis (horizontal)
        //Limit velocity
        if (rb.linearVelocity.magnitude > max_speed)
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, max_speed);

        jetForwardEffects();
        jump();

        if (transform.position.y < -5) { transform.position = Vector3.zero; } //Fix position if falls belloiw ground
    }

    void jetForwardEffects()
    {
        //Apply jet effects
        if (Input.GetAxis("Vertical") <= 0) // stopped or backwards
        {
            //Debug.Log("(" + Input.GetAxis("Horizontal") + ", " + Input.GetAxis("Vertical") + ")");
            jet_back_left.transform.localScale = new Vector3(0, 0, 0);
            jet_back_right.transform.localScale = new Vector3(0, 0, 0);
            jets.GetComponent<AudioSource>().Stop();
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
                jets.GetComponent<AudioSource>().Play();

            }

            // full size = new Vector3(0.3f, 0.3f, 0.3f) (max)
            jet_back_left.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // if negative right engine
            jet_back_right.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    void jump()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 50) // give a initial boost
        {
            //AudioSource effects
            jets.GetComponent<AudioSource>().Play(); // set sound
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * jump_force * 9.81f * rb.mass, ForceMode.Impulse);

            // jet Effects
            jet_bottom.transform.localScale = new Vector3(0.3f, 0.9f, 0.3f);
        }
        else if (Input.GetKey(KeyCode.Space)) // In Air Accelaration
        {

            jets.GetComponent<AudioSource>().Play();
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.up * speed * 9.81f * rb.mass, ForceMode.Acceleration);

            // jet Effects
            jet_bottom.transform.localScale = new Vector3(0.3f, 0.6f, 0.3f);
        }
        else if (Input.GetKeyUp(KeyCode.Space) && transform.position.y < 50) // In Air desaccelaration
        {
            //AudioSource effects
            jets.GetComponent<AudioSource>().Stop();
            //Physics.gravity = new Vector3(0, -9.81f, 0);
            // We need the vector to have move force than the gravity, ForceMode is applied against mass
            rb.AddForce(Vector3.down * fall_force, ForceMode.Impulse);

            // jet Effects
            jet_bottom.transform.localScale = new Vector3(0.3f, 0, 0.3f);
        }
        else
        { // return to main effects

            //AudioSource effects
            jet_bottom.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            //Stop audio
            jets.GetComponent<AudioSource>().Stop();
        }
    }
}