using UnityEngine;

public class LiquidSoundController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("On Enter door colider");
        if (other.CompareTag("Player"))
        {
            // Some info about you can press a R to on/off
            Debug.Log("On Enter door colider");
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GetComponent<AudioSource>().Stop();
        Debug.Log("On Exit door colider");
        if (other.CompareTag("Player"))
        {
            // Remove the info you can press E to on/off
            Debug.Log("On Exit door colider");
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }
}
