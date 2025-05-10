using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public GameObject cameraToToggle;
    public GameObject player;
    public GameObject eyes;

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
            Debug.Log("Hit: " + hit.collider.name + " with tag: " + hit.collider.tag);

            if (hit.transform.CompareTag("Plant"))
            {
                player.GetComponent<PickUpPlant>().grab(hit);
                //hit.transform.SendMessage("grab", hit, SendMessageOptions.DontRequireReceiver);
                return;
            }
            foreach (string tag in interactableTags)
            {
                if (hit.transform.CompareTag(tag))
                {
                    hit.transform.SendMessage("OnPlayerInteract", SendMessageOptions.DontRequireReceiver);
                    break;
                }
            }
        }
    }
}


    private void OnDrawGizmosSelected(){
        Gizmos.color= Color.blue;
        Gizmos.DrawLine(transform.position, transform.position+transform.forward*interactLength);
    }

}