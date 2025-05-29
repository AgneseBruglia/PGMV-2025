using System.Collections.Generic;
using System.Collections;
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

    private Vector3 startPositionPlant = Vector3.zero;

    private Dictionary<string, LSystemRule> rules = new Dictionary<string, LSystemRule>();

    void Start()
    {
        LoadRulesFromJSON();
        Vector3 startPosition = transform.position;

        string lSystem = GenerateLSystem(axiom, rules, iterations);
        Debug.Log("Final L-System: " + lSystem);

        GeneratePot(ref startPosition);        

        // Draws the plant
        DrawLSystem(lSystem, startPosition, gameObject.transform);

        // Calculates bounds for the boxcollider
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
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
            BoxCollider box = gameObject.AddComponent<BoxCollider>();

            // Convert bounds center and size from world space to local space
            box.center = gameObject.transform.InverseTransformPoint(combinedBounds.center);
            box.size = gameObject.transform.InverseTransformVector(combinedBounds.size);
            box.size *= 0.9f;

            box.isTrigger = false;

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            gameObject.tag = "Plant";
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

        if (ruleSets.Length > 0)
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

    void GeneratePot(ref Vector3 position)
    {
        if (potPrefab != null)
        {
            GameObject pot = Instantiate(potPrefab, position, Quaternion.identity, gameObject.transform);
            pot.transform.localScale *= scale;

            Renderer rend = pot.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float potHeight = rend.bounds.size.y;
                position += Vector3.up * potHeight;
            }
        }
    }

    // Getter function for the plant values
    public Plant GetValues()
    {
        return new Plant
        {
            iterations = this.iterations,
            ruleConfigFile = this.ruleConfigFile,
            scale = this.scale,
            delta = this.delta,
            flowerSpawnProbability = this.flowerSpawnProbability
        };
    }

    public void SetIterations(int value)
    {
        iterations = value;
    }

    public void SetRuleConfigFile(TextAsset file)
    {
        ruleConfigFile = file;
    }

    public void SetScale(float value)
    {
        scale = Mathf.Clamp(value, 0.1f, 10f);
    }

    public void SetDelta(float value)
    {
        delta = value;
    }

    public void SetFlowerSpawnProbability(float value)
    {
        flowerSpawnProbability = Mathf.Clamp01(value);
    }

    // Generates the new plant after changes in the interface
    public void RegeneratePlant()
    {
        // Rimuovi i componenti fisici se esistono
        BoxCollider oldCollider = GetComponent<BoxCollider>();
        if (oldCollider != null)
            Destroy(oldCollider);

        Rigidbody oldRb = GetComponent<Rigidbody>();
        if (oldRb != null)
            Destroy(oldRb);

        // Rimuovi i figli (pianta vecchia)
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Ricarica le regole
        LoadRulesFromJSON();

        Vector3 startPosition = transform.position;

        // Genera il vaso e aggiorna posizione iniziale
        GeneratePot(ref startPosition);

        // Crea L-system e disegna la pianta
        if (iterations > 0)
        {
            if (string.IsNullOrEmpty(axiom))
            {
                Debug.LogError("Axiom is null or empty! Cannot regenerate plant.");
                return;
            }

            string lSystem = GenerateLSystem(axiom, rules, iterations);
            DrawLSystem(lSystem, startPosition, transform);

            Debug.Log("Plant regenerated!");
        }

        // Aspetta un frame e poi aggiungi Rigidbody e BoxCollider
        StartCoroutine(ReattachPhysicsAfterFrame());
    }

    private IEnumerator ReattachPhysicsAfterFrame()
    {
        // Aspetta un frame per assicurarsi che Destroy() sia completato
        yield return null;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                combinedBounds.Encapsulate(renderers[i].bounds);

            if (GetComponent<BoxCollider>() == null)
            {
                BoxCollider box = gameObject.AddComponent<BoxCollider>();
                box.center = transform.InverseTransformPoint(combinedBounds.center);
                box.size = transform.InverseTransformVector(combinedBounds.size) * 0.9f;
                box.isTrigger = false;
            }

            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            gameObject.tag = "Plant";
        }
        else
        {
            Debug.LogWarning("No renderers found to calculate bounds.");
        }
    }

    public IEnumerator ResetPlant()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        BoxCollider oldCollider = GetComponent<BoxCollider>();
        if (oldCollider != null)
            Destroy(oldCollider);

        Rigidbody oldRb = GetComponent<Rigidbody>();
        if (oldRb != null)
            Destroy(oldRb);

        gameObject.tag = "Untagged";

        yield return null;
    }

    public void SimulateStep(int iteration)
    {
        Debug.Log($"SimulateStep chiamato con iteration: {iteration}");
        SetIterations(iteration);
        RegeneratePlant();
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
