using System.Collections.Generic;
using UnityEngine;

public class Plant1Generator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject branchPrefab;

    [Header("L-System Settings")]
    public string axiom = "F";
    public int iterations = 2;

    [Header("Pot Settings")]
    public GameObject potPrefab;

    private Dictionary<string, LSystemRule> rules = new Dictionary<string, LSystemRule>();

    void Start()
    {
        // Stochastic rules definition 
        rules["F"] = new LSystemRule
        {
            predecessor = "F",
            stochasticRules = new List<(string, float)>
            {
                ("F[+F]F[-F]F", 0.3f),
                ("[+F]F[-F]", 0.4f),
                ("F[+F[+F]]F[-F[-F]]F", 0.3f)
            }
        };
        Vector3 startPosition = this.transform.position;
        if (potPrefab != null)
        {
            GameObject pot = Instantiate(potPrefab, startPosition, Quaternion.identity);
            pot.transform.SetParent(this.transform);
            // Cerca la height del vaso dal suo renderer (se esiste)
            Renderer rend = pot.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                float potHeight = rend.bounds.size.y;
                startPosition += Vector3.up * potHeight;
            }
        }
        string lSystem = GenerateLSystem(axiom, rules, iterations);
        Debug.Log("L-System finale: " + lSystem);
        DrawLSystem(lSystem);
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

    void DrawLSystem(string lsystem)
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        Transform turtle = new GameObject("Turtle").transform;
        turtle.position = this.transform.position + Vector3.up * 0.2f; // sostituire con startPosition se vuoi passarla come argomento
        foreach (char c in lsystem)
        {
            switch (c)
            {
                case 'F':
                    GameObject branch = Instantiate(branchPrefab, turtle.position, turtle.rotation);
                    branch.transform.SetParent(this.transform);
                    turtle.Translate(Vector3.up * Random.Range(0.15f, 0.3f)); // variazione lunghezza ramo
                    break;
                case '+':
                    turtle.Rotate(Vector3.forward * Random.Range(20f, 60f));
                    break;
                case '-':
                    turtle.Rotate(Vector3.forward * -Random.Range(20f, 60f));
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
                if (rand <= cumulative) return succ;
            }
            return predecessor;
        }
    }
}
