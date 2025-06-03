using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// PlantGenerator generates procedural plant models using a Stochastic L-System.
/// Meaning it supports stochastic rules, different prefab types (branches, leaves, flowers, pot),
/// and can be configured through JSON files.
/// </summary>
public class PlantGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject branchPrefab;
    public GameObject leafPrefab;
    public GameObject flowerPrefab;
    public GameObject potPrefab;

    [Header("L-System Settings")]
    private string axiom; // The initial string to start the L-System
    public int iterations = 3; // Number of L-System iterations
    public string ruleFileName; // Name of the rules JSON file to load

    [Header("Plant Settings")]
    [Range(0.1f, 10f)] public float scale = 1f; // Global scale for all elements
    public float delta = 20f; // Rotation angle used by the turtle for drawing
    [Range(0.1f, 5f)] public float branchLength = 1f; // Base branch scale

    [Range(0f, 1f)] public float flowerSpawnProbability = 0.25f; // Probability of spawning a flower at branch ends

    private Vector3 startPositionPlant = Vector3.zero;
    private Dictionary<string, LSystemRule> rules = new Dictionary<string, LSystemRule>();

    void Start()
    {
        // Preload rule files from StreamingAssets
        string[] fileNames = new string[]
        {
            "LSystemRules1.json",
            "LSystemRules2.json",
            "LSystemRules3.json"
        };
        StartCoroutine(RuleFileManager.CopyRulesFromStreamingAssets(fileNames));

        // Trigger plant generation at startup
        GeneratePlant();
    }

    /// <summary>
    /// Load L-System rules from a JSON file and store them in the rules dictionary.
    /// </summary>
    void LoadRulesFromJSON(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("Rules file not found: " + filePath);
            return;
        }
        string jsonText = System.IO.File.ReadAllText(filePath);
        LSystemRuleSet[] ruleSets = JsonHelper.FromJson<LSystemRuleSet>(jsonText);
        rules.Clear();
        foreach (var ruleSet in ruleSets)
        {
            List<(string, float)> converted = new List<(string, float)>();
            foreach (var entry in ruleSet.successors)
                converted.Add((entry.successor, entry.probability));
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

    /// <summary>
    /// Recursively apply L-System rules to generate the final string.
    /// </summary>
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

    /// <summary>
    /// Interpret and render the L-System string using a turtle-based approach.
    /// </summary>
    void DrawLSystem(string lsystem, Vector3 startPosition, Transform parent)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Transform turtle = new GameObject("Turtle").transform;
        turtle.position = startPosition;
        int depth = 0;
        foreach (char c in lsystem)
        {
            float depthFactor = Mathf.Pow(0.9f, depth);
            float iterationFactor = Mathf.Pow(0.9f, iterations);
            float scaleFactor = depthFactor * iterationFactor;
            float shrinkFactor = Mathf.Pow(0.9f, iterations);
            switch (c)
            {
                case 'F': // Draw branch
                    GameObject branch = Instantiate(branchPrefab, turtle.position, turtle.rotation, parent);
                    branch.transform.localScale *= scale;
                    branch.transform.localScale = new Vector3(
                        branch.transform.localScale.x * shrinkFactor,
                        branch.transform.localScale.y * branchLength * scaleFactor,
                        branch.transform.localScale.z * shrinkFactor
                    );
                    float length = branch.transform.localScale.y;
                    branch.transform.Translate(Vector3.up * length / 2f);
                    turtle.Translate(Vector3.up * length);
                    branch.AddComponent<WindEffect>();
                    break;
                case 'L': // Draw leaf
                    if (leafPrefab != null)
                    {
                        Quaternion offsetRot = Quaternion.Euler(
                            Random.Range(-30f, 30f),
                            Random.Range(-90f, 90f),
                            Random.Range(-5f, 5f)
                        );
                        Quaternion randomRot = turtle.rotation * offsetRot;
                        GameObject leaf = Instantiate(leafPrefab, turtle.position, randomRot, parent);
                        leaf.transform.localScale *= scale * scaleFactor;
                        Renderer rend = leaf.GetComponentInChildren<Renderer>();
                        if (rend != null)
                        {
                            float minY = rend.bounds.min.y;
                            float pivotYOffset = (leaf.transform.position.y - minY) - 0.04f;
                            leaf.transform.position += leaf.transform.up * pivotYOffset;
                            leaf.transform.position += leaf.transform.forward * 0.05f;
                        }
                        leaf.AddComponent<WindEffect>();
                    }
                    break;
                case '[': // Push transform state
                    transformStack.Push(new TransformInfo(turtle.position, turtle.rotation));
                    depth++;
                    break;
                case ']': // Pop transform state and possibly spawn flower
                    if (transformStack.Count > 0)
                    {
                        TransformInfo ti = transformStack.Pop();
                        turtle.position = ti.position;
                        turtle.rotation = ti.rotation;
                        depth--;
                        if (flowerPrefab != null && Random.value < flowerSpawnProbability)
                        {
                            Quaternion randomRot = turtle.rotation * Quaternion.Euler(
                                Random.Range(-180f, 180f),
                                Random.Range(-180f, 180f),
                                Random.Range(-180f, 180f)
                            );
                            Vector3 flowerPosition = turtle.position + turtle.forward * 0.05f;
                            GameObject flower = Instantiate(flowerPrefab, flowerPosition, randomRot, parent);
                            flower.transform.localScale *= scale * scaleFactor;
                            flower.AddComponent<WindEffect>();
                        }
                    }
                    break;
                // Turtle rotation commands
                case '&': turtle.Rotate(Vector3.right * delta); break;
                case '^': turtle.Rotate(Vector3.right * -delta); break;
                case '/': turtle.Rotate(Vector3.forward * -delta); break;
                case '\\': turtle.Rotate(Vector3.forward * delta); break;
                case '+': turtle.Rotate(Vector3.up * delta); break;
                case '-': turtle.Rotate(Vector3.up * -delta); break;
            }
        }
        Destroy(turtle.gameObject); // Clean up turtle
    }

    /// <summary>
    /// Instantiate and place a pot at the base of the plant.
    /// </summary>
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

    /// <summary>
    /// Clears existing plant, if there is, and regenerates it based on current parameters.
    /// </summary>
    public void GeneratePlant()
    {
        // Clean up old components, if there are
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        if (!string.IsNullOrEmpty(ruleFileName))
        {
            string filePath = RuleFileManager.GetRuleFilePath(ruleFileName);
            LoadRulesFromJSON(filePath);
        }
        else
        {
            Debug.LogError("Rule config file name not set!");
            return;
        }
        Vector3 startPosition = transform.position;
        GeneratePot(ref startPosition);
        if (iterations > 0)
        {
            if (string.IsNullOrEmpty(axiom))
            {
                Debug.LogError("Axiom is null or empty! Cannot regenerate plant.");
                return;
            }
            string lSystem = GenerateLSystem(axiom, rules, iterations);
            DrawLSystem(lSystem, startPosition, transform);
            Debug.Log("Plant generated!");
        }
        StartCoroutine(ReattachPhysicsAfterFrame());
    }

    /// <summary>
    /// Adds colliders and rigidbody after rendering is complete for physics interaction.
    /// </summary>
    private IEnumerator ReattachPhysicsAfterFrame()
    {
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

    // Public method to get the current plant configuration
    public Plant GetValues()
    {
        return new Plant
        {
            iterations = this.iterations,
            ruleFileName = this.ruleFileName,
            scale = this.scale,
            branchLength = this.branchLength,
            delta = this.delta,
            flowerSpawnProbability = this.flowerSpawnProbability
        };
    }

    // Setters for runtime adjustment
    public void SetIterations(int value) => iterations = value;
    public void SetRuleConfigFile(string fileName) => ruleFileName = fileName;
    public void SetScale(float value) => scale = Mathf.Clamp(value, 0.1f, 10f);
    public void SetBranchLenght(float value) => branchLength = Mathf.Clamp(value, 0.1f, 10f);
    public void SetDelta(float value) => delta = value;
    public void SetFlowerSpawnProbability(float value) => flowerSpawnProbability = Mathf.Clamp01(value);

    /// <summary>
    /// Clears current plant GameObjects and resets physics components.
    /// </summary>
    public IEnumerator ResetPlant()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        gameObject.tag = "Untagged";
        yield return null;
    }

    /// <summary>
    /// Simulate a single step of plant growth by regenerating with a new iteration count.
    /// </summary>
    public void SimulateStep(int iteration)
    {
        SetIterations(iteration);
        GeneratePlant();
    }

    /// <summary>
    /// Data structure used to store turtle transform state during branching.
    /// </summary>
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

    /// <summary>
    /// Represents a single L-System rule with stochastic successors.
    /// </summary>
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
            return predecessor; // Fallback
        }
    }

    /// <summary>
    /// Serializable class for loading rule data from JSON.
    /// </summary>
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
