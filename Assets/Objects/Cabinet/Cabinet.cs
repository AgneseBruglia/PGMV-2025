using UnityEngine;
using System.IO;
using System.Xml;

public class Cabinet : MonoBehaviour
{
    public TextAsset xml_asset;

    public GameObject prefab_p; // prefab_p
    public GameObject prefab_c; // prefabC
    public GameObject prefab_g; // prefabG
    public GameObject shell_corner; // Cabinet part 1,3,7,9
    public GameObject shell_middle; // Cabinet part 2, 4, 6, 8
    public GameObject shell_center; // Cabinet part 5
    public GameObject prefab_door_part; // door child
    private GameObject door; // door parent
    private Light cabinetLight; // Reference to the light component

    private Vector3 minPosition;
    private Vector3 maxPosition;

    /*
     * Initializes before appication starts
     */
    private void Awake()
    {
        //Debug.LogError("Failed to instantiate door part!");

        // load xml and set it up
        //TextAsset xml_asset = (TextAsset)Resources.Load<TextAsset>(xml_file);

        //TextAsset xmlData = new TextAsset();
        //xmlData = (TextAsset)Resources.Load("Talents.xml", typeof(TextAsset));

        XmlDocument xml_doc = new XmlDocument();
        xml_doc.LoadXml(xml_asset.text);

        XmlNodeList columns = xml_doc.SelectNodes("/cabinet/column");

        door = new GameObject("Door");
        door.transform.parent = transform;
        door.transform.localPosition = new Vector3(0, 0, 0);

        Vector3 parent_position = transform.position;
        Quaternion parent_rotation = transform.rotation;

        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        int n_max = columns.Count - 1;
        int n = 0;
        foreach (XmlNode column in columns)
        {
            int m_max = column.ChildNodes.Count - 1;
            int m = 0;
            foreach (XmlNode row in column.ChildNodes)
            {
                GameObject prefab = null;

                switch (row.InnerText)
                {
                    case "P":
                        prefab = prefab_p;
                        break;
                    case "C":
                        prefab = prefab_c;
                        break;
                    case "G":
                        prefab = prefab_g;
                        break;
                    default:
                        prefab = prefab_p; // empty
                        break;
                }

                //get position of the obj that the script is attached to
                //Buiild interior of the cabinet
                //Vector3 position = transform.position + new Vector3(0, m, n);
                
                //Instantiate(prefab, position, Quaternion.identity, transform);

                Vector3 position = transform.position + new Vector3(0, m, n);
                GameObject instance = Instantiate(prefab);
                instance.transform.parent = transform; // Set parent first
                instance.transform.localRotation = Quaternion.identity; // Adjust local position
                instance.transform.localPosition = position; // Adjust local position
                instance.transform.localScale    = new Vector3(1, 1, 1);
               
                //door
                GameObject door_instance = Instantiate(prefab_door_part);
                door_instance.transform.parent = transform; // Set parent first
                door_instance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
                door_instance.transform.localPosition = position + new Vector3(0,0,0); // Adjust local position
                door_instance.transform.localScale = new Vector3(1, 1, 1);
                door_instance.transform.parent = door.transform; // Set parent first

                //door
                //GameObject door_part = Instantiate(prefab_door_part, position, Quaternion.Euler(0f, 0f, 0f), door.transform);

                
                //build corners
                if (n == 0 && m == m_max) //Build corner 1
                {
                    //Instantiate(shell_corner, position, Quaternion.Euler(0f, 0f, 0f), transform);
                    GameObject shell_corner_instance = Instantiate(shell_corner);
                    shell_corner_instance.transform.parent = transform; // Set parent first
                    shell_corner_instance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Adjust local position
                    shell_corner_instance.transform.localPosition = position; // Adjust local position
                    shell_corner_instance.transform.localScale    = new Vector3(1, 1, 1);
                }

                else if (n == n_max && m == m_max)//Build corner 2
                {
                    //Instantiate(shell_corner, position, Quaternion.Euler(90f, 0f, 0f), transform);
                    GameObject shell_corner2_instance = Instantiate(shell_corner);
                    shell_corner2_instance.transform.parent = transform; // Set parent first
                    shell_corner2_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Adjust local position
                    shell_corner2_instance.transform.localPosition = position; // Adjust local position
                    shell_corner2_instance.transform.localScale    = new Vector3(1, 1, 1);
                }
                else if (n == 0 && m == 0)//Build corner 3 (7)
                {
                    //Instantiate(shell_corner, position, Quaternion.Euler(-90f, 0f, 0f), transform);
                    //removed the bottom
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform; // Set parent first
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Adjust local position
                    shell_middle2_instance.transform.localPosition = position; // Adjust local position
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == n_max && m == 0)//Build corner 4 (build 9 if 3x3)
                {
                    //Instantiate(shell_corner, position, Quaternion.Euler(180f, 0f, 0f), transform); 
                    //removed the bottom
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform; // Set parent first
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Adjust local position
                    shell_middle2_instance.transform.localPosition = position; // Adjust local position
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                    
                }
                // build borders
                else if (n != 0 && n != n_max && m == m_max) //Build between corner 1 to corner 2 (build 2 if 3x3)
                {
                    //Instantiate(shell_middle, position, Quaternion.Euler(0f, 0f, 0f), transform);
                    GameObject shell_middle1_instance = Instantiate(shell_middle);
                    shell_middle1_instance.transform.parent = transform; // Set parent first
                    shell_middle1_instance.transform.localRotation = Quaternion.Euler(0, 0, 0); // Adjust local position
                    shell_middle1_instance.transform.localPosition = position; // Adjust local position
                    shell_middle1_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == 0 && m != m_max && m != 0)
                {
                    //Build between corner 1 to corner 3 (build 4 if 3x3)
                    //Instantiate(shell_middle, position, Quaternion.Euler(-90f, 0f, 0f), transform);
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform; // Set parent first
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // Adjust local position
                    shell_middle2_instance.transform.localPosition = position; // Adjust local position
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == n_max && m != m_max && m != 0)
                {
                    //Build between corner 2 to corner 4 (build 6 if 3x3)
                    //Instantiate(shell_middle, position, Quaternion.Euler(90f, 0f, 0f), transform);
                    GameObject shell_middle3_instance = Instantiate(shell_middle);
                    shell_middle3_instance.transform.parent = transform; // Set parent first
                    shell_middle3_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Adjust local position
                    shell_middle3_instance.transform.localPosition = position; // Adjust local position
                    shell_middle3_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                /* removed the bootom
                 * else if (n != 0 && n != n_max && m == 0) //Build between corner 3 to corner 4 (build 8 if 3x3)
                {
                    //Instantiate(shell_middle, position, Quaternion.Euler(180f, 0f, 0f), transform);
                }
                */
                else
                {
                    //Instantiate(shell_center, position, Quaternion.Euler(0f, 0f, 0f), transform);
                    GameObject shell_center_instance = Instantiate(shell_center);
                    shell_center_instance.transform.parent = transform; // Set parent first
                    shell_center_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Adjust local position
                    shell_center_instance.transform.localPosition = position; // Adjust local position
                    shell_center_instance.transform.localScale = new Vector3(1, 1, 1);
                }

                m++;
            }
            n++;
        }

        transform.position = parent_position;
        transform.rotation = parent_rotation;

        door.AddComponent<Door.DoorController>(); //adicionar o script paarra abrir a porta


        // Create a light and attach it to the cabinet
        //GameObject lightGameObject = new GameObject("CabinetLight");
        //cabinetLight = lightGameObject.AddComponent<Light>();
        //cabinetLight.type = LightType.Point;
        //cabinetLight.color = Color.white;
        //cabinetLight.intensity = 1.0f;
        //cabinetLight.range = 10.0f;

        // Set the light's position and parent
        //lightGameObject.transform.position = new Vector3(0, 1, 0); // Adjust the position as needed
        //lightGameObject.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the E key is pressed
        //if (Input.GetKeyDown(KeyCode.E))
        //{
            // Toggle the light on and off
            //cabinetLight.enabled = !cabinetLight.enabled;
        //}
    }
}
