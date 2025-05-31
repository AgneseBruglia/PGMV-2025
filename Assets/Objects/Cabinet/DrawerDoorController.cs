using UnityEngine;

public class DrawerDoorController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
    }

    public void OnPlayerInteract()
    {
        Debug.Log("DrawerDoorController OnPlayerInteract");
        transform.parent.GetComponent<DrawerController>().OnPlayerInteract();
    }
}
