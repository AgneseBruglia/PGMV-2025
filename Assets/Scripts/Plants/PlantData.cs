using UnityEngine;

[System.Serializable]
public struct Plant
{
    public int iterations;
    public TextAsset ruleConfigFile;
    public float scale;
    public float delta;
    public float flowerSpawnProbability;
}
