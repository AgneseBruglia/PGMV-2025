using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject branchPrefab;
    public GameObject leafPrefab;
    public GameObject flowerPrefab;
    public GameObject potPrefab;

    [Header("L-System Settings")]
    private string axiom;
    public int iterations = 3;
    public TextAsset ruleConfigFile;

    [Header("Plant Settings")]
    [Range(0.1f, 10f)]
    public float scale = 1f;
    public float delta = 20f;
    [Range(0f, 1f)]
    public float flowerSpawnProbability = 0.25f;

    [Header("Plant UI Controller")]
    public PlantUIController plantUIController;

    private Dictionary<string, LSystemRule> rules = new Dictionary<string, LSystemRule>();

    void Start()
    {
        LoadRulesFromJSON();
        Vector3 startPosition = transform.position;

        string lSystem = GenerateLSystem(axiom, rules, iterations);
        Debug.Log("Final L-System: " + lSystem);

        GameObject plant = new GameObject("Plant");
        plant.transform.SetParent(this.transform);

        if (potPrefab != null)
        {
            GameObject pot = Instantiate(potPrefab, startPosition, Quaternion.identity, plant.transform);
            pot.transform.localScale *= scale;
            Renderer rend = pot.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float potHeight = rend.bounds.size.y;
                startPosition += Vector3.up * potHeight;
            }
        }        

        // Draws the plant
        DrawLSystem(lSystem, startPosition, plant.transform);

        // Calculates bounds for the boxcollider
        Renderer[] renderers = plant.GetComponentsInChildren<Renderer>();
        Bounds combinedBounds = new Bounds();
        bool initialized = false;

        foreach (Renderer rend in renderers)
        {
            if (!initialized)
            {
                combinedBounds = rend.bounds;
                initialized = true;
            }
            else
            {
                combinedBounds.Encapsulate(rend.bounds);
            }
        }

        if (initialized)
        {
            BoxCollider box = plant.AddComponent<BoxCollider>();

            // Convert bounds center and size from world space to local space
            box.center = plant.transform.InverseTransformPoint(combinedBounds.center);
            box.size = plant.transform.InverseTransformVector(combinedBounds.size);
            box.size *= 0.9f;

            box.isTrigger = true;

            Rigidbody rb = plant.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogWarning("No renderers found to calculate bounds.");
        }
    }

    void LoadRulesFromJSON()
    {
        if (ruleConfigFile == null)
        {
            Debug.LogError("Rules not found: assign a JSON file to 'ruleConfigFile'");
            return;
        }

        LSystemRuleSet[] ruleSets = JsonHelper.FromJson<LSystemRuleSet>(ruleConfigFile.text);
        rules.Clear();
        foreach (var ruleSet in ruleSets)
        {
            List<(string, float)> converted = new List<(string, float)>();
            foreach (var entry in ruleSet.successors)
            {
                converted.Add((entry.successor, entry.probability));
            }
            rules[ruleSet.predecessor] = new LSystemRule
            {
                predecessor = ruleSet.predecessor,
                stochasticRules = converted
            };
        }

        if (string.IsNullOrEmpty(axiom) && ruleSets.Length > 0)
        {
            axiom = ruleSets[0].predecessor;
        }
    }

    string GenerateLSystem(string axiom, Dictionary<string, LSystemRule> rules, int iterations)
    {
        string current = axiom;
        for (int i = 0; i < iterations; i++)
        {
            string next = "";
            foreach (char c in current)
            {
                string key = c.ToString();
                if (rules.ContainsKey(key))
                    next += rules[key].GetSuccessor();
                else
                    next += c;
            }
            current = next;
        }
        return current;
    }

    void DrawLSystem(string lsystem, Vector3 startPosition, Transform parent)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Transform turtle = new GameObject("Turtle").transform;
        turtle.position = startPosition;

        foreach (char c in lsystem)
        {
            switch (c)
            {
                case 'F':
                    GameObject branch = Instantiate(branchPrefab, turtle.position, turtle.rotation, parent);
                    branch.transform.localScale *= scale;
                    float length = branch.transform.localScale.y;
                    branch.transform.Translate(Vector3.up * length / 2f);
                    turtle.Translate(Vector3.up * length);
                    branch.AddComponent<WindEffect>();
                    break;
                case 'L':
                    if (leafPrefab != null)
                    {
                        Quaternion offsetRot = Quaternion.Euler(
                            Random.Range(-30f, 30f),
                            Random.Range(-90f, 90f),
                            Random.Range(-5f, 5f)
                        );
                        Quaternion randomRot = turtle.rotation * offsetRot;
                        GameObject leaf = Instantiate(leafPrefab, turtle.position, randomRot, parent);
                        leaf.transform.localScale *= scale;
                        Renderer rend = leaf.GetComponentInChildren<Renderer>();
                        if (rend != null)
                        {
                            float minY = rend.bounds.min.y;
                            float desiredOffsetFromMin = 0.04f;
                            float pivotYOffset = (leaf.transform.position.y - minY) - desiredOffsetFromMin;
                            float zOffsetAmount = 0.05f;
                            leaf.transform.position += leaf.transform.up * pivotYOffset;
                            leaf.transform.position += leaf.transform.forward * zOffsetAmount;
                        }
                        leaf.AddComponent<WindEffect>();
                    }
                    break;
                case '&':
                    turtle.Rotate(Vector3.right * delta);
                    break;
                case '^':
                    turtle.Rotate(Vector3.right * -delta);
                    break;
                case '/':
                    turtle.Rotate(Vector3.forward * -delta);
                    break;
                case '\\':
                    turtle.Rotate(Vector3.forward * delta);
                    break;
                case '+':
                    turtle.Rotate(Vector3.up * delta);
                    break;
                case '-':
                    turtle.Rotate(Vector3.up * -delta);
                    break;
                case '[':
                    transformStack.Push(new TransformInfo(turtle.position, turtle.rotation));
                    break;
                case ']':
                    if (transformStack.Count > 0)
                    {
                        TransformInfo ti = transformStack.Pop();
                        turtle.position = ti.position;
                        turtle.rotation = ti.rotation;
                        if (flowerPrefab != null && Random.value < flowerSpawnProbability)
                        {
                            Quaternion randomRot = turtle.rotation * Quaternion.Euler(
                                Random.Range(-180f, 180f),
                                Random.Range(-180f, 180f),
                                Random.Range(-180f, 180f)
                            );
                            Vector3 offset = turtle.forward * 0.05f;
                            Vector3 flowerPosition = turtle.position + offset;
                            GameObject flower = Instantiate(flowerPrefab, flowerPosition, randomRot, parent);
                            flower.transform.localScale *= scale;
                            flower.AddComponent<WindEffect>();
                        }
                    }
                    break;
            }
        }

        Destroy(turtle.gameObject);
    }

    struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public TransformInfo(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    public class LSystemRule
    {
        public string predecessor;
        public List<(string successor, float probability)> stochasticRules;
        public string GetSuccessor()
        {
            float rand = Random.Range(0f, 1f);
            float cumulative = 0f;
            foreach (var (succ, prob) in stochasticRules)
            {
                cumulative += prob;
                if (rand <= cumulative)
                    return succ;
            }
            return predecessor;
        }
    }

    [System.Serializable]
    public class LSystemRuleSet
    {
        public string predecessor;
        public LSystemSuccessor[] successors;

        [System.Serializable]
        public class LSystemSuccessor
        {
            public string successor;
            public float probability;
        }
    }
}
