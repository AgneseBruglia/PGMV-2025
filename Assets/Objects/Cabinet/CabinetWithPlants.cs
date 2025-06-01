using UnityEngine;
using System.IO;
using System.Xml;

public class CabinetWithPlants : MonoBehaviour
{
    public TextAsset xml_asset;

    // Prefab del cabinet (strutturali)
    public GameObject prefab_p; // blocco P
    public GameObject prefab_c; // blocco C
    public GameObject prefab_g; // blocco G
    public GameObject prefab_l; // luce
    public GameObject shell_corner; // angoli del cabinet
    public GameObject shell_middle; // bordi del cabinet
    public GameObject shell_center; // centro del cabinet
    public GameObject prefab_door_part; // porta

    // Prefab per la pianta (L-System)
    public GameObject branchPrefab; // ramo della pianta 
    public GameObject leafPrefab;   // foglia della pianta 
    public GameObject potPrefab;    // vaso della pianta 
    public GameObject prefab_flower; // opzionale: fiore
    public string plantRulesJson; // regole L-System

    private GameObject door;

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
                instance.transform.parent = transform;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localPosition = position;

                // Cabinet structure logic (angoli e bordi)
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
                if (branchPrefab == null || leafPrefab == null || potPrefab == null || plantRulesJson == null)
                {
                    Debug.LogError("🚫 Prefab della pianta o JSON mancante!");
                    continue;
                }

                GameObject plantInstance = new GameObject("ProceduralPlant");
                plantInstance.transform.position = child.position;
                plantInstance.transform.rotation = child.rotation;
                plantInstance.transform.localScale = Vector3.one;
                plantInstance.transform.parent = child;

                PlantGenerator generator = plantInstance.AddComponent<PlantGenerator>();
                generator.branchPrefab = branchPrefab;
                generator.leafPrefab = leafPrefab;
                generator.potPrefab = potPrefab;
                generator.flowerPrefab = prefab_flower;
                generator.ruleFileName = plantRulesJson;
                generator.iterations = 3;
                generator.scale = 0.3f;

                Debug.Log($"🌱 Pianta generata in {child.name}");
                count++;
            }
        }

        Debug.Log($"✅ Totale InsertPoint trovati: {count}");
    }

    void Update() { }
}
