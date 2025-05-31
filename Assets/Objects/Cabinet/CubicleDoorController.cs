using UnityEngine;

public class CubicleDoorController : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
    }

    public void OnPlayerInteract()
    {
        transform.parent.GetComponent<CubicleController>().OnPlayerInteract();
    }
}
