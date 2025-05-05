using UnityEngine;

namespace Door
{
    public class DoorController : MonoBehaviour
    {
        public float speed;
        public bool open; //is opened
        public bool active; //action active 

        private Vector3 open_pos;
        private Vector3 close_pos;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            active = true;
            open = false;
            speed = 2.0f;
            close_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            open_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2.9f);
        }

        // Update is called once per frame
        void Update()
        {
            if (!active)
            {
                return;
            }


            if (open)
            {
                transform.position = Vector3.Lerp(transform.position, open_pos, Time.deltaTime * speed); // Linearly interpolates between two points.
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, close_pos, Time.deltaTime * speed); //Linearly interpolates between two points.
            }
        }

        public void OnPlayerInteract()
        {
            Debug.Log("OnPlayerInteract chiamato per: " + gameObject.name);
            open = !open;
        }
    }

}

