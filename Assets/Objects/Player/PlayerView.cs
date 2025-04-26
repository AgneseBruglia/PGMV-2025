using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public float mouse_sensitivity_x = 200f;
    public float mouse_sensitivity_y = 200f;
    public Transform player_body;

    private float x_rotate = 0f;

    [SerializeField] private string[] interactableTags;
    [SerializeField] private float interactLength = 3f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the center of the view
        Cursor.lockState= CursorLockMode.None;
        Cursor.visible= true;

    }

    // Update is called once per frame
    void Update()
    {
        //get mouse input x,y axis, control mouse sensitivity, control frame rate
        float x_input = Input.GetAxis("Mouse X") * mouse_sensitivity_x * 2 * Time.deltaTime;
        float y_input = Input.GetAxis("Mouse Y") * mouse_sensitivity_y * 2 * Time.deltaTime;

        // rotate XX
        x_rotate -= y_input;
        x_rotate = Mathf.Clamp(x_rotate, -90f, 90f); // restric 90�


        transform.localRotation = Quaternion.Euler(x_rotate, 0f, 0f);

        player_body.Rotate(Vector3.up * x_input); // rotate YY

        // Raycast to interact with elements of the scene
        if(Input.GetKeyDown(KeyCode.E)) 
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
            /* Ray ray= new Ray(transform.position, transform.forward);
             hit;
            if(Physics.Raycast(ray, out hit, 5f))
            {
                Debug.Log("Clicked on: "+ hit.collider.name); // log to check if it works

                //if the object has an Interact method it gets called
                var interactable= hit.collider.GetComponent<IInteractable>();
                if(interactable!= null){
                    interactable.Interact();s
                }
            } */
        }
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color= Color.blue;
        Gizmos.DrawLine(transform.position, transform.position+transform.forward*interactLength);
    }

}
