using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public GameObject cameraToToggle;
    public GameObject player;
    public GameObject eyes;
    public GameObject plantInteraction;

    [SerializeField] private string[] interactableTags;
    [SerializeField] private float interactLength = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view
        //Cursor.lockState= CursorLockMode.None;
        Cursor.visible= true;
        cameraToToggle = GameObject.FindWithTag("Camera");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            eyes.SetActive(!eyes.activeSelf); // Toggle eyes visibility
            cameraToToggle.SetActive(!cameraToToggle.activeSelf); // Toggle camera
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Lancia un raycast dalla camera/occhi nella direzione in cui guarda
            Ray ray = new Ray(eyes.transform.position, eyes.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactLength))
            {
                Debug.Log(" interactableTags: " + string.Join(", ", interactableTags));
                Debug.Log("Hit: " + hit.collider.name + " with tag: " + hit.collider.tag);
                Debug.Log("hit.transform.CompareTag(\"DraerDor\"):" + hit.transform.CompareTag("DrawerDoor"));

                if (hit.transform.CompareTag("Cubicle"))
                {
                    // Grabs the plant and shows the settings interface
                    hit.transform.gameObject.GetComponent<CubicleController>().getPlant();
                    return;
                }

                if (hit.transform.CompareTag("Drawer"))
                {
                    // Grabs the plant and shows the settings interface
                    hit.transform.gameObject.GetComponent<DrawerController>().getPlant();
                    return;
                }

                if (hit.transform.CompareTag("Shelve"))
                {
                    // Grabs the plant and shows the settings interface
                    hit.transform.gameObject.GetComponent<ShelveController>().getPlant();
                    return;
                }

                if (hit.transform.CompareTag("Plant") || hit.collider.name == "Plant")
                {
                    // Grabs the plant and shows the settings interface
                    player.GetComponent<PickUpPlant>().grab(hit);
                    return;
                }

                foreach (string tag in interactableTags)
                {
                    if (hit.transform.CompareTag(tag))
                    {
                        Debug.Log("before send message : " + hit.collider.tag);
                        hit.transform.SendMessage("OnPlayerInteract", SendMessageOptions.DontRequireReceiver);
                        break;
                    }
                }
            }
        }
        
        // Right click on the mouse to open the plant setting interface
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = new Ray(eyes.transform.position, eyes.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactLength))
            {
                if (hit.transform.CompareTag("Plant") || hit.collider.name == "Plant" || hit.collider.name == "PrefabPickupPlant")
                {
                    // Opens the interface
                    plantInteraction.GetComponent<InteractionController>().OpenInterface(hit.collider.gameObject);
                }
            }
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color= Color.blue;
        Gizmos.DrawLine(transform.position, transform.position+transform.forward*interactLength);
    }

}