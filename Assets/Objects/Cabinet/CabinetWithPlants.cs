using UnityEngine;
using System.IO;
using System.Xml;

public class CabinetWithPlants : MonoBehaviour
{
    public TextAsset xml_asset;

    public GameObject prefab_p; // prefab_p
    public GameObject prefab_c; // prefabC
    public GameObject prefab_g; // prefabG
    public GameObject prefab_l; // light
    public GameObject shell_corner; // Cabinet part 1,3,7,9
    public GameObject shell_middle; // Cabinet part 2, 4, 6, 8
    public GameObject shell_center; // Cabinet part 5
    public GameObject prefab_door_part; // door child
    public GameObject plant_prefab; // Prefab della pianta

    private GameObject door; // door parent
    private Vector3 minPosition;
    private Vector3 maxPosition;

    private void Awake()
    {
        XmlDocument xml_doc = new XmlDocument();
        xml_doc.LoadXml(xml_asset.text);

        XmlNodeList columns = xml_doc.SelectNodes("/cabinet/column");

        Vector3 parent_position = transform.position;
        Quaternion parent_rotation = transform.rotation;

        door = new GameObject("Door");
        door.transform.parent = transform;
        door.transform.localPosition = Vector3.zero;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        int n_max = columns.Count - 1;
        int m_max = 0;
        int n = 0;

        foreach (XmlNode column in columns)
        {
            int column_m_max = column.ChildNodes.Count - 1;
            m_max = Mathf.Max(m_max, column_m_max);
            int m = 0;

            foreach (XmlNode row in column.ChildNodes)
            {
                GameObject prefab = null;

                switch (row.InnerText)
                {
                    case "P": prefab = prefab_p; break;
                    case "C": prefab = prefab_c; break;
                    case "G": prefab = prefab_g; break;
                    default: prefab = prefab_p; break;
                }

                Vector3 position = new Vector3(0, m, n);
                GameObject instance = Instantiate(prefab, transform);
                instance.transform.localPosition = position;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                // Struttura del cabinet (angoli e bordi)
                if (n == 0 && m == m_max)
                    Instantiate(shell_corner, position, Quaternion.Euler(0f, 0f, 0f), transform);
                else if (n == n_max && m == m_max)
                    Instantiate(shell_corner, position, Quaternion.Euler(90f, 0f, 0f), transform);
                else if (n == 0 && m == 0)
                    Instantiate(shell_middle, position, Quaternion.Euler(-90f, 0f, 0f), transform);
                else if (n == n_max && m == 0)
                    Instantiate(shell_middle, position, Quaternion.Euler(90f, 0f, 0f), transform);
                else if (n != 0 && n != n_max && m == m_max)
                    Instantiate(shell_middle, position, Quaternion.identity, transform);
                else if (n == 0 && m != m_max && m != 0)
                    Instantiate(shell_middle, position, Quaternion.Euler(-90f, 0f, 0f), transform);
                else if (n == n_max && m != m_max && m != 0)
                    Instantiate(shell_middle, position, Quaternion.Euler(90f, 0f, 0f), transform);
                else
                    Instantiate(shell_center, position, Quaternion.Euler(90f, 0f, 0f), transform);

                m++;
            }
            n++;
        }

        // Porta
        GameObject door_instance = Instantiate(prefab_door_part, door.transform);
        float centerY = m_max / 2f;
        float centerZ = n_max / 2f;
        door_instance.transform.localPosition = new Vector3(0f, centerY, centerZ);
        door_instance.transform.localRotation = Quaternion.identity;
        door_instance.transform.localScale = new Vector3(1f, m_max + 1, n_max + 1);

        // Collider
        if (!door_instance.TryGetComponent<Collider>(out _))
        {
            BoxCollider boxCollider = door_instance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(
                1f + 10f,
                m_max + 1 + 2f,
                n_max + 1 + 10f
            );
            boxCollider.center = Vector3.zero;
        }

        transform.position = parent_position;
        transform.rotation = parent_rotation;

        // Luce sopra il cabinet
        if (prefab_l != null)
        {
            Vector3 lightPosition = new Vector3(0f, m_max + 1, n_max / 2f);
            GameObject light_instance = Instantiate(prefab_l, transform);
            light_instance.transform.localPosition = lightPosition;
            light_instance.transform.localRotation = Quaternion.identity;
            light_instance.transform.localScale = new Vector3(1f, 1f, n_max + 1);
        }
    }

    private void Start()
    {
        InsertPlants();
    }

    private void InsertPlants()
{
    Transform[] allChildren = GetComponentsInChildren<Transform>(true);
    int count = 0;

    foreach (Transform child in allChildren)
    {
        if (child.name.Equals("InsertPoint", System.StringComparison.OrdinalIgnoreCase))
        {
            if (plant_prefab == null)
            {
                Debug.LogError("🚫 plant_prefab non assegnato!");
                return;
            }

            // Istanzia la pianta in scena (senza ancora impostare la posizione)
            GameObject plantInstance = Instantiate(plant_prefab, transform);
            Transform plantBase = plantInstance.transform.Find("PlantBase");

            if (plantBase == null)
            {
                Debug.LogError("🚫 Il prefab della pianta deve contenere un GameObject chiamato 'PlantBase'");
                return;
            }

            // Calcola offset dal root al PlantBase
            Vector3 offset = plantBase.position - plantInstance.transform.position;

            // Posiziona il root della pianta in modo che PlantBase combaci con l'InsertPoint
            plantInstance.transform.position = child.position - offset;
            plantInstance.transform.rotation = child.rotation;
            plantInstance.transform.localScale = Vector3.one;

            // Rendi la pianta figlia dell'InsertPoint (se desiderato)
            plantInstance.transform.parent = child;

            Debug.Log($"🌱 Pianta instanziata correttamente in {child.name}");
            count++;
        }
    }

    Debug.Log($"✅ Totale InsertPoint trovati: {count}");
}



    void Update() { }
}
