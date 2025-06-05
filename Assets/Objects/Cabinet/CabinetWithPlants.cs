using UnityEngine;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// This script builds a cabinet structure based on XML configuration,
/// adds decorative elements (shells, plants), a door with audio,
/// and optionally a light prefab at the top.
/// </summary>
public class CabinetWithPlants : MonoBehaviour
{
    // XML file containing the cabinet layout
    public TextAsset xml_asset;

    // Prefabs for cabinet blocks
    public GameObject prefab_p;
    public GameObject prefab_c;
    public GameObject prefab_g;
    public GameObject prefab_l;

    // Decorative shell prefabs
    public GameObject shell_corner;
    public GameObject shell_middle;
    public GameObject shell_center;

    // Door prefab and related audio
    public GameObject prefab_door_part;
    public AudioClip cabinet_door_open_close_clip;

    // Plant components
    public GameObject branchPrefab;
    public GameObject leafPrefab;
    public GameObject potPrefab;
    public GameObject prefab_flower;

    [Header("List of available plant rule files (only JSON filenames)")]
    public List<string> plantRuleFileNames = new();

    private void Awake()
    {
        // Load and parse XML
        XmlDocument xml_doc = new XmlDocument();
        xml_doc.LoadXml(xml_asset.text);
        XmlNodeList columns = xml_doc.SelectNodes("/cabinet/column");

        // Save initial position/rotation to restore later
        Vector3 parent_position = transform.position;
        Quaternion parent_rotation = transform.rotation;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Track max dimensions
        int n_max = columns.Count - 1;
        int m_max = 0;
        int n = 0;

        // Build cabinet structure column by column
        foreach (XmlNode column in columns)
        {
            int column_m_max = column.ChildNodes.Count - 1;
            m_max = Mathf.Max(m_max, column_m_max);
            int m = 0;

            foreach (XmlNode row in column.ChildNodes)
            {
                // Choose prefab based on XML tag
                GameObject prefab = row.InnerText switch
                {
                    "P" => prefab_p,
                    "C" => prefab_c,
                    "G" => prefab_g,
                    _ => prefab_p
                };

                // Instantiate cabinet cell
                Vector3 position = new Vector3(0, m, n);
                GameObject instance = Instantiate(prefab, transform);
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localPosition = position;

                // Instantiate cabinet shell decorations
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

        // Instantiate the cabinet door in the center
        GameObject door_instance = Instantiate(prefab_door_part, transform);
        float centerY = m_max / 2f;
        float centerZ = n_max / 2f;
        door_instance.transform.localPosition = new Vector3(0f, centerY, centerZ);
        door_instance.transform.localRotation = Quaternion.identity;
        door_instance.transform.localScale = new Vector3(1f, m_max + 1, n_max + 1);

        // Add collider if not present
        if (!door_instance.TryGetComponent<Collider>(out _))
        {
            BoxCollider boxCollider = door_instance.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(11f, m_max + 3f, n_max + 11f);
            boxCollider.center = Vector3.zero;
        }

        // Restore original transform
        transform.position = parent_position;
        transform.rotation = parent_rotation;

        // Instantiate the light prefab on top of the cabinet
        if (prefab_l != null)
        {
            Vector3 lightPosition = new Vector3(0f, m_max + 0.68f, n_max / 2f);
            GameObject light_instance = Instantiate(prefab_l, transform);
            light_instance.transform.localPosition = lightPosition;
            light_instance.transform.localRotation = Quaternion.identity;
            light_instance.transform.localScale = new Vector3(1f, 1f, n_max + 1);
        }

        // Add audio source to the door
        AudioSource audioSource = door_instance.AddComponent<AudioSource>();
        audioSource.clip = cabinet_door_open_close_clip;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1.0f;
        audioSource.Stop();

        // Add plants inside the cabinet
        InsertPlants();
    }

    /// <summary>
    /// Instantiates plants inside the cabinet based on available rules and insert points.
    /// </summary>
    private void InsertPlants()
    {
        if (plantRuleFileNames == null || plantRuleFileNames.Count == 0)
        {
            Debug.LogWarning("⚠️ No plant rule files configured!");
            return;
        }

        // Find all children named "InsertPoint"
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        List<Transform> insertPoints = new();

        foreach (Transform child in allChildren)
        {
            if (child.name.Equals("InsertPoint", System.StringComparison.OrdinalIgnoreCase))
                insertPoints.Add(child);
        }

        // Instantiate plants based on rules
        int ruleIndex = 0;
        for (int i = 0; i < insertPoints.Count; i++)
        {
            Transform insert = insertPoints[i];
            string ruleFileName = plantRuleFileNames[ruleIndex];

            if (string.IsNullOrEmpty(ruleFileName))
            {
                Debug.LogError($"🚫 Missing rule filename for rule #{ruleIndex}");
                continue;
            }

            GameObject plantInstance = new GameObject($"Plant_{i}_Rule{ruleIndex}");
            plantInstance.transform.position = insert.position;
            plantInstance.transform.rotation = insert.rotation;
            plantInstance.transform.localScale = Vector3.one;
            //plantInstance.transform.parent = insert;

            // Configure the plant generator
            PlantGenerator generator = plantInstance.AddComponent<PlantGenerator>();
            generator.branchPrefab = branchPrefab;
            generator.leafPrefab = leafPrefab;
            generator.potPrefab = potPrefab;
            generator.flowerPrefab = prefab_flower;
            generator.ruleFileName = ruleFileName;
            generator.iterations = 3;
            generator.scale = 0.3f;

            ruleIndex = (ruleIndex + 1) % plantRuleFileNames.Count;

            // Put plant in cubicle
            if (insert.transform.parent.transform.CompareTag("Cubicle"))
            {
                insert.transform.parent.transform.gameObject.GetComponent<CubicleController>().initPlant(plantInstance);
            }

            // Put plant in drawer
            if (insert.transform.parent.transform.CompareTag("Drawer"))
            {
                insert.transform.parent.transform.gameObject.GetComponent<DrawerController>().initPlant(plantInstance);
            }
        }
    }

    // Optional Update method (currently unused)
    void Update() { }
}
