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
        release = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed_obj != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                drop();
            }

            grabbed_obj.transform.position = hold_position.position; // hold position;
        }

        if (grabbed_obj != null && release == true)
        {
            player.GetComponent<ArmGenerator>().main_pose();
            grabbed_rigidbody.useGravity = true;
            grabbed_rigidbody.constraints = RigidbodyConstraints.None;
            grabbed_obj = null;
            grabbed_rigidbody = null;
            Debug.Log("released:");
        }
    }

    public void grab(RaycastHit hit)
    {
        Debug.Log("GRAB: " + hit.collider.name);
        if (hit.collider.gameObject.GetComponent<Rigidbody>())
        {
            grabbed_obj = hit.collider.gameObject;
            grabbed_rigidbody = grabbed_obj.GetComponent<Rigidbody>();
            grabbed_rigidbody.useGravity = false;
            grabbed_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            player.GetComponent<ArmGenerator>().grab_pose();

            release = false;

        }
    }

    void drop()
    {
        release = true;
    }

}