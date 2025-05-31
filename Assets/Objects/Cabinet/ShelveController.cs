using UnityEngine;
using System.Collections;

public class ShelveController : MonoBehaviour
{
    public GameObject content;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //add plant to module
        initPlant();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("On enter Drawer colider:" + other.name);
        Debug.Log("is tag pantr:" + other.CompareTag("Plant"));

        if (other.CompareTag("Plant"))
        {
            Debug.Log("On enter Drawer colider:" + other.name);
            content = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plant"))
        {
            Debug.Log("On enter Drawer colider:" + other.name);
            content = null;
        }
    }

    public void getPlant()
    {
        Debug.Log("GetPlany before content");
        if (content != null)
        {
            Debug.Log("GetPlany");
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PickUpPlant>().grabFromCabinet(content);
        }
    }

    public void initPlant()
    {
        // TODO
    }
}
