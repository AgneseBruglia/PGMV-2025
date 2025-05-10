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
     * Initializes before application starts
     */
    private void Awake()
    {
        XmlDocument xml_doc = new XmlDocument();
        xml_doc.LoadXml(xml_asset.text);

        XmlNodeList columns = xml_doc.SelectNodes("/cabinet/column");

        Vector3 parent_position = transform.position;
        Quaternion parent_rotation = transform.rotation;

        // Prepara contenitore della porta
        door = new GameObject("Door");
        door.transform.parent = transform;
        door.transform.localPosition = Vector3.zero;

        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        int n_max = columns.Count - 1;
        int m_max = 0;
        int n = 0;

        foreach (XmlNode column in columns)
        {
             int column_m_max = column.ChildNodes.Count - 1; // Rinomina m_max in column_m_max per evitare conflitti
            m_max = Mathf.Max(m_max, column_m_max);
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

                // Posizione dell'oggetto
                Vector3 position = transform.position + new Vector3(0, m, n);
                GameObject instance = Instantiate(prefab);
                instance.transform.parent = transform;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localPosition = position;
                instance.transform.localScale = new Vector3(1, 1, 1);

                // Costruzione degli angoli
                if (n == 0 && m == m_max) // Build corner 1
                {
                    GameObject shell_corner_instance = Instantiate(shell_corner);
                    shell_corner_instance.transform.parent = transform;
                    shell_corner_instance.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    shell_corner_instance.transform.localPosition = position;
                    shell_corner_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == n_max && m == m_max) // Build corner 2
                {
                    GameObject shell_corner2_instance = Instantiate(shell_corner);
                    shell_corner2_instance.transform.parent = transform;
                    shell_corner2_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    shell_corner2_instance.transform.localPosition = position;
                    shell_corner2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == 0 && m == 0) // Build corner 3 (7)
                {
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform;
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                    shell_middle2_instance.transform.localPosition = position;
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == n_max && m == 0) // Build corner 4 (build 9 if 3x3)
                {
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform;
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    shell_middle2_instance.transform.localPosition = position;
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                // Costruzione dei bordi
                else if (n != 0 && n != n_max && m == m_max) // Build between corner 1 to corner 2
                {
                    GameObject shell_middle1_instance = Instantiate(shell_middle);
                    shell_middle1_instance.transform.parent = transform;
                    shell_middle1_instance.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    shell_middle1_instance.transform.localPosition = position;
                    shell_middle1_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == 0 && m != m_max && m != 0) // Build between corner 1 to corner 3
                {
                    GameObject shell_middle2_instance = Instantiate(shell_middle);
                    shell_middle2_instance.transform.parent = transform;
                    shell_middle2_instance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                    shell_middle2_instance.transform.localPosition = position;
                    shell_middle2_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (n == n_max && m != m_max && m != 0) // Build between corner 2 to corner 4
                {
                    GameObject shell_middle3_instance = Instantiate(shell_middle);
                    shell_middle3_instance.transform.parent = transform;
                    shell_middle3_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    shell_middle3_instance.transform.localPosition = position;
                    shell_middle3_instance.transform.localScale = new Vector3(1, 1, 1);
                }
                else // Costruzione del centro
                {
                    GameObject shell_center_instance = Instantiate(shell_center);
                    shell_center_instance.transform.parent = transform;
                    shell_center_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    shell_center_instance.transform.localPosition = position;
                    shell_center_instance.transform.localScale = new Vector3(1, 1, 1);
                }

                m++;
            }
            n++;
        }


        // Instanzia la porta DOPO aver calcolato m_max e n_max
        GameObject door_instance_final = Instantiate(prefab_door_part, door.transform);
        float centerY = m_max / 2f;
        float centerZ = n_max / 2f;
        door_instance_final.transform.localPosition = new Vector3(0f, centerY, centerZ);
        door_instance_final.transform.localRotation = Quaternion.identity;
        door_instance_final.transform.localScale = new Vector3(1f, m_max + 1, n_max + 1);

        // Assicurati che abbia un BoxCollider
        if (!door_instance_final.TryGetComponent<Collider>(out _))
            {
                Debug.LogWarning("Stiamo aggiungendo il collider");
                BoxCollider boxCollider = door_instance_final.AddComponent<BoxCollider>();
                boxCollider.size = door_instance_final.transform.localScale;
            }
        else{
            Debug.LogWarning("Il collider ci sta");
        }

        transform.position = parent_position;
        transform.rotation = parent_rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
