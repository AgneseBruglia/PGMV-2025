using UnityEngine;

public class PickUpPlant : MonoBehaviour
{
    private GameObject grabbed_obj;
    private Rigidbody grabbed_rigidbody;
    public Transform hold_position;
    public GameObject player;
    private bool release; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        release = true; // 
    }

    // Update is called once per frame
    void Update()
    {
        // If player is grabbing the robot
        
        if (grabbed_obj != null)
        {
            // if mouse button up (released), then drop it
            if (Input.GetMouseButtonUp(0))
            {
                drop();
            }

            // if mouse is down keep grabing it in position
            grabbed_obj.transform.position = hold_position.position; // hold position;
        }

        // Drop plant
        if (grabbed_obj != null && release == true)
        {
            // Return arms to main pose
            player.GetComponent<ArmGenerator>().main_pose();

            // Put physics back
            grabbed_rigidbody.useGravity = true;
            grabbed_rigidbody.constraints = RigidbodyConstraints.None;

            // release instances of grabbed plants
            grabbed_obj = null;
            grabbed_rigidbody = null;
        }
    }

    public void grab(RaycastHit hit)
    {
        // if hit object has rigidbody
        if (hit.collider.gameObject.GetComponent<Rigidbody>())
        {
            // reference to grabbed object
            grabbed_obj = hit.collider.gameObject;

            //Remove physics
            grabbed_rigidbody = grabbed_obj.GetComponent<Rigidbody>();
            grabbed_rigidbody.useGravity = false;
            grabbed_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // Set arms in position
            player.GetComponent<ArmGenerator>().grab_pose();

            release = false; // flag it
        }
    }

    public void grabFromCabinet(GameObject content)
    {
        // Cabinets have coliders that can be in front of the plant colider
        grabbed_obj = content;

        // Remove physics
        grabbed_rigidbody = grabbed_obj.GetComponent<Rigidbody>();
        grabbed_rigidbody.useGravity = false;
        grabbed_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        player.GetComponent<ArmGenerator>().grab_pose();

        release = false;
    }

    void drop()
    {
        // flag to drop in update
        release = true; 
    }

}