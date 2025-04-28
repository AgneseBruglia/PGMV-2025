using UnityEngine;

public class CabinetSwitchController : MonoBehaviour
{
    public GameObject ObjectWhereActionIsTriggered;
    public GameObject SwitchTriggerAnimationObject;
    public GameObject InstructionR;
    public bool onoff;
    public float nob_speed = 5f;
    private bool active;

    private Vector3 on_pos;
    private Vector3 off_pos;

    void Awake()
    {
        Physics.IgnoreCollision(transform.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onoff = false;
        active = false;

        on_pos = new Vector3(SwitchTriggerAnimationObject.transform.position.x, 3, SwitchTriggerAnimationObject.transform.position.z);
        off_pos = new Vector3(SwitchTriggerAnimationObject.transform.position.x, 3.5f, SwitchTriggerAnimationObject.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (active==true &&  Input.GetKeyDown(KeyCode.R))
        {
            //myObject.GetComponent<MyScript>().MyFunction();


            onoff = !onoff;
            //do the swith animation

            if (onoff == true)
            {
                SwitchTriggerAnimationObject.transform.position = Vector3.Lerp(off_pos, on_pos, Time.deltaTime * nob_speed);
                ObjectWhereActionIsTriggered.GetComponent<CabinetDoorController>().OpenDoors();
            }
            else
            {
                SwitchTriggerAnimationObject.transform.position = Vector3.Lerp(on_pos, off_pos, Time.deltaTime * nob_speed);
                ObjectWhereActionIsTriggered.GetComponent<CabinetDoorController>().CloseDoors();
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Some info about you can press a R to on/off
            Debug.Log("On Enter door colider");
            InstructionR.SetActive(true);
            active = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Remove the info you can press E to on/off
            Debug.Log("On Exit door colider");
            InstructionR.SetActive(false);
            active = false;
        }
    }
}
