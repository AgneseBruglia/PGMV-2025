using UnityEngine;
using System.IO;
using System.Xml;

public class Cabinet : MonoBehaviour
{
    public string xml_file = "XML_Cabinet_1.xml"; // Defaults to cabinet
    public GameObject prefab_p; // prefab_p
    public GameObject prefab_c; // prefabC
    public GameObject prefab_g; // prefabG

    /*
     * Initializes before appication starts
     */
    private void Awake()
    {
        // load xml and set it up
        TextAsset xml_asset = (TextAsset)Resources.Load<TextAsset>(xml_file);

        if (xml_asset == null)
        {
            Debug.LogError("XML file not found: " + xml_file);
            return;
        }

        XmlDocument xml_doc = new XmlDocument();
        xml_doc.LoadXml(xml_asset.text);

        XmlNodeList columns = xml_doc.SelectNodes("/cabinet/column");

        int i = 0;
        foreach (XmlNode column in columns)
        {
            int w = 0;
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
                        Debug.LogError("node is not configured correctly");
                        continue;
                }
                
                Vector3 position = new Vector3(i, w, 0);
                Instantiate(prefab, position, Quaternion.identity);
                w++;
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
