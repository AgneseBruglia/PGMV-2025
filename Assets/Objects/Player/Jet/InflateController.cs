using UnityEngine;

public class InflateController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetComponent<Renderer>().material.SetFloat("_Inflate", 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        RaycastHit hit_info = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit_info);

        if (hit && hit_info.transform.gameObject.tag == "jet_material")
        {
            transform.GetComponent<Renderer>().material.SetFloat("_Inflate", 0.2f);

        }
    }
}
