using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public GameObject cameraToToggle;
    public GameObject eyes;

    [SerializeField] private string[] interactableTags;
    [SerializeField] private float interactLength = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view
        //Cursor.lockState= CursorLockMode.None;
        //Cursor.visible= true;
        cameraToToggle = GameObject.FindWithTag("Camera");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            bool hide_eyes = eyes.activeSelf;
            eyes.SetActive(!hide_eyes);


            bool active = cameraToToggle.activeSelf;

            cameraToToggle.SetActive(!active);

            Debug.Log($"Camera is active: '{cameraToToggle.activeSelf}'.");
        }
        // Raycast to interact with elements of the scene
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            RaycastHit[] hits= Physics.RaycastAll(transform.position, transform.forward, interactLength);
            if(hits.Length>0 ){
                foreach(RaycastHit hit in hits){
                    Debug.Log("Clicked on 1: "+ hit.collider.name);
                    foreach(string tag in interactableTags){
                        if(hit.transform.CompareTag(tag)){
                            Debug.Log("Clicked on 2: "+ hit.collider.name);
                            hit.transform.SendMessage("OnPlayerInteract");
                            break;
                        }
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
