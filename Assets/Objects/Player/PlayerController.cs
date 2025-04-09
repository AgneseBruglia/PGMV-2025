using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float speed = 2f;
    public float jump_force = 5f;
    public float turn_rate = 10f;
    [SerializeField] bool use_physics = true;
    private Rigidbody rb;
    private bool isGrounded;
    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.isKinematic = !use_physics;
        rb.useGravity = use_physics;

        //Move 
        if (!use_physics)
        {
            transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime, Space.Self);
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * turn_rate * Time.deltaTime, Space.Self);
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Force);
            rb.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * speed, ForceMode.Force);
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }

        //Fix position
        if (transform.position.y < -5)
        {
            transform.position = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

}
